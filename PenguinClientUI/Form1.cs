using PenguinClient;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace PenguinClientUI
{
	public partial class Form1 : Form
	{
		private Client client;

		public Form1()
		{
			InitializeComponent();
			LogWriter info = new LogWriter(log);
			LogWriter error = new LogWriter(log);
			error.ForeColor = Color.Red;
			LogWriter output = new LogWriter(log);
			output.ForeColor = Color.Green;
			LogWriter input = new LogWriter(log);
			input.ForeColor = Color.Blue;
			client = new Client(info, error, output, input);
			Thread thread = new Thread(() =>
			{
				bool connected = client.Connect("158.69.214.194", 6112, 6115, "nagitest", "test");
			});
			thread.Start();
		}

		private void info_CheckedChanged(object sender, EventArgs e)
		{
			CheckBox info = (CheckBox)sender;
			((LogWriter)client.Info).Enabled = info.Checked;
		}

		private void error_CheckedChanged(object sender, EventArgs e)
		{
			CheckBox error = (CheckBox)sender;
			((LogWriter)client.Error).Enabled = error.Checked;
		}

		private void send_CheckedChanged(object sender, EventArgs e)
		{
			CheckBox send = (CheckBox)sender;
			((LogWriter)client.Output).Enabled = send.Checked;
		}

		private void receive_CheckedChanged(object sender, EventArgs e)
		{
			CheckBox receive = (CheckBox)sender;
			((LogWriter)client.Input).Enabled = receive.Checked;
		}

		private void clear_Click(object sender, EventArgs e)
		{
			log.Clear();
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
