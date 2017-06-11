namespace PenguinClientUI
{
	partial class Log
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lines = new System.Windows.Forms.FlowLayoutPanel();
			this.SuspendLayout();
			// 
			// lines
			// 
			this.lines.AutoScroll = true;
			this.lines.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lines.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.lines.Location = new System.Drawing.Point(0, 0);
			this.lines.Name = "lines";
			this.lines.Size = new System.Drawing.Size(150, 150);
			this.lines.TabIndex = 0;
			// 
			// Log
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lines);
			this.Name = "Log";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel lines;
	}
}
