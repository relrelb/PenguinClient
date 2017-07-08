using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace PenguinClient
{
	public class Bot : Client, IDisposable
	{
		#region Fields

		private IO io;

		#endregion

		#region Properties

		public override bool Connected
		{
			get
			{
				if (io == null)
					return false;
				return io.Connected;
			}
		}

		#endregion

		#region Constructors

		public Bot(TextWriter info, TextWriter error, TextWriter output, TextWriter input) : base(info, error, output, input) { }

		public Bot(TextWriter info, TextWriter error) : this(info, error, TextWriter.Null, TextWriter.Null) { }

		public Bot(TextWriter info) : this(info, TextWriter.Null) { }

		public Bot() : this(TextWriter.Null) { }

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

		private bool Send(string data)
		{
			Output.WriteLine(data);
			return io.Send(data);
		}

		private string Receive()
		{
			string data = io.Receive();
			Input.WriteLine(data);
			return data;
		}

		private bool VersionCheck(int version)
		{
			Info.WriteLine("Sending \"verChk\" request...");
			bool sent = Send(string.Format("<msg t=\"sys\"><body action=\"verChk\" r=\"0\"><ver v=\"{0}\"/></body></msg>", version));
			if (!sent)
				return false;
			string response = Receive();
			if (response == null)
				return false;
			if (response.Contains("apiOK"))
			{
				Info.WriteLine("Received \"apiOK\" response");
				return true;
			}
			if (response.Contains("apiKO"))
			{
				Info.WriteLine("Received \"apiKO\" response");
				return false;
			}
			Error.WriteLine("Received invalid response");
			return false;
		}

		private string ReceiveKey()
		{
			Info.WriteLine("Sending \"rndK\" request...");
			bool sent = Send("<msg t=\"sys\"><body action=\"rndK\" r=\"-1\"></body></msg>");
			if (!sent)
				return null;
			string response = Receive();
			if (response == null)
				return null;
			if (response.Contains("rndK"))
			{
				Regex regex = new Regex(@"<k>(<!\[CDATA\[)?(.*?)(\]\]>)?<\/k>");
				if (regex.IsMatch(response))
				{
					string key = regex.Match(response).Groups[2].Value;
					Info.WriteLine("Received key: {0}", key);
					return key;
				}
			}
			Error.WriteLine("Received invalid response");
			return null;
		}

		protected override bool SendPacket(Packet packet)
		{
			return Send(packet.ToString());
		}

		protected override Packet ReceivePacket()
		{
			string data = Receive();
			if (data == null)
				return null;
			Packet packet;
			try
			{
				packet = PenguinClient.Packet.Parse(data, false);
			}
			catch (Exception e)
			{
				Error.WriteLine("Error: {0}", e.Message);
				return null;
			}
			return packet;
		}

		private string Login(string username, string password, int version)
		{
			Info.WriteLine("Logging in...");
			bool ok = VersionCheck(version);
			if (!ok)
				return null;
			string key = ReceiveKey();
			if (key == null)
				return null;
			string hash = SwappedMD5(SwappedMD5(password).ToUpper() + key + "Y(02.>'H}t\":E1");
			bool sent = Send(string.Format("<msg t=\"sys\"><body action=\"login\" r=\"0\"><login z=\"w1\"><nick><![CDATA[{0}]]></nick><pword><![CDATA[{1}]]></pword></login></body></msg>", username, hash));
			if (!sent)
				return null;
			Packet packet;
			do
			{
				packet = ReceivePacket(true);
				if (packet == null)
					return null;
			} while (packet.Command != "l");
			PenguinId = int.Parse(packet.Array[1]);
			key = packet.Array[2];
			Info.WriteLine("Logged in");
			return key;
		}

		private bool JoinServer(string username, string loginKey, int version)
		{
			Info.WriteLine("Joining server...");
			bool ok = VersionCheck(version);
			if (!ok)
				return false;
			string key = ReceiveKey();
			if (key == null)
				return false;
			string hash = SwappedMD5(loginKey + key) + loginKey;
			bool sent = Send(string.Format("<msg t=\"sys\"><body action=\"login\" r=\"0\"><login z=\"w1\"><nick><![CDATA[{0}]]></nick><pword><![CDATA[{1}]]></pword></login></body></msg>", username, hash));
			if (!sent)
				return false;
			Packet packet;
			do
			{
				packet = ReceivePacket(true);
				if (packet == null)
					return false;
			} while (packet.Command != "l");
			sent = SendPacket("s", "j#js", InternalRoomId, PenguinId, loginKey, "en");
			if (!sent)
				return false;
			do
			{
				packet = ReceivePacket(true);
				if (packet == null)
					return false;
			} while (packet.Command != "js");
			Info.WriteLine("Joined server");
			return true;
		}

		public bool Connect(IPAddress ip, int loginPort, int gamePort, string username, string password, int version)
		{
			Info.WriteLine("Connecting to {0}:{1}...", ip, loginPort);
			io = new IO(ip, loginPort);

			string key = Login(username, password, version);
			if (key == null)
			{
				Error.WriteLine("Failed to log in");
				return false;
			}

			Info.WriteLine("Connecting to {0}:{1}...", ip, gamePort);
			io.Dispose();
			io = new IO(ip, gamePort);

			bool joined = JoinServer(username, key, version);
			if (!joined)
			{
				Error.WriteLine("Failed to join server");
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

		public override void Dispose()
		{
			base.Dispose();
			if (io != null)
			{
				io.Dispose();
				io = null;
			}
		}

		#endregion
	}
}
