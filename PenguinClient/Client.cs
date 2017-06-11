using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace PenguinClient
{
	public class Client : IDisposable
	{
		#region Fields

		private TextWriter output;

		private TextWriter error;

		private IO io;

		private int internalRoomId;

		private int id;

		private Dictionary<int, Penguin> penguins;

		#endregion

		#region Properties

		public TextWriter Output { get { return output; } }

		public TextWriter Error { get { return error; } }

		public int InternalRoomId { get { return internalRoomId; } }

		public bool Connected { get { return io.Connected; } }

		#endregion

		#region Constructors

		public Client(TextWriter output, TextWriter error)
		{
			this.output = output;
			this.error = error;
			internalRoomId = -1;
		}

		public Client(TextWriter output) : this(output, TextWriter.Null) { }

		public Client() : this(TextWriter.Null, TextWriter.Null) { }

		#endregion

		#region Methods

		private static string SwappedMD5(string value)
		{
			string hash = string.Empty;
			using (MD5 md5 = MD5.Create())
			{
				byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(value));
				foreach (byte b in bytes)
				{
					hash += b.ToString("x2");
				}
			}
			return hash.Substring(16, 16) + hash.Substring(0, 16);
		}

		private bool VersionCheck(int version)
		{
			output.WriteLine("Sending \"verChk\" request...");
			io.Send(string.Format("<msg t=\"sys\"><body action=\"verChk\" r=\"0\"><ver v=\"{0}\"/></body></msg>", version));
			string response = io.Receive();
			if (response.Contains("apiOK"))
				return true;
			if (response.Contains("apiKO"))
				return false;
			error.WriteLine("Invalid response");
			return false;
		}

		private string ReceiveKey()
		{
			output.WriteLine("Sending \"rndK\" request...");
			output.WriteLine();
			io.Send("<msg t=\"sys\"><body action=\"rndK\" r=\"-1\"></body></msg>");
			string response = io.Receive();
			if (response.Contains("rndK"))
			{
				Regex regex = new Regex(@"<k>(<!\[CDATA\[)?(.*?)(\]\]>)?<\/k>");
				if (regex.IsMatch(response))
				{
					string key = regex.Match(response).Groups[2].Value;
					output.WriteLine("Received key: {0}", key);
					return key;
				}
			}
			error.WriteLine("Invalid response");
			return null;
		}

		private string[] ReceivePacket(bool error)
		{
			string data = io.Receive();
			if (data[0] == '%')
			{
				string[] packet = data.Split('%');
				if (error && packet[2] == "e")
				{
					HandleError(packet);
					return null;
				}
				return packet;
			}
			this.error.WriteLine("Invalid packet");
			return null;
		}

		private string[] ReceivePacket()
		{
			return ReceivePacket(true);
		}

		private void HandleError(string[] packet)
		{
			string code = packet[4];
			string message = GetErrorMessage(code);
			if (message == null)
				error.WriteLine("Error {0}", code);
			else
				error.WriteLine("Error {0}: {1}", code, message);
		}

		private string GetErrorMessage(string code)
		{
			switch (code)
			{
				case "1":
					return "Connection lost";
				case "2":
					return "Time out";
				case "3":
					return "Multi connections";
				case "4":
					return "Disconnect";
				case "5":
					return "Kick";
				case "6":
					return "Connection not allowed";
				case "100":
					return "Name not found";
				case "101":
					return "Password wrong";
				case "103":
					return "Server full";
				case "104":
					return "Old salt error";
				case "130":
					return "Password required";
				case "131":
					return "Password short";
				case "132":
					return "Password long";
				case "140":
					return "Name required";
				case "141":
					return "Name short";
				case "142":
					return "Name long";
				case "150":
					return "Login flooding";
				case "200":
					return "Player in room";
				case "210":
					return "Room full";
				case "211":
					return "Game full";
				case "212":
					return "Room capacity rule";
				case "213":
					return "Room does not exist";
				case "400":
					return "Already own inventory item";
				case "401":
					return "Not enough coins";
				case "403":
					return "Max furniture items";
				case "406":
					return "Max pufflecare items";
				case "407":
					return "Max pufflehat items";
				case "408":
					return "Already own superplay item";
				case "409":
					return "Max cj mats";
				case "402":
					return "Item not exist";
				case "410":
					return "Item not available";
				case "405":
					return "Not enough medals";
				case "441":
					return "Name not allowed";
				case "440":
					return "Puffle limit m";
				case "442":
					return "Puffle limit nm";
				case "500":
					return "Already own igloo";
				case "501":
					return "Already own floor";
				case "502":
					return "Already own location";
				case "601":
					return "Ban duration";
				case "602":
					return "Ban an hour";
				case "603":
					return "Ban forever";
				case "610":
					return "Auto ban";
				case "611":
					return "Hacking auto ban";
				case "800":
					return "Game cheat";
				case "851":
					return "Invalid room id specified in j#jr";
				case "900":
					return "Account not activate";
				case "901":
					return "Buddy limit";
				case "910":
					return "Play time up";
				case "911":
					return "Out play time";
				case "913":
					return "Grounded";
				case "914":
					return "Play time ending";
				case "915":
					return "Play hours ending";
				case "916":
					return "Play hours up";
				case "917":
					return "Play hours hasnt start";
				case "918":
					return "Play hours update";
				case "990":
					return "System reboot";
				case "999":
					return "Not member";
				case "1000":
					return "No db connection";
				case "10001":
					return "No socket connection";
				case "10002":
					return "Timeout";
				case "10003":
					return "Password save prompt";
				case "10004":
					return "Socket lost connection";
				case "10005":
					return "Load error";
				case "10006":
					return "Max igloo furniture error";
				case "10007":
					return "Multiple connections";
				case "10008":
					return "Connection timeout";
				case "10009":
					return "Max stampbook cover items";
				case "10010":
					return "Web service load error";
				case "10011":
					return "Web service send error";
				case "10104":
					return "Chrome mac login error";
				case "20001":
					return "Redemption connection lost";
				case "20002":
					return "Redemption already have item";
				case "20103":
					return "Redemption server full";
				case "20140":
					return "Name required redemption";
				case "20141":
					return "Name short redemption";
				case "20130":
					return "Password required redemption";
				case "20131":
					return "Password short redemption";
				case "20710":
					return "Redemption book id not exist";
				case "20711":
					return "Redemption book already redeemed";
				case "20712":
					return "Redemption wrong book answer";
				case "20713":
					return "Redemption book too many attempts";
				case "20720":
					return "Redemption code not found";
				case "20721":
					return "Redemption code already redeemed";
				case "20722":
					return "Redemption too many attempts";
				case "20723":
					return "Redemption catalog not available";
				case "20724":
					return "Redemption no exclusive redeems";
				case "20725":
					return "Redemption code group redeemed";
				case "20726":
					return "Redemption code expired";
				case "20730":
					return "Redemption puffles max";
				case "21700":
					return "Redemption puffle invalid";
				case "21701":
					return "Redemption puffle code max";
				case "21702":
					return "Redemption code too short";
				case "21703":
					return "Redemption code too long";
				case "21704":
					return "Golden code not ready";
				case "21705":
					return "Redemption puffle name empty";
			}
			return null;
		}

		private string Login(string username, string password, int version)
		{
			output.WriteLine("Logging in...");
			bool ok = VersionCheck(version);
			if (!ok)
				return null;
			string key = ReceiveKey();
			if (key == null)
				return null;
			string hash = SwappedMD5(SwappedMD5(password).ToUpper() + key + "Y(02.>'H}t\":E1");
			io.Send(string.Format("<msg t=\"sys\"><body action=\"login\" r=\"0\"><login z=\"w1\"><nick><![CDATA[{0}]]></nick><pword><![CDATA[{1}]]></pword></login></body></msg>", username, hash));
			string[] packet;
			do
			{
				packet = ReceivePacket();
				if (packet == null)
					return null;
			} while (packet[2] != "l");
			id = int.Parse(packet[4]);
			key = packet[5];
			output.WriteLine("Logged in.");
			return key;
		}

		private bool JoinServer(string username, string loginKey, int version)
		{
			output.WriteLine("Joining server...");
			bool ok = VersionCheck(version);
			if (!ok)
				return false;
			string key = ReceiveKey();
			if (key == null)
				return false;
			string hash = SwappedMD5(loginKey + key) + loginKey;
			io.Send(string.Format("<msg t=\"sys\"><body action=\"login\" r=\"0\"><login z=\"w1\"><nick><![CDATA[{0}]]></nick><pword><![CDATA[{1}]]></pword></login></body></msg>", username, hash));
			string[] packet;
			do
			{
				packet = ReceivePacket();
				if (packet == null)
					return false;
			} while (packet[2] != "l");
			io.Send(string.Format("%xt%s%j#js%{0}%{1}%{2}%en%", internalRoomId, id, loginKey));
			do
			{
				packet = ReceivePacket();
				if (packet == null)
					return false;
			} while (packet[2] != "js");
			output.WriteLine("Joined server.");
			return true;
		}

		private void Listen()
		{
			output.WriteLine("Listening to packets...");
			while (true)
			{
				string[] packet = ReceivePacket();
				if (packet != null)
					HandlePacket(packet);
			}
		}

		private void HandlePacket(string[] packet)
		{
			string op = packet[2];
			//TODO
		}

		public bool Connect(IPAddress ip, int loginPort, int gamePort, string username, string password, int version)
		{
			output.WriteLine("Connecting to {0}:{1}...", ip, loginPort);
			io = new IO(ip, loginPort);

			string key = Login(username, password, version);
			if (key == null)
			{
				error.WriteLine("Failed to log in.");
				return false;
			}

			output.WriteLine("Connecting to {0}:{1}...", ip, gamePort);
			io.Dispose();
			io = new IO(ip, gamePort);
			bool joined = JoinServer(username, key, version);
			if (!joined)
			{
				error.WriteLine("Failed to join server.");
				return false;
			}

			penguins = new Dictionary<int, Penguin>();
			Thread thread = new Thread(Listen);
			thread.Start();
			return true;
		}

		public bool Connect(IPAddress ip, int loginPort, int gamePort, string username, string password)
		{
			return Connect(ip, loginPort, gamePort, username, password, 153);
		}

		public bool Connect(string ip, int loginPort, int gamePort, string username, string password, int version)
		{
			return Connect(IPAddress.Parse(ip), loginPort, gamePort, username, password, version);
		}

		public bool Connect(string ip, int loginPort, int gamePort, string username, string password)
		{
			return Connect(IPAddress.Parse(ip), loginPort, gamePort, username, password);
		}

		public void Dispose()
		{
			io.Dispose();
		}

		#endregion
	}
}
