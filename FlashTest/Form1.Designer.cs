﻿namespace FlashTest
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.axShockwaveFlash = new AxShockwaveFlashObjects.AxShockwaveFlash();
			this.button = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.axShockwaveFlash)).BeginInit();
			this.SuspendLayout();
			// 
			// axShockwaveFlash
			// 
			this.axShockwaveFlash.Dock = System.Windows.Forms.DockStyle.Fill;
			this.axShockwaveFlash.Enabled = true;
			this.axShockwaveFlash.Location = new System.Drawing.Point(0, 0);
			this.axShockwaveFlash.Name = "axShockwaveFlash";
			this.axShockwaveFlash.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axShockwaveFlash.OcxState")));
			this.axShockwaveFlash.Size = new System.Drawing.Size(284, 261);
			this.axShockwaveFlash.TabIndex = 0;
			// 
			// button
			// 
			this.button.Location = new System.Drawing.Point(106, 0);
			this.button.Name = "button";
			this.button.Size = new System.Drawing.Size(75, 23);
			this.button.TabIndex = 1;
			this.button.Text = "Waddle";
			this.button.UseVisualStyleBackColor = true;
			this.button.Click += new System.EventHandler(this.button_Click);
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(0, 0);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(100, 20);
			this.textBox1.TabIndex = 2;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 261);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.button);
			this.Controls.Add(this.axShockwaveFlash);
			this.Name = "Form1";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.axShockwaveFlash)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private AxShockwaveFlashObjects.AxShockwaveFlash axShockwaveFlash;
		private System.Windows.Forms.Button button;
		private System.Windows.Forms.TextBox textBox1;
	}
}

