namespace Set_GSC_Values
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
			this.components = new System.ComponentModel.Container();
			this.lblTitle = new System.Windows.Forms.Label();
			this.lblIP = new System.Windows.Forms.Label();
			this.tbIP = new System.Windows.Forms.TextBox();
			this.tbRegStartAdr = new System.Windows.Forms.TextBox();
			this.lblRegStartAdr = new System.Windows.Forms.Label();
			this.lbMsgs = new System.Windows.Forms.ListBox();
			this.butUpdateSV180 = new System.Windows.Forms.Button();
			this.tbUnitId = new System.Windows.Forms.TextBox();
			this.lblUnitId = new System.Windows.Forms.Label();
			this.butReadFile = new System.Windows.Forms.Button();
			this.butReadSV180 = new System.Windows.Forms.Button();
			this.tbBytesPerReg = new System.Windows.Forms.TextBox();
			this.lblBytesPerReg = new System.Windows.Forms.Label();
			this.butConnect = new System.Windows.Forms.Button();
			this.butWriteFile = new System.Windows.Forms.Button();
			this.lblLastSV180DataTime = new System.Windows.Forms.Label();
			this.tbLastSV180DataTime = new System.Windows.Forms.TextBox();
			this.butContinuousReadSV180 = new System.Windows.Forms.Button();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// lblTitle
			// 
			this.lblTitle.AutoSize = true;
			this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblTitle.Location = new System.Drawing.Point(656, 15);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(174, 29);
			this.lblTitle.TabIndex = 0;
			this.lblTitle.Text = "SV180 Values";
			// 
			// lblIP
			// 
			this.lblIP.AutoSize = true;
			this.lblIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblIP.Location = new System.Drawing.Point(217, 15);
			this.lblIP.Name = "lblIP";
			this.lblIP.Size = new System.Drawing.Size(86, 17);
			this.lblIP.TabIndex = 1;
			this.lblIP.Text = "IP Address";
			// 
			// tbIP
			// 
			this.tbIP.Location = new System.Drawing.Point(311, 15);
			this.tbIP.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tbIP.Name = "tbIP";
			this.tbIP.Size = new System.Drawing.Size(111, 22);
			this.tbIP.TabIndex = 2;
			this.tbIP.Text = "192.168.1.130";
			this.tbIP.Leave += new System.EventHandler(this.tbIP_Leave);
			// 
			// tbRegStartAdr
			// 
			this.tbRegStartAdr.Location = new System.Drawing.Point(560, 15);
			this.tbRegStartAdr.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tbRegStartAdr.Name = "tbRegStartAdr";
			this.tbRegStartAdr.Size = new System.Drawing.Size(61, 22);
			this.tbRegStartAdr.TabIndex = 4;
			this.tbRegStartAdr.Text = "0400";
			this.tbRegStartAdr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tbRegStartAdr.Leave += new System.EventHandler(this.tbRegStartAdr_Leave);
			// 
			// lblRegStartAdr
			// 
			this.lblRegStartAdr.AutoSize = true;
			this.lblRegStartAdr.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblRegStartAdr.Location = new System.Drawing.Point(439, 15);
			this.lblRegStartAdr.Name = "lblRegStartAdr";
			this.lblRegStartAdr.Size = new System.Drawing.Size(117, 17);
			this.lblRegStartAdr.TabIndex = 3;
			this.lblRegStartAdr.Text = "Reg. Start Adr.";
			// 
			// lbMsgs
			// 
			this.lbMsgs.FormattingEnabled = true;
			this.lbMsgs.HorizontalScrollbar = true;
			this.lbMsgs.ItemHeight = 16;
			this.lbMsgs.Location = new System.Drawing.Point(1279, 471);
			this.lbMsgs.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.lbMsgs.Name = "lbMsgs";
			this.lbMsgs.ScrollAlwaysVisible = true;
			this.lbMsgs.Size = new System.Drawing.Size(555, 436);
			this.lbMsgs.TabIndex = 5;
			// 
			// butUpdateSV180
			// 
			this.butUpdateSV180.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butUpdateSV180.Location = new System.Drawing.Point(1635, 71);
			this.butUpdateSV180.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.butUpdateSV180.Name = "butUpdateSV180";
			this.butUpdateSV180.Size = new System.Drawing.Size(180, 43);
			this.butUpdateSV180.TabIndex = 6;
			this.butUpdateSV180.Text = "Update SV180";
			this.butUpdateSV180.UseVisualStyleBackColor = true;
			this.butUpdateSV180.Visible = false;
			this.butUpdateSV180.Click += new System.EventHandler(this.butUpdateSV180_Click);
			// 
			// tbUnitId
			// 
			this.tbUnitId.Location = new System.Drawing.Point(933, 15);
			this.tbUnitId.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tbUnitId.Name = "tbUnitId";
			this.tbUnitId.Size = new System.Drawing.Size(33, 22);
			this.tbUnitId.TabIndex = 8;
			this.tbUnitId.Text = "4";
			this.tbUnitId.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tbUnitId.Leave += new System.EventHandler(this.tbUnitId_Leave);
			// 
			// lblUnitId
			// 
			this.lblUnitId.AutoSize = true;
			this.lblUnitId.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblUnitId.Location = new System.Drawing.Point(868, 15);
			this.lblUnitId.Name = "lblUnitId";
			this.lblUnitId.Size = new System.Drawing.Size(57, 17);
			this.lblUnitId.TabIndex = 7;
			this.lblUnitId.Text = "Unit ID";
			// 
			// butReadFile
			// 
			this.butReadFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butReadFile.Location = new System.Drawing.Point(1433, 15);
			this.butReadFile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.butReadFile.Name = "butReadFile";
			this.butReadFile.Size = new System.Drawing.Size(180, 43);
			this.butReadFile.TabIndex = 9;
			this.butReadFile.Text = "Read File";
			this.butReadFile.UseVisualStyleBackColor = true;
			this.butReadFile.Click += new System.EventHandler(this.butReadFile_Click);
			// 
			// butReadSV180
			// 
			this.butReadSV180.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butReadSV180.Location = new System.Drawing.Point(1433, 71);
			this.butReadSV180.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.butReadSV180.Name = "butReadSV180";
			this.butReadSV180.Size = new System.Drawing.Size(180, 43);
			this.butReadSV180.TabIndex = 10;
			this.butReadSV180.Text = "Read SV180";
			this.butReadSV180.UseVisualStyleBackColor = true;
			this.butReadSV180.Visible = false;
			this.butReadSV180.Click += new System.EventHandler(this.butReadSV180_Click);
			// 
			// tbBytesPerReg
			// 
			this.tbBytesPerReg.Location = new System.Drawing.Point(1079, 15);
			this.tbBytesPerReg.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tbBytesPerReg.Name = "tbBytesPerReg";
			this.tbBytesPerReg.Size = new System.Drawing.Size(43, 22);
			this.tbBytesPerReg.TabIndex = 12;
			this.tbBytesPerReg.Text = "4";
			this.tbBytesPerReg.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tbBytesPerReg.Leave += new System.EventHandler(this.tbBytesPerReg_Leave);
			// 
			// lblBytesPerReg
			// 
			this.lblBytesPerReg.AutoSize = true;
			this.lblBytesPerReg.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblBytesPerReg.Location = new System.Drawing.Point(987, 15);
			this.lblBytesPerReg.Name = "lblBytesPerReg";
			this.lblBytesPerReg.Size = new System.Drawing.Size(82, 17);
			this.lblBytesPerReg.TabIndex = 11;
			this.lblBytesPerReg.Text = "Bytes/Reg";
			// 
			// butConnect
			// 
			this.butConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butConnect.Location = new System.Drawing.Point(20, 15);
			this.butConnect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.butConnect.Name = "butConnect";
			this.butConnect.Size = new System.Drawing.Size(169, 43);
			this.butConnect.TabIndex = 13;
			this.butConnect.Text = "Connect";
			this.butConnect.UseVisualStyleBackColor = true;
			this.butConnect.Click += new System.EventHandler(this.butConnect_Click);
			// 
			// butWriteFile
			// 
			this.butWriteFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butWriteFile.Location = new System.Drawing.Point(1635, 15);
			this.butWriteFile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.butWriteFile.Name = "butWriteFile";
			this.butWriteFile.Size = new System.Drawing.Size(180, 43);
			this.butWriteFile.TabIndex = 14;
			this.butWriteFile.Text = "Write File";
			this.butWriteFile.UseVisualStyleBackColor = true;
			this.butWriteFile.Click += new System.EventHandler(this.butWriteFile_Click);
			// 
			// lblLastSV180DataTime
			// 
			this.lblLastSV180DataTime.AutoSize = true;
			this.lblLastSV180DataTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblLastSV180DataTime.Location = new System.Drawing.Point(1464, 153);
			this.lblLastSV180DataTime.Name = "lblLastSV180DataTime";
			this.lblLastSV180DataTime.Size = new System.Drawing.Size(139, 20);
			this.lblLastSV180DataTime.TabIndex = 15;
			this.lblLastSV180DataTime.Text = "Last Data Time";
			this.lblLastSV180DataTime.Visible = false;
			// 
			// tbLastSV180DataTime
			// 
			this.tbLastSV180DataTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbLastSV180DataTime.Location = new System.Drawing.Point(1635, 153);
			this.tbLastSV180DataTime.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tbLastSV180DataTime.Name = "tbLastSV180DataTime";
			this.tbLastSV180DataTime.ReadOnly = true;
			this.tbLastSV180DataTime.Size = new System.Drawing.Size(180, 26);
			this.tbLastSV180DataTime.TabIndex = 16;
			this.tbLastSV180DataTime.Text = "No SV180 Data Yet";
			this.tbLastSV180DataTime.Visible = false;
			// 
			// butContinuousReadSV180
			// 
			this.butContinuousReadSV180.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butContinuousReadSV180.Location = new System.Drawing.Point(1435, 186);
			this.butContinuousReadSV180.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.butContinuousReadSV180.Name = "butContinuousReadSV180";
			this.butContinuousReadSV180.Size = new System.Drawing.Size(380, 43);
			this.butContinuousReadSV180.TabIndex = 17;
			this.butContinuousReadSV180.Text = "Set Continuous SV180 Reading";
			this.butContinuousReadSV180.UseVisualStyleBackColor = true;
			this.butContinuousReadSV180.Visible = false;
			this.butContinuousReadSV180.Click += new System.EventHandler(this.butContinuousReadSV180_Click);
			// 
			// timer1
			// 
			this.timer1.Interval = 60000;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick_1);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1837, 1051);
			this.Controls.Add(this.butContinuousReadSV180);
			this.Controls.Add(this.tbLastSV180DataTime);
			this.Controls.Add(this.lblLastSV180DataTime);
			this.Controls.Add(this.butWriteFile);
			this.Controls.Add(this.butConnect);
			this.Controls.Add(this.tbBytesPerReg);
			this.Controls.Add(this.lblBytesPerReg);
			this.Controls.Add(this.butReadSV180);
			this.Controls.Add(this.butReadFile);
			this.Controls.Add(this.tbUnitId);
			this.Controls.Add(this.lblUnitId);
			this.Controls.Add(this.butUpdateSV180);
			this.Controls.Add(this.lbMsgs);
			this.Controls.Add(this.tbRegStartAdr);
			this.Controls.Add(this.lblRegStartAdr);
			this.Controls.Add(this.tbIP);
			this.Controls.Add(this.lblIP);
			this.Controls.Add(this.lblTitle);
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "Form1";
			this.Text = "SV180 Values";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.Label lblIP;
		private System.Windows.Forms.TextBox tbIP;
		private System.Windows.Forms.TextBox tbRegStartAdr;
		private System.Windows.Forms.Label lblRegStartAdr;
		private System.Windows.Forms.ListBox lbMsgs;
		private System.Windows.Forms.Button butUpdateSV180;
		private System.Windows.Forms.TextBox tbUnitId;
		private System.Windows.Forms.Label lblUnitId;
		private System.Windows.Forms.Button butReadFile;
		private System.Windows.Forms.Button butReadSV180;
		private System.Windows.Forms.TextBox tbBytesPerReg;
		private System.Windows.Forms.Label lblBytesPerReg;
		private System.Windows.Forms.Button butConnect;
		private System.Windows.Forms.Button butWriteFile;
		private System.Windows.Forms.Label lblLastSV180DataTime;
		private System.Windows.Forms.TextBox tbLastSV180DataTime;
		private System.Windows.Forms.Button butContinuousReadSV180;
		private System.Windows.Forms.Timer timer1;
	}
}

