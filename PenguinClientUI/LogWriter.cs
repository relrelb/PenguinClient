using System.Drawing;
using System.IO;
using System.Text;

namespace PenguinClientUI
{
	public class LogWriter : TextWriter
	{
		#region Fields

		private readonly Log log;

		private bool enabled;

		private Color back;

		private Color fore;

		private Font font;

		#endregion

		#region Properties

		public Log Log { get { return log; } }

		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
			}
		}

		public Color BackColor
		{
			get
			{
				return back;
			}
			set
			{
				back = value;
			}
		}

		public Color ForeColor
		{
			get
			{
				return fore;
			}
			set
			{
				fore = value;
			}
		}

		public Font Font
		{
			get
			{
				return font;
			}

			set
			{
				font = value;
			}
		}

		public override Encoding Encoding { get { return Encoding.Default; } }

		#endregion

		#region Constructors

		public LogWriter(Log log, bool enabled)
		{
			this.log = log;
			this.enabled = enabled;
		}

		public LogWriter(Log log) : this(log, true) { }

		#endregion

		#region Methods

		public override void WriteLine(string value)
		{
			if (enabled)
				log.WriteLine(value, back, fore, font);
		}

		#endregion
	}
}
