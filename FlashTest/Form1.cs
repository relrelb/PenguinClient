using System.Windows.Forms;

namespace FlashTest
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			axShockwaveFlash1.LoadMovie(0, "https://play.clubpenguinrewritten.pw/media/loader.swf");
			axShockwaveFlash1.Play();

			//axShockwaveFlash1.CallFunction("<invoke name=\"test\" returntype=\"void\" /></invoke>");

			/*
			<invoke name="sendText" returntype="xml">
			<arguments>
			<string>some text message here</string>
			</arguments>
			</invoke>
			*/
		}
	}
}
