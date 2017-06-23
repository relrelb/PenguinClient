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

		private int coins;

		private int room;

		private Timer heartbeat;

		private Dictionary<int, Penguin> penguins;

		#endregion

		#region Properties

		public TextWriter Output { get { return output; } }

		public TextWriter Error { get { return error; } }

		public int InternalRoomId { get { return internalRoomId; } }

		public int PenguinId { get { return id; } }

		public int Coins { get { return coins; } }

		public int RoomId { get { return room; } }

		public bool Connected { get { return io.Connected; } }

		#endregion

		#region Constructors

		public Client(TextWriter output, TextWriter error)
		{
			this.output = output;
			this.error = error;
			internalRoomId = -1;
			id = -1;
			coins = -1;
			room = -1;
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
			bool sent = io.Send(string.Format("<msg t=\"sys\"><body action=\"verChk\" r=\"0\"><ver v=\"{0}\"/></body></msg>", version));
			if (!sent)
				return false;
			string response = io.Receive();
			if (response == null)
				return false;
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
			bool sent = io.Send("<msg t=\"sys\"><body action=\"rndK\" r=\"-1\"></body></msg>");
			if (!sent)
				return null;
			string response = io.Receive();
			if (response == null)
				return null;
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

		private bool SendPacket(string extension, string opcode, params object[] args)
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat("%xt%{0}%{1}%", extension, opcode);
			foreach (object arg in args)
			{
				builder.AppendFormat("{0}%", arg);
			}
			return io.Send(builder.ToString());
		}

		private string[] ReceivePacket(bool error)
		{
			string data = io.Receive();
			if (data == null)
				return null;
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
			bool sent = io.Send(string.Format("<msg t=\"sys\"><body action=\"login\" r=\"0\"><login z=\"w1\"><nick><![CDATA[{0}]]></nick><pword><![CDATA[{1}]]></pword></login></body></msg>", username, hash));
			if (!sent)
				return null;
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
			bool sent = io.Send(string.Format("<msg t=\"sys\"><body action=\"login\" r=\"0\"><login z=\"w1\"><nick><![CDATA[{0}]]></nick><pword><![CDATA[{1}]]></pword></login></body></msg>", username, hash));
			if (!sent)
				return false;
			string[] packet;
			do
			{
				packet = ReceivePacket();
				if (packet == null)
					return false;
			} while (packet[2] != "l");
			sent = SendPacket("s", "j#js", internalRoomId, id, loginKey, "en");
			if (!sent)
				return false;
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
			heartbeat = new Timer(Heartbeat, null, 600000, 600000);
			output.WriteLine("Listening to packets...");
			while (Connected)
			{
				string[] packet = ReceivePacket();
				if (packet != null)
					HandlePacket(packet);
			}
		}

		private void Heartbeat(object state)
		{
			SendPacket("s", "u#h", internalRoomId);
		}

		private void HandlePacket(string[] packet)
		{
			output.WriteLine(string.Join("%", packet));
			string opcode = packet[2];
			int id;
			Penguin penguin;
			switch (opcode)
			{
				case "e":
					break;
				case "h":
					break;
				case "lp":
					penguin = Penguin.FromPlayer(packet[4]);
					penguins.Add(penguin.Id, penguin);
					this.coins = int.Parse(packet[5]);
					bool safe = packet[6] == "1";
					//int eggTimer = int.Parse(packet[7]);
					long loginTime = long.Parse(packet[8]);
					int age = int.Parse(packet[9]);
					//int bannedAge = int.Parse(packet[10]);
					int playTime = int.Parse(packet[11]);
					int memberLeft = packet[12].Length > 0 ? int.Parse(packet[12]) : 0;
					int timezone = int.Parse(packet[13]);
					//bool openedPlaycard = packet[14] == "1";
					//int savedMapCategory = int.Parse(packet[15]);
					//int statusField = int.Parse(packet[16]);
					break;
				case "ap":
					penguin = Penguin.FromPlayer(packet[4]);
					penguins.Add(penguin.Id, penguin);
					break;
				case "jr":
					internalRoomId = int.Parse(packet[3]);
					penguins = new Dictionary<int, Penguin>();
					room = int.Parse(packet[4]);
					for (int i = 5; i < packet.Length; i++)
					{
						penguin = Penguin.FromPlayer(packet[i]);
						penguins.Add(penguin.Id, penguin);
					}
					break;
				case "rp":
					id = int.Parse(packet[4]);
					penguin = penguins[id];
					penguins.Remove(id);
					break;
				case "upc":
					id = int.Parse(packet[4]);
					penguin = penguins[id];
					int color = int.Parse(packet[5]);
					penguin.Color = color;
					break;
				case "uph":
					id = int.Parse(packet[4]);
					penguin = penguins[id];
					int head = int.Parse(packet[5]);
					penguin.Head = head;
					break;
				case "upf":
					id = int.Parse(packet[4]);
					penguin = penguins[id];
					int face = int.Parse(packet[5]);
					penguin.Face = face;
					break;
				case "upn":
					id = int.Parse(packet[4]);
					penguin = penguins[id];
					int neck = int.Parse(packet[5]);
					penguin.Neck = neck;
					break;
				case "upb":
					id = int.Parse(packet[4]);
					penguin = penguins[id];
					int body = int.Parse(packet[5]);
					penguin.Body = body;
					break;
				case "upa":
					id = int.Parse(packet[4]);
					penguin = penguins[id];
					int hand = int.Parse(packet[5]);
					penguin.Hand = hand;
					break;
				case "upe":
					id = int.Parse(packet[4]);
					penguin = penguins[id];
					int feet = int.Parse(packet[5]);
					penguin.Feet = feet;
					break;
				case "upl":
					id = int.Parse(packet[4]);
					penguin = penguins[id];
					int pin = int.Parse(packet[5]);
					penguin.Pin = pin;
					break;
				case "upp":
					id = int.Parse(packet[4]);
					penguin = penguins[id];
					int background = int.Parse(packet[5]);
					penguin.Background = background;
					break;
				case "sp":
					id = int.Parse(packet[4]);
					penguin = penguins[id];
					penguin.X = int.Parse(packet[5]);
					penguin.Y = int.Parse(packet[6]);
					break;
				case "ai":
					id = int.Parse(packet[4]);
					int coins = int.Parse(packet[5]);
					int cost = this.coins - coins;
					output.WriteLine("Added item {0} (cost {1} coins)", id, cost);
					break;
				default:
					error.WriteLine("Unknown opcode: {0}", opcode);
					break;
			}
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

		public void GoToRoom(int id, int x, int y)
		{
			output.WriteLine("Going to room {0}...", id);
			SendPacket("s", "j#jr", internalRoomId, id, x, y);
		}

		public void GoToRoom(int id)
		{
			GoToRoom(id, 0, 0);
		}

		public void UpdateColor(int id)
		{
			output.WriteLine("Changing color to {0}...", id);
			SendPacket("s", "s#upc", internalRoomId, id);
		}

		public void UpdateHead(int id)
		{
			output.WriteLine("Changing head item to {0}...", id);
			SendPacket("s", "s#uph", internalRoomId, id);
		}

		public void UpdateFace(int id)
		{
			output.WriteLine("Changing face item to {0}...", id);
			SendPacket("s", "s#upf", internalRoomId, id);
		}

		public void UpdateNeck(int id)
		{
			output.WriteLine("Changing neck item to {0}...", id);
			SendPacket("s", "s#upn", internalRoomId, id);
		}

		public void UpdateBody(int id)
		{
			output.WriteLine("Changing body item to {0}...", id);
			SendPacket("s", "s#upb", internalRoomId, id);
		}

		public void UpdateHand(int id)
		{
			output.WriteLine("Changing hand item to {0}...", id);
			SendPacket("s", "s#upa", internalRoomId, id);
		}

		public void UpdateFeet(int id)
		{
			output.WriteLine("Changing feet item to {0}...", id);
			SendPacket("s", "s#upe", internalRoomId, id);
		}

		public void UpdatePin(int id)
		{
			output.WriteLine("Changing pin to {0}...", id);
			SendPacket("s", "s#upl", internalRoomId, id);
		}

		public void UpdateBackground(int id)
		{
			output.WriteLine("Changing background to {0}...", id);
			SendPacket("s", "s#upp", internalRoomId, id);
		}

		public void Walk(int x, int y)
		{
			output.WriteLine("Walking to ({0}, {1})...", x, y);
			SendPacket("s", "u#sp", id, x, y);
		}

		private void Action(int id)
		{
			SendPacket("s", "u#sa", internalRoomId, id);
		}

		private void Frame(int id)
		{
			SendPacket("s", "u#sf", internalRoomId, id);
		}

		public void Dance()
		{
			output.WriteLine("Dancing...");
			Frame(26);
		}

		public void Wave()
		{
			output.WriteLine("Waving...");
			Action(25);
		}

		public void Sit(SitDirection direction)
		{
			output.WriteLine("Sitting...");
			Frame((int)direction);
		}

		public void Sit()
		{
			Sit(SitDirection.South);
		}

		public void Snowball(int x, int y)
		{
			output.WriteLine("Throwing snowball to ({0}, {1})...", x, y);
			SendPacket("s", "u#sb", internalRoomId, x, y);
		}

		public void Say(string message)
		{
			output.WriteLine("Saying '{0}'...", message);
			SendPacket("s", "m#sm", internalRoomId, id, message);
		}

		public void SaySafe(int id)
		{
			output.WriteLine("Saying {0}...", id);
			SendPacket("s", "u#ss", internalRoomId, id);
		}

		public void Joke(int id)
		{
			output.WriteLine("Saying joke {0}...", id);
			SendPacket("s", "u#sj", this.id, id);
		}

		public void Emote(int id)
		{
			output.WriteLine("Saying emote {0}...", id);
			SendPacket("s", "u#se", internalRoomId, id);
		}

		public void AddItem(int id)
		{
			output.WriteLine("Adding item {0}...", id);
			SendPacket("s", "i#ai", internalRoomId, id);
		}

		public void Logout()
		{
			output.WriteLine("Logging out...");
			Dispose();
		}

		public void Dispose()
		{
			heartbeat.Dispose();
			io.Dispose();
		}

		#endregion
	}
}
