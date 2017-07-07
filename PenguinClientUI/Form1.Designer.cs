namespace PenguinClientUI
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
			this.log = new PenguinClientUI.Log();
			this.logs = new System.Windows.Forms.FlowLayoutPanel();
			this.info = new System.Windows.Forms.CheckBox();
			this.error = new System.Windows.Forms.CheckBox();
			this.send = new System.Windows.Forms.CheckBox();
			this.receive = new System.Windows.Forms.CheckBox();
			this.clear = new System.Windows.Forms.Button();
			this.logLabel = new System.Windows.Forms.Label();
			this.logs.SuspendLayout();
			this.SuspendLayout();
			// 
			// log
			// 
			this.log.Dock = System.Windows.Forms.DockStyle.Top;
			this.log.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.log.Location = new System.Drawing.Point(0, 0);
			this.log.Name = "log";
			this.log.Size = new System.Drawing.Size(784, 526);
			this.log.TabIndex = 0;
			// 
			// logs
			// 
			this.logs.Controls.Add(this.logLabel);
			this.logs.Controls.Add(this.info);
			this.logs.Controls.Add(this.error);
			this.logs.Controls.Add(this.send);
			this.logs.Controls.Add(this.receive);
			this.logs.Controls.Add(this.clear);
			this.logs.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.logs.Location = new System.Drawing.Point(0, 532);
			this.logs.Name = "logs";
			this.logs.Size = new System.Drawing.Size(784, 29);
			this.logs.TabIndex = 1;
			// 
			// info
			// 
			this.info.AutoSize = true;
			this.info.Checked = true;
			this.info.CheckState = System.Windows.Forms.CheckState.Checked;
			this.info.Location = new System.Drawing.Point(37, 3);
			this.info.Name = "info";
			this.info.Size = new System.Drawing.Size(44, 17);
			this.info.TabIndex = 0;
			this.info.Text = "Info";
			this.info.UseVisualStyleBackColor = true;
			this.info.CheckedChanged += new System.EventHandler(this.info_CheckedChanged);
			// 
			// error
			// 
			this.error.AutoSize = true;
			this.error.Checked = true;
			this.error.CheckState = System.Windows.Forms.CheckState.Checked;
			this.error.ForeColor = System.Drawing.Color.Red;
			this.error.Location = new System.Drawing.Point(87, 3);
			this.error.Name = "error";
			this.error.Size = new System.Drawing.Size(48, 17);
			this.error.TabIndex = 1;
			this.error.Text = "Error";
			this.error.UseVisualStyleBackColor = true;
			this.error.CheckedChanged += new System.EventHandler(this.error_CheckedChanged);
			// 
			// send
			// 
			this.send.AutoSize = true;
			this.send.Checked = true;
			this.send.CheckState = System.Windows.Forms.CheckState.Checked;
			this.send.ForeColor = System.Drawing.Color.Green;
			this.send.Location = new System.Drawing.Point(141, 3);
			this.send.Name = "send";
			this.send.Size = new System.Drawing.Size(51, 17);
			this.send.TabIndex = 2;
			this.send.Text = "Send";
			this.send.UseVisualStyleBackColor = true;
			this.send.CheckedChanged += new System.EventHandler(this.send_CheckedChanged);
			// 
			// receive
			// 
			this.receive.AutoSize = true;
			this.receive.Checked = true;
			this.receive.CheckState = System.Windows.Forms.CheckState.Checked;
			this.receive.ForeColor = System.Drawing.Color.Blue;
			this.receive.Location = new System.Drawing.Point(198, 3);
			this.receive.Name = "receive";
			this.receive.Size = new System.Drawing.Size(66, 17);
			this.receive.TabIndex = 3;
			this.receive.Text = "Receive";
			this.receive.UseVisualStyleBackColor = true;
			this.receive.CheckedChanged += new System.EventHandler(this.receive_CheckedChanged);
			// 
			// clear
			// 
			this.clear.Location = new System.Drawing.Point(270, 3);
			this.clear.Name = "clear";
			this.clear.Size = new System.Drawing.Size(75, 23);
			this.clear.TabIndex = 2;
			this.clear.Text = "Clear";
			this.clear.UseVisualStyleBackColor = true;
			this.clear.Click += new System.EventHandler(this.clear_Click);
			// 
			// logLabel
			// 
			this.logLabel.AutoSize = true;
			this.logLabel.Location = new System.Drawing.Point(3, 0);
			this.logLabel.Name = "logLabel";
			this.logLabel.Size = new System.Drawing.Size(28, 13);
			this.logLabel.TabIndex = 4;
			this.logLabel.Text = "Log:";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 561);
			this.Controls.Add(this.logs);
			this.Controls.Add(this.log);
			this.Name = "Form1";
			this.Text = "Form1";
			this.logs.ResumeLayout(false);
			this.logs.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private Log log;
		private System.Windows.Forms.FlowLayoutPanel logs;
		private System.Windows.Forms.CheckBox info;
		private System.Windows.Forms.CheckBox error;
		private System.Windows.Forms.CheckBox send;
		private System.Windows.Forms.CheckBox receive;
		private System.Windows.Forms.Button clear;
		private System.Windows.Forms.Label logLabel;
	}
}

