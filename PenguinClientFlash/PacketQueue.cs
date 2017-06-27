using PenguinClient;
using System;
using System.Collections.Generic;
using System.Threading;

namespace PenguinClientFlash
{
	internal class PacketQueue : IDisposable
	{
		#region Fields

		private Queue<Packet> queue;

		private Mutex mutex;

		#endregion

		#region Constructors

		public PacketQueue()
		{
			queue = new Queue<Packet>();
			mutex = new Mutex();
		}

		#endregion

		#region Methods

		public void Enqueue(Packet packet)
		{
			if (packet == null)
				throw new ArgumentNullException(nameof(packet));
			lock (queue)
			{
				queue.Enqueue(packet);
			}
			mutex.ReleaseMutex();
		}

		public void Enqueue(string extension, string command, params object[] array)
		{
			Enqueue(new Packet(extension, command, array));
		}

		public Packet Dequeue()
		{
			mutex.WaitOne();
			lock (queue)
			{
				return queue.Dequeue();
			}
		}

		public void Dispose()
		{
			if (mutex != null)
			{
				mutex.Dispose();
				mutex = null;
			}
		}

		#endregion
	}
}
