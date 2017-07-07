using System;
using System.Drawing;
using System.Windows.Forms;

namespace PenguinClientUI
{
	public partial class Log : UserControl
	{
		public Log()
		{
			InitializeComponent();
		}

		public void WriteLine(string value, Color back, Color fore, Font font)
		{
			Label label = new Label();
			label.BackColor = back;
			label.ForeColor = fore;
			label.Font = font;
			label.AutoSize = true;
			label.Text = value;
			if (lines.InvokeRequired)
				lines.Invoke(new Action<string, Color, Color, Font>(WriteLine), value, back, fore, font);
			else
				lines.Controls.Add(label);
		}

		public void Clear()
		{
			lines.Controls.Clear();
		}
	}
}
