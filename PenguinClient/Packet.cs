using System;
using System.Text;

namespace PenguinClient
{
	public class Packet
	{
		#region Fields

		private string extension;

		private string command;

		private string[] array;

		#endregion

		#region Properties

		public string Extension { get { return extension; } }

		public string Command { get { return command; } }

		public string[] Array { get { return array; } }

		#endregion

		#region Constructors

		public Packet(string extension, string command, params object[] array)
		{
			this.extension = extension;
			this.command = command;
			int length = array.Length;
			this.array = new string[length];
			for (int i = 0; i < length; i++)
			{
				this.array[i] = array[i].ToString();
			}
		}

		public Packet(string command, params object[] array) : this(null, command, array) { }

		#endregion

		#region Methods

		public static Packet Parse(string data, bool extension)
		{
			if (data[0] == '%')
			{
				string[] packet = data.Split('%');
				if (extension)
				{
					object[] array = new object[packet.Length - 5];
					System.Array.Copy(packet, 4, array, 0, array.Length);
					return new Packet(packet[2], packet[3], array);
				}
				else
				{
					object[] array = new object[packet.Length - 4];
					System.Array.Copy(packet, 3, array, 0, array.Length);
					return new Packet(packet[2], array);
				}
			}
			throw new FormatException("Invalid packet");
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat("%xt%{0}%{1}%", extension, command);
			foreach (string str in array)
			{
				builder.AppendFormat("{0}%", str);
			}
			return builder.ToString();
		}

		#endregion
	}
}
