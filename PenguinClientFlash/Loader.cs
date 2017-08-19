using System;
using System.Windows.Forms;

namespace PenguinClientFlash
{
	public partial class Loader : Form
	{
		private FlashClient client;

		public Loader()
		{
			InitializeComponent();
			client = new FlashClient(axShockwaveFlash, Application.StartupPath + @"\loader.swf");
			client.InvokeRequest += Client_InvokeRequest;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			client.Load();
		}

		private void Client_InvokeRequest(object sender, InvokeRequestEventArgs e)
		{
			switch (e.Name)
			{
				case "mbox":
					string message = string.Empty;
					for (int i = 0; i < e.Arguments.Length; i++)
					{
						if (i > 0)
							message += " ";
						message += FlashClient.GetLiteral(e.Arguments[i]);
					}
					MessageBox.Show(message);
					break;
			}
		}

		private void button_Click(object sender, EventArgs e)
		{
			client.Walk(400, 400);
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
