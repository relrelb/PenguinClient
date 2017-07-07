namespace PenguinClient
{
	public class Penguin
	{
		#region Fields

		private readonly int id;

		private readonly string name;

		private int color;

		private int head;

		private int face;

		private int neck;

		private int body;

		private int hand;

		private int feet;

		private int pin;

		private int background;

		private int x;

		private int y;

		#endregion

		#region Properties

		public int Id { get { return id; } }

		public string Name { get { return name; } }

		public int Color
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
			}
		}

		public int Head
		{
			get
			{
				return head;
			}
			set
			{
				head = value;
			}
		}

		public int Face
		{
			get
			{
				return face;
			}
			set
			{
				face = value;
			}
		}

		public int Neck
		{
			get
			{
				return neck;
			}
			set
			{
				neck = value;
			}
		}

		public int Body
		{
			get
			{
				return body;
			}
			set
			{
				body = value;
			}
		}

		public int Hand
		{
			get
			{
				return hand;
			}
			set
			{
				hand = value;
			}
		}

		public int Feet
		{
			get
			{
				return feet;
			}
			set
			{
				feet = value;
			}
		}

		public int Pin
		{
			get
			{
				return pin;
			}
			set
			{
				pin = value;
			}
		}

		public int Background
		{
			get
			{
				return background;
			}
			set
			{
				background = value;
			}
		}

		public int X
		{
			get
			{
				return x;
			}
			set
			{
				x = value;
			}
		}

		public int Y
		{
			get
			{
				return y;
			}
			set
			{
				y = value;
			}
		}

		#endregion

		#region Constructors

		private Penguin(int id, string name, int color, int head, int face, int neck, int body, int hand, int feet, int pin, int background, int x, int y)
		{
			this.id = id;
			this.name = name;
			this.color = color;
			this.head = head;
			this.face = face;
			this.neck = neck;
			this.body = body;
			this.hand = hand;
			this.feet = feet;
			this.pin = pin;
			this.background = background;
			this.x = x;
			this.y = y;
		}

		internal static Penguin FromPlayer(string player)
		{
			string[] parts = player.Split('|');
			int id = int.Parse(parts[0]);
			string name = parts[1];
			// ??? = int.Parse(parts[2]);
			int color = int.Parse(parts[3]);
			int head = int.Parse(parts[4]);
			int face = int.Parse(parts[5]);
			int neck = int.Parse(parts[6]);
			int body = int.Parse(parts[7]);
			int hand = int.Parse(parts[8]);
			int feet = int.Parse(parts[9]);
			int pin = int.Parse(parts[10]);
			int background = int.Parse(parts[11]);
			int x = int.Parse(parts[12]);
			int y = int.Parse(parts[13]);
			return new Penguin(id, name, color, head, face, neck, body, hand, feet, pin, background, x, y);
		}

		#endregion
	}
}
