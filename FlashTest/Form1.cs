﻿using System.Windows.Forms;

namespace FlashTest
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			axShockwaveFlash1.LoadMovie(0, "https://play.clubpenguinrewritten.pw/media/loader.swf");
			axShockwaveFlash1.Play();
		}
	}
}
