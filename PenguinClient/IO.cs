using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PenguinClient
{
	internal class IO : IDisposable
	{
		#region Fields

		private readonly IPAddress ip;

		private readonly int port;

		private Socket socket;

		private string buffer;

		private TextWriter output;

		private TextWriter input;

		#endregion

		#region Properties

		public IPAddress IpAddress { get { return ip; } }

		public int Port { get { return port; } }

		public bool Connected
		{
			get
			{
				if (socket == null)
					return false;
				return socket.Connected;
			}
		}

		public TextWriter Output
		{
			get
			{
				return output;
			}
			set
			{
				output = value;
			}
		}

		public TextWriter Input
		{
			get
			{
				return input;
			}
			set
			{
				input = value;
			}
		}

		#endregion

		#region Constructors

		public IO(IPAddress ip, int port, TextWriter output, TextWriter input)
		{
			this.ip = ip;
			this.port = port;
			this.output = output;
			this.input = input;
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.Connect(ip, port);
			buffer = string.Empty;
		}

		public IO(IPAddress ip, int port) : this(ip, port, TextWriter.Null, TextWriter.Null) { }

		#endregion

		#region Methods

		public bool Send(string data)
		{
			output.WriteLine(data);
			byte[] buffer = Encoding.UTF8.GetBytes(data + '\0');
			try
			{
				socket.Send(buffer);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public string Receive()
		{
			byte[] buffer = new byte[4096];
			int i = this.buffer.IndexOf('\0');
			while (i < 0)
			{
				int length;
				try
				{
					length = socket.Receive(buffer);
				}
				catch
				{
					return null;
				}
				string received = Encoding.UTF8.GetString(buffer, 0, length);
				i = received.IndexOf('\0');
				if (i >= 0)
					i += this.buffer.Length;
				this.buffer += received;
			}
			string data = this.buffer.Substring(0, i + 1);
			this.buffer = this.buffer.Substring(i + 1);
			input.WriteLine(data);
			return data;
		}

		public void Dispose()
		{
			if (socket != null)
			{
				socket.Shutdown(SocketShutdown.Both);
				socket.Close();
				socket = null;
			}
		}

		#endregion
	}
}
