using System;
using System.Windows.Forms;

namespace PenguinClientFlash
{
	public partial class Form1 : Form
	{
		private FlashClient client;

		public Form1()
		{
			InitializeComponent();
			client = new FlashClient(axShockwaveFlash, Application.StartupPath + @"\loader.swf");
			client.InvokeRequest += Loader_InvokeRequest;
			client.Load();
		}

		private void Loader_InvokeRequest(object sender, InvokeRequestEventArgs e)
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
	}
}
