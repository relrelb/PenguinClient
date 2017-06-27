﻿using System;
using System.Windows.Forms;

namespace PenguinClientFlash
{
	public partial class Form1 : Form
	{
		private Loader loader;

		public Form1()
		{
			InitializeComponent();
			loader = new Loader(axShockwaveFlash, Application.StartupPath + @"\loader.swf");
			loader.InvokeRequest += Loader_InvokeRequest;
			loader.Load();
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
						message += Loader.GetLiteral(e.Arguments[i]);
					}
					MessageBox.Show(message);
					break;
			}
		}

		private void button_Click(object sender, EventArgs e)
		{
			loader.Init();
			Waddle(428310, 400, 400);
		}

		private void Waddle(int id, int x, int y)
		{
			loader.SendPacket("s", "u#sp", new object[] { id, x, y });
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			if (loader != null)
			{
				loader.Dispose();
				loader = null;
			}
		}
	}
}
