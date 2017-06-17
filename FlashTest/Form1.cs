using System;
using System.Windows.Forms;

namespace FlashTest
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
					MessageBox.Show((string)invoke.Arguments[0]);
					break;
			}
		}

		private void button_Click(object sender, EventArgs e)
		{
			Waddle(1841, 400, 400);
		}

		private void Waddle(int id, int x, int y)
		{
			object obj = Util.SendPacket(axShockwaveFlash, "s", "u#sp", new object[] { id, x, y }, -1);
			MessageBox.Show(Util.GetString(obj));
		}
	}
}
