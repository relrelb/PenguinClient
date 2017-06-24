using System;
using System.Windows.Forms;

namespace PenguinClientFlash
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			axShockwaveFlash.LoadMovie(0, Application.StartupPath + "\\loader.swf");
			axShockwaveFlash.Play();
		}

		private void axShockwaveFlash_FlashCall(object sender, AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEvent e)
		{
			InvokeRequest invoke = InvokeRequest.Parse(e.request);
			switch (invoke.Name)
			{
				case "mbox":
					string message = string.Empty;
					for (int i = 0; i < invoke.Arguments.Length; i++)
					{
						if (i > 0)
							message += " ";
						message += Util.GetLiteral(invoke.Arguments[i]);
					}
					MessageBox.Show(message);
					break;
			}
		}

		private void button_Click(object sender, EventArgs e)
		{
			Util.Init(axShockwaveFlash);
			Waddle(485456, 400, 400);
		}

		private void Waddle(int id, int x, int y)
		{
			Util.SendPacket(axShockwaveFlash, "s", "u#sp", new object[] { id, x, y }, -1);
		}
	}
}
