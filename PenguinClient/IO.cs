using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PenguinClient
{
	internal class IO : IDisposable
	{
		private IPAddress ip;

		private int port;

		private Socket socket;

		private string buffer;

		public bool Connected { get { return socket.Connected; } }

		public IO(IPAddress ip, int port)
		{
			this.ip = ip;
			this.port = port;
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.Connect(ip, port);
			buffer = string.Empty;
		}

		public void Send(string data)
		{
			socket.Send(Encoding.UTF8.GetBytes(data + '\0'));
		}

		public string Receive()
		{
			byte[] buffer = new byte[4096];
			int i = this.buffer.IndexOf('\0');
			while (i < 0)
			{
				int length = socket.Receive(buffer);
				string received = Encoding.UTF8.GetString(buffer, 0, length);
				i = received.IndexOf('\0');
				if (i >= 0)
					i += this.buffer.Length;
				this.buffer += received;
			}
			string message = this.buffer.Substring(0, i + 1);
			this.buffer = this.buffer.Substring(i + 1);
			return message;
		}

		public void Dispose()
		{
			socket.Close();
		}
	}
}
