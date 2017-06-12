using System.Drawing;
using System.IO;
using System.Text;

namespace PenguinClientUI
{
	public class LogWriter : TextWriter
	{
		#region Fields

		private Log log;

		private Color back;

		private Color fore;

		private Font font;

		#endregion

		#region Properties

		public Log Log { get { return log; } }

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

		public LogWriter(Log log)
		{
			this.log = log;
		}

		#endregion

		#region Methods

		public override void WriteLine(string value)
		{
			log.WriteLine(value, back, fore, font);
		}

		#endregion
	}
}
