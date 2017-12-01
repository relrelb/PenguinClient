using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace PenguinClient
{
	public abstract class Client : IDisposable
	{
		#region Fields

		private TextWriter info;

		private TextWriter error;

		private TextWriter output;

		private TextWriter input;

		private int internalRoomId;

		private int id;

		private int coins;

		private int room;

		private Timer heartbeat;

		private Dictionary<int, Penguin> penguins;

		#endregion

		#region Events

		public event EventHandler<PacketEventArgs> Packet;

		#endregion

		#region Properties

		public TextWriter Info
		{
			get
			{
				return info;
			}
			set
			{
				info = value;
			}
		}

		public TextWriter Error
		{
			get
			{
				return error;
			}
			set
			{
				error = value;
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

		public int InternalRoomId
		{
			get
			{
				return internalRoomId;
			}
			protected set
			{
				internalRoomId = value;
			}
		}

		public int PenguinId
		{
			get
			{
				return id;
			}
			protected set
			{
				id = value;
			}
		}

		public int Coins
		{
			get
			{
				return coins;
			}
			protected set
			{
				coins = value;
			}
		}

		public int RoomId
		{
			get
			{
				return room;
			}
			protected set
			{
				room = value;
			}
		}

		public abstract bool Connected { get; }

		#endregion

		#region Constructors

		protected Client(TextWriter info, TextWriter error, TextWriter output, TextWriter input)
		{
			this.info = info;
			this.error = error;
			this.output = output;
			this.input = input;
			internalRoomId = -1;
			id = -1;
			coins = -1;
			room = -1;
		}

		#endregion

		#region Methods

		protected abstract bool SendPacket(Packet packet);

		protected bool SendPacket(string extension, string command, params object[] array)
		{
			return SendPacket(new Packet(extension, command, array));
		}

		protected abstract Packet ReceivePacket();

		protected Packet ReceivePacket(bool error)
		{
			Packet packet = ReceivePacket();
			if (error && packet.Command == "e")
			{
				HandleError(packet);
				return null;
			}
			return packet;
		}

		private void HandleError(Packet packet)
		{
			string code = packet.Array[1];
			string message = GetErrorMessage(code);
			if (message == null)
				error.WriteLine("Error #{0}", code);
			else
				error.WriteLine("Error #{0}: {1}", code, message);
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

		protected void Listen()
		{
			heartbeat = new Timer(state =>
			{
				SendPacket("s", "u#h", internalRoomId);
			}, null, 600000, 600000);
			OnPacket("h", packet => { }, false);
			OnPacket("lp", packet =>
			{
				Penguin penguin = Penguin.FromPlayer(packet.Array[1]);
				Coins = int.Parse(packet.Array[2]);
				bool safe = packet.Array[3] == "1";
				//int eggTimer = int.Parse(packet.Array[4]);
				long loginTime = long.Parse(packet.Array[5]);
				int age = int.Parse(packet.Array[6]);
				//int bannedAge = int.Parse(packet.Array[7]);
				int playTime = int.Parse(packet.Array[8]);
				int memberLeft = packet.Array[9].Length > 0 ? int.Parse(packet.Array[9]) : 0;
				int timezone = int.Parse(packet.Array[10]);
				//bool openedPlaycard = packet.Array[11] == "1";
				//int savedMapCategory = int.Parse(packet.Array[12]);
				//int statusField = int.Parse(packet.Array[13]);
			}, false);
			OnPacket("ap", packet =>
			{
				Penguin penguin = Penguin.FromPlayer(packet.Array[1]);
				penguins[penguin.Id] = penguin;
			}, false);
			OnPacket("jr", packet =>
			{
				internalRoomId = int.Parse(packet.Array[0]);
				penguins = new Dictionary<int, Penguin>();
				room = int.Parse(packet.Array[1]);
				for (int i = 2; i < packet.Array.Length; i++)
				{
					Penguin penguin = Penguin.FromPlayer(packet.Array[i]);
					if (!penguins.ContainsKey(penguin.Id))
						penguins.Add(penguin.Id, penguin);
				}
			}, false);
			OnPacket("rp", packet =>
			{
				int id = int.Parse(packet.Array[1]);
				Penguin penguin = penguins[id];
				penguins.Remove(id);
			}, false);
			OnPacket("upc", packet =>
			{
				int id = int.Parse(packet.Array[1]);
				Penguin penguin = penguins[id];
				int color = int.Parse(packet.Array[2]);
				penguin.Color = color;
			}, false);
			OnPacket("uph", packet =>
			{
				int id = int.Parse(packet.Array[1]);
				Penguin penguin = penguins[id];
				int head = int.Parse(packet.Array[2]);
				penguin.Head = head;
			}, false);
			OnPacket("upf", packet =>
			{
				int id = int.Parse(packet.Array[1]);
				Penguin penguin = penguins[id];
				int face = int.Parse(packet.Array[2]);
				penguin.Face = face;
			}, false);
			OnPacket("upn", packet =>
			{
				int id = int.Parse(packet.Array[1]);
				Penguin penguin = penguins[id];
				int neck = int.Parse(packet.Array[2]);
				penguin.Neck = neck;
			}, false);
			OnPacket("upb", packet =>
			{
				int id = int.Parse(packet.Array[1]);
				Penguin penguin = penguins[id];
				int body = int.Parse(packet.Array[2]);
				penguin.Body = body;
			}, false);
			OnPacket("upa", packet =>
			{
				int id = int.Parse(packet.Array[1]);
				Penguin penguin = penguins[id];
				int hand = int.Parse(packet.Array[2]);
				penguin.Hand = hand;
			}, false);
			OnPacket("upe", packet =>
			{
				int id = int.Parse(packet.Array[1]);
				Penguin penguin = penguins[id];
				int feet = int.Parse(packet.Array[2]);
				penguin.Feet = feet;
			}, false);
			OnPacket("upl", packet =>
			{
				int id = int.Parse(packet.Array[1]);
				Penguin penguin = penguins[id];
				int pin = int.Parse(packet.Array[2]);
				penguin.Pin = pin;
			}, false);
			OnPacket("upp", packet =>
			{
				int id = int.Parse(packet.Array[1]);
				Penguin penguin = penguins[id];
				int background = int.Parse(packet.Array[2]);
				penguin.Background = background;
			}, false);
			OnPacket("sp", packet =>
			{
				int id = int.Parse(packet.Array[1]);
				Penguin penguin = penguins[id];
				penguin.X = int.Parse(packet.Array[2]);
				penguin.Y = int.Parse(packet.Array[3]);
			}, false);
			OnPacket("ai", packet =>
			{
				int id = int.Parse(packet.Array[1]);
				int coins = int.Parse(packet.Array[2]);
				int cost = this.coins - coins;
				info.WriteLine("Added item {0} (cost {1} coins)", id, cost);
			}, false);
			info.WriteLine("Listening to packets...");
			while (Connected)
			{
				Packet packet = ReceivePacket(true);
				if (packet != null)
				{
					PacketEventArgs e = new PacketEventArgs(packet);
					Packet(this, e);
					if (!e.Handled)
						error.WriteLine("Unhadled packet: {0}", packet);
				}
			}
		}

		public void OnPacket(string command, Action<Packet> action, bool once)
		{
			EventHandler<PacketEventArgs> handler = null;
			handler = (sender, e) =>
			{
				if (e.Packet.Command == command)
				{
					action(e.Packet);
					e.Handled = true;
					if (once)
						Packet -= handler;
				}
			};
			Packet += handler;
		}

		public void OnPacket(string command, Action<Packet> action)
		{
			OnPacket(command, action, true);
		}

		public Packet OnPacket(string command)
		{
			Packet packet = null;
			AutoResetEvent handle = new AutoResetEvent(false);
			OnPacket(command, p =>
			{
				packet = p;
				handle.Set();
			});
			handle.WaitOne();
			handle.Dispose();
			handle = null;
			return packet;
		}

		public void JoinRoom(int id, int x, int y)
		{
			info.WriteLine("Joining room {0}...", id);
			SendPacket("s", "j#jr", internalRoomId, id, x, y);
		}

		public void JoinRoom(int id)
		{
			JoinRoom(id, 0, 0);
		}

		public void JoinIgloo(int id)
		{
			info.WriteLine("Joining {0}'s igloo...", id);
			SendPacket("s", "j#jp", this.id, id + 1000);
		}

		public void JoinIgloo()
		{
			JoinIgloo(id);
		}

		public void UpdateColor(int id)
		{
			info.WriteLine("Changing color to {0}...", id);
			SendPacket("s", "s#upc", internalRoomId, id);
		}

		public void UpdateHead(int id)
		{
			info.WriteLine("Changing head item to {0}...", id);
			SendPacket("s", "s#uph", internalRoomId, id);
		}

		public void UpdateFace(int id)
		{
			info.WriteLine("Changing face item to {0}...", id);
			SendPacket("s", "s#upf", internalRoomId, id);
		}

		public void UpdateNeck(int id)
		{
			info.WriteLine("Changing neck item to {0}...", id);
			SendPacket("s", "s#upn", internalRoomId, id);
		}

		public void UpdateBody(int id)
		{
			info.WriteLine("Changing body item to {0}...", id);
			SendPacket("s", "s#upb", internalRoomId, id);
		}

		public void UpdateHand(int id)
		{
			info.WriteLine("Changing hand item to {0}...", id);
			SendPacket("s", "s#upa", internalRoomId, id);
		}

		public void UpdateFeet(int id)
		{
			info.WriteLine("Changing feet item to {0}...", id);
			SendPacket("s", "s#upe", internalRoomId, id);
		}

		public void UpdatePin(int id)
		{
			info.WriteLine("Changing pin to {0}...", id);
			SendPacket("s", "s#upl", internalRoomId, id);
		}

		public void UpdateBackground(int id)
		{
			info.WriteLine("Changing background to {0}...", id);
			SendPacket("s", "s#upp", internalRoomId, id);
		}

		public void Walk(int x, int y)
		{
			info.WriteLine("Walking to ({0}, {1})...", x, y);
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
			info.WriteLine("Dancing...");
			Frame(26);
		}

		public void Wave()
		{
			info.WriteLine("Waving...");
			Action(25);
		}

		public void Sit(SitDirection direction)
		{
			info.WriteLine("Sitting...");
			Frame((int)direction);
		}

		public void Sit()
		{
			Sit(SitDirection.South);
		}

		public void Snowball(int x, int y)
		{
			info.WriteLine("Throwing snowball to ({0}, {1})...", x, y);
			SendPacket("s", "u#sb", internalRoomId, x, y);
		}

		public void Say(string message)
		{
			info.WriteLine("Saying '{0}'...", message);
			SendPacket("s", "m#sm", internalRoomId, id, message);
		}

		public void SaySafe(int id)
		{
			info.WriteLine("Saying {0}...", id);
			SendPacket("s", "u#ss", internalRoomId, id);
		}

		public void Joke(int id)
		{
			info.WriteLine("Saying joke {0}...", id);
			SendPacket("s", "u#sj", this.id, id);
		}

		public void Emote(int id)
		{
			info.WriteLine("Saying emote {0}...", id);
			SendPacket("s", "u#se", internalRoomId, id);
		}

		public void AddItem(int id)
		{
			info.WriteLine("Adding item {0}...", id);
			SendPacket("s", "i#ai", internalRoomId, id);
		}

		public void AddCoins(int coins)
		{
			info.WriteLine("Adding {0} coins", coins);
			int room = this.room;
			JoinRoom(912);
			SendPacket("z", "zo", coins);
			JoinRoom(room);
		}

		public void Logout()
		{
			info.WriteLine("Logging out...");
			Dispose();
		}

		public virtual void Dispose()
		{
			if (heartbeat != null)
			{
				heartbeat.Dispose();
				heartbeat = null;
			}
		}

		#endregion
	}
}
