using System;

namespace PenguinClient
{
	public class PacketEventArgs : EventArgs
	{
		#region Fields

		private readonly Packet packet;

		private bool handled;

		#endregion

		#region Properties

		public Packet Packet { get { return packet; } }

		public bool Handled
		{
			get
			{
				return handled;
			}
			set
			{
				handled |= value;
			}
		}

		#endregion

		#region Constructors

		public PacketEventArgs(Packet packet)
		{
			this.packet = packet;
			handled = false;
		}

		#endregion
	}
}
