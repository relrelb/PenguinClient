using System;

namespace PenguinClient
{
	public class PacketEventArgs : EventArgs
	{
		private readonly Packet packet;

		public Packet Packet { get { return packet; } }

		public PacketEventArgs(Packet packet)
		{
			this.packet = packet;
		}
	}
}
