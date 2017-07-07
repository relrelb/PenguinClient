using System;
using System.Drawing;
using System.Windows.Forms;
using PenguinClient;

namespace PenguinClientUI
{
	public partial class Form1 : Form
	{
		private Client client;

		public Form1()
		{
			InitializeComponent();
			LogWriter output = new LogWriter(log);
			LogWriter error = new LogWriter(log);
			error.ForeColor = Color.Red;
			client = new Client(output, error);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			bool connected = client.Connect("158.69.214.194", 6112, 6115, "nagitest", "test");
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			if (client != null)
			{
				client.Dispose();
				client = null;
			}
		}
	}
}
