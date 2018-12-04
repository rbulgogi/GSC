using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;


namespace Set_GSC_Values
{
	public partial class Form1 : Form
	{
		string strMsg;

		int iLoopCount = 0;

		// Create a StreamWriter to output data to a file
		string strOutputFile = "..\\..\\Set_GSC_Values_Timing.txt";
		StreamWriter swTimingFile;
		TimeSpan ts = new TimeSpan();
		DateTime dt1;
		DateTime dt2;

		int iDT_MinMSecs;
		int iDT_MaxMSecs;
		int iDT_AvgMSecs;

		//IPAddress[] IPs;

		// Declare socket variable
		Socket sock = null;

		//Constants for Modbus functions
		const byte byteMbBaseReqADUHdrSize = 7;
		const byte byteMbBaseReqADUQuerySize = 5;

		const int iMAX_TRYS = 3;
		const int iMAX_AMDS = 4; 
		const int iServPort = 502;

		const int iMaxGrps = 6; 
		const int iMaxGscSynNum = 178;
		const short sMaxGscRegsToWrite = 60;

		ushort usRegStartAdr;
		byte byteUnitId;
		int iBytesPerReg;

		Label[] GscTbLabels = new Label[iMaxGscSynNum];
		TextBox[] GscTextboxes = new TextBox[iMaxGscSynNum];

		Label[,] GrpAMDLabels = new Label[iMAX_AMDS, iMaxGrps - 1];
		TextBox[,] GrpAMDTextboxes = new TextBox[iMAX_AMDS, iMaxGrps - 1];
		string[] strGrpAMDLabelText = new string[iMAX_AMDS] { "Pts Grp-", "Avg Grp-", "Max Grp-", "Dev Grp-" };

		float[] xGscSynVals = new float[iMaxGscSynNum];

		float[,] xGscAMDSynVals = new float[iMAX_AMDS, iMaxGrps - 1];

		// Declare jagged array of two elements [iGrp][iSyn]
		int[][] iGrpsBySyn = new int[iMaxGrps][];
		string[] strGrpNums = new string[iMaxGrps] { "1", "2", "3", "4", "7", "No" };
		string[] strGrpNames = new string[iMaxGrps] { "Group-1", "Group-2", "Group-3", "Group-4", "Group-7", "No-Group" };
		// Label array for Group Names
		Label[] grpLabels = new Label[iMaxGrps];

		// Declare Union
		UintFloatShortByte_Union Val_Union = new UintFloatShortByte_Union();

		// ===========================================================================================
		public Form1()
		{
			InitializeComponent();

		}

		// ===========================================================================================
		private void Form1_Load(object sender, EventArgs e)
		{
			swTimingFile = new StreamWriter(new FileStream(strOutputFile, FileMode.Append, FileAccess.Write));
			//swTimingFile.WriteLine("\n{0} - Output file opened\n", System.DateTime.Now.ToString());

			// Initialized the jagged array elements
			iGrpsBySyn[0] = new int[12] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
			iGrpsBySyn[1] = new int[60] { 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42,
													43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72};
			iGrpsBySyn[2] = new int[42] { 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121,
													122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142};
			iGrpsBySyn[3] = new int[30] { 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172 };
			iGrpsBySyn[4] = new int[6]	 { 173, 174, 175, 176, 177, 178 };
			iGrpsBySyn[5] = new int[13] { 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 100 };

			int iMaxTempsPerColumn = 30;

			int iLabelWidth = 80; 
			int iLabelHeight = 18; 
			int iTextboxWidth = 55;
			int iTextboxHeight = 19;

			strMsg = string.Format("Form1_Load(): tbIP.Text = {0}", tbIP.Text);
			lbMsgs.Items.Add(strMsg);

			usRegStartAdr = ushort.Parse(tbRegStartAdr.Text);
			strMsg = string.Format("Form1_Load(): usRegStartAdr = {0}", usRegStartAdr.ToString("D4"));
			lbMsgs.Items.Add(strMsg);

			byteUnitId = byte.Parse(tbUnitId.Text);
			strMsg = string.Format("Form1_Load(): byteUnitId = {0}", byteUnitId.ToString("D1"));
			lbMsgs.Items.Add(strMsg);

			iBytesPerReg = Int32.Parse(tbBytesPerReg.Text);
			strMsg = string.Format("Form1_Load(): iBytesPerReg = {0}", iBytesPerReg.ToString("D4"));
			lbMsgs.Items.Add(strMsg);

			string strDataFile = "..\\..\\GSC_Data1.txt";
			// Get GSC values by Syn # from a text file.
			ReadDataFile(strDataFile);

			strMsg = string.Format("Form1_Load(): Returned from ReadDataFile()");
			lbMsgs.Items.Add(strMsg);

			int iCount = 0;
			int iXPos_Label = 0;
			int iXPos_Textbox = 0;
			int iYPos = 0;

			int iRow = 0;
			int iCol = 0;

			int[] iAMD_X_Pos = new int[iMaxGrps];
			int iAMD_Y_Pos = 0;

			for (int i = 0; i < iMAX_AMDS; i++)
				for (int j = 0; j < iMaxGrps - 1; j++)
					xGscAMDSynVals[i, j] = 0.0f;

			//strMsg = string.Format("Form1_Load(): xGscAMDSynVals initialized");
			//lbMsgs.Items.Add(strMsg);

			// Create GSC Labels, Textboxes and populate all GSC PID values
			for (int i = 0; i < iMaxGrps; i++)
			{
				iRow = 0;
				iCol++;
				iXPos_Label = (iCol-1) * (iLabelWidth + iTextboxWidth + 20) + 10;
				iAMD_X_Pos[i] = iXPos_Label;
				iYPos = 60;

				//strMsg = string.Format("Form1_Load(): iAMD_X_Pos[{0}] = {1}", i, iAMD_X_Pos[i]);
				//lbMsgs.Items.Add(strMsg);

				grpLabels[i] = new Label();
				grpLabels[i].Location = new Point(iXPos_Label, iYPos);
				grpLabels[i].Width = iLabelWidth;
				grpLabels[i].Height = iLabelHeight;
				grpLabels[i].Text = strGrpNames[i];
				grpLabels[i].Visible = true;
				this.Controls.Add(grpLabels[i]);

				for (int j = 0; j < iGrpsBySyn[i].Length; j++)
				{
					if (iRow == 0)
					{
						iXPos_Label = (iCol-1) * (iLabelWidth + iTextboxWidth + 20) + 10;
						iXPos_Textbox = iXPos_Label + iLabelWidth;
						//strMsg = string.Format("Form1_Load(): iXPos_Label = {0}, iXPos_Textbox = {1}", iXPos_Label, iXPos_Textbox);
						//lbMsgs.Items.Add(strMsg);
						iYPos = 60 + iLabelHeight;
					}
					else if (iRow % iMaxTempsPerColumn == 0)
					{
						iRow = 0;
						iCol++;
						iXPos_Label = (iCol - 1) * (iLabelWidth + iTextboxWidth + 20) + 10;
						iXPos_Textbox = iXPos_Label + iLabelWidth;
						//strMsg = string.Format("Form1_Load(): iXPos_Label = {0}, iXPos_Textbox = {1}", iXPos_Label, iXPos_Textbox);
						//lbMsgs.Items.Add(strMsg);
						iYPos = 60 + iLabelHeight;
					}

					string labelTxt = string.Format("GSCPT{0}", (iGrpsBySyn[i][j]).ToString("D3"));

					//strMsg = string.Format("Form1_Load(): Processing, labelTxt = " + labelTxt);
					//lbMsgs.Items.Add(strMsg);

					GscTbLabels[iCount] = new Label();
					GscTbLabels[iCount].Location = new Point(iXPos_Label, iYPos);
					GscTbLabels[iCount].Width = iLabelWidth;
					GscTbLabels[iCount].Height = iLabelHeight;
					GscTbLabels[iCount].Text = labelTxt;
					GscTbLabels[iCount].Visible = true;
					this.Controls.Add(GscTbLabels[iCount]);

					GscTextboxes[iCount] = new TextBox();
					GscTextboxes[iCount].Location = new Point(iXPos_Textbox, iYPos);
					GscTextboxes[iCount].Width = iTextboxWidth;
					GscTextboxes[iCount].Height = iTextboxHeight;
					GscTextboxes[iCount].TextAlign = HorizontalAlignment.Right;
					GscTextboxes[iCount].Text = xGscSynVals[iGrpsBySyn[i][j] - 1].ToString("n1");
					GscTextboxes[iCount].Leave += new System.EventHandler(this.GSC_textbox_Leave);
					GscTextboxes[iCount].Visible = true;
					this.Controls.Add(GscTextboxes[iCount]);

					// Get the maximum Y-component to know where to put Avg, Max & Dev information
					iAMD_Y_Pos = Math.Max(iAMD_Y_Pos, iYPos);

					if (i < iMaxGrps-1)
					{
						if (xGscSynVals[iGrpsBySyn[i][j] - 1] != 0.0 && xGscSynVals[iGrpsBySyn[i][j] - 1] != 100.0)
						{
							xGscAMDSynVals[0, i] += 1.0f;
							xGscAMDSynVals[1, i] += xGscSynVals[iGrpsBySyn[i][j] - 1];
							xGscAMDSynVals[2, i] = Math.Max(xGscAMDSynVals[2, i], xGscSynVals[iGrpsBySyn[i][j] - 1]);
						}
					}

					iRow++;
					iCount++;
					iYPos += (iTextboxHeight + 3);
				} 
			}

			for (int i = 0; i < iMaxGrps - 1; i++)
			{
				// Get group AVG & Dev from Avg
				xGscAMDSynVals[1, i] = xGscAMDSynVals[1, i] / xGscAMDSynVals[0, i];
				xGscAMDSynVals[3, i] = xGscAMDSynVals[2, i] - xGscAMDSynVals[1, i];

				//strMsg = string.Format("Form1_Load(): Group {0} - Avg, Max & Dev = {1}, {2} & {3} ", strGrpNums[i], xGscAMDSynVals[0, i].ToString("n1"),
				//   xGscAMDSynVals[1, i].ToString("n1"), xGscAMDSynVals[2, i].ToString("n1"));
				//lbMsgs.Items.Add(strMsg);
			}

			// Move Y-position down a bit for Avg, Max & Dev values
			iAMD_Y_Pos += (iTextboxHeight + 3);
			// Loop to process AMD Labels and Textboxes
			for (int iAMD = 0; iAMD < iMAX_AMDS; iAMD++)
			{
				iAMD_Y_Pos += (iTextboxHeight + 3);							
				for (int i = 0; i < iMaxGrps - 1; i++)
				{
					//strMsg = string.Format("Form1_Load(): iAMD_Y_Pos = {0}", iAMD_Y_Pos);
					//lbMsgs.Items.Add(strMsg);		   

					GrpAMDLabels[iAMD, i] = new Label();
					GrpAMDLabels[iAMD, i].Location = new Point(iAMD_X_Pos[i], iAMD_Y_Pos);
					//GrpAMDLabels[iAMD, i].Location = new Point(iAMD_X_Pos[i], iAMD_Y_Pos + (i - 1) * (iTextboxHeight + 3));
					GrpAMDLabels[iAMD, i].Width = iLabelWidth;
					GrpAMDLabels[iAMD, i].Height = iLabelHeight;
					GrpAMDLabels[iAMD, i].Text = strGrpAMDLabelText[iAMD] + strGrpNums[i];
					GrpAMDLabels[iAMD, i].Visible = true;
					this.Controls.Add(GrpAMDLabels[iAMD, i]);

					GrpAMDTextboxes[iAMD, i] = new TextBox();
					GrpAMDTextboxes[iAMD, i].Location = new Point(iAMD_X_Pos[i] + iLabelWidth, iAMD_Y_Pos);
					//GrpAMDTextboxes[iAMD, i].Location = new Point(iAMD_X_Pos[i] + iLabelWidth, iAMD_Y_Pos + (i - 1) * (iTextboxHeight + 3));
					GrpAMDTextboxes[iAMD, i].Width = iTextboxWidth;
					GrpAMDTextboxes[iAMD, i].Height = iTextboxHeight;
					GrpAMDTextboxes[iAMD, i].TextAlign = HorizontalAlignment.Right;
					if (iAMD == 0)
						GrpAMDTextboxes[iAMD, i].Text = ((int)xGscAMDSynVals[iAMD, i]).ToString("d");
					else
						GrpAMDTextboxes[iAMD, i].Text = xGscAMDSynVals[iAMD, i].ToString("n2");
					GrpAMDTextboxes[iAMD, i].ReadOnly = true;
					GrpAMDTextboxes[iAMD, i].Visible = true;
					this.Controls.Add(GrpAMDTextboxes[iAMD, i]);
				}
			}

			lbMsgs.SelectedIndex = lbMsgs.Items.Count - 1;

		}

		// ===========================================================================================
		private void tbIP_Leave(object sender, EventArgs e)
		{
			if (tbIP.Text == "")
			{
				strMsg = string.Format("tbIP_Leave(): address.ToString() is blank");
				lbMsgs.Items.Add(strMsg);
			}
			else
			{
				IPAddress ipAddr = IPAddress.Parse(tbIP.Text);

				// Display the address in standard notation.
				strMsg = string.Format("tbIP_Leave(): Parsing your input string: " + "\"" + tbIP.Text + "\"" + " produces this address (shown in its standard notation): " + ipAddr.ToString());
				lbMsgs.Items.Add(strMsg);
			}
		}

		// ===========================================================================================
		private void tbRegStartAdr_Leave(object sender, EventArgs e)
		{
			usRegStartAdr = ushort.Parse(tbRegStartAdr.Text);
			strMsg = string.Format("tbRegStartAdr_Leave(): usRegStartAdr = {0}", usRegStartAdr.ToString("D4"));
			lbMsgs.Items.Add(strMsg);
		}

		// ===========================================================================================
		private void tbUnitId_Leave(object sender, EventArgs e)
		{
			byteUnitId = byte.Parse(tbUnitId.Text);
			strMsg = string.Format("tbUnitId_Leave(): byteUnitId = {0}", byteUnitId.ToString());
			lbMsgs.Items.Add(strMsg);
		}

		// ===========================================================================================
		private void tbBytesPerReg_Leave(object sender, EventArgs e)
		{
			iBytesPerReg = Int32.Parse(tbBytesPerReg.Text);
			strMsg = string.Format("tbBytesPerReg_Leave(): iBytesPerReg = {0}", iBytesPerReg.ToString());
			lbMsgs.Items.Add(strMsg);
		}

		// ===========================================================================================
		private void GSC_textbox_Leave(object sender, EventArgs e)
		{
			// Re-format affected textbox
			((TextBox)sender).Text = float.Parse(((TextBox)sender).Text).ToString("N1");

			strMsg = string.Format("GSC_textbox_Leave(): GSC textbox re-formatted");
			lbMsgs.Items.Add(strMsg);

			// Go through all GSC Textboxes and re-populate GSC float values
			UpdateGscMemoryValuesFromTextboxes();
		}

		// ===========================================================================================
		private void UpdateGscTextboxesFromMemoryValues()
		{
			//strMsg = string.Format("UpdateGscTextboxesFromMemoryValues(): Entered");
			//lbMsgs.Items.Add(strMsg);

			int iCount = 0;

			// Populate all GSC TextBoxes with GSC values
			for (int i = 0; i < iMaxGrps; i++)
			{
				for (int j = 0; j < iGrpsBySyn[i].Length; j++)
				{
					GscTextboxes[iCount].Text = xGscSynVals[iGrpsBySyn[i][j] - 1].ToString("n1");
					GscTextboxes[iCount].Refresh();
					iCount++;
				}
			}

			// Call routine to update the AMD values and textboxes
			UpdateAMDValuesAndTextboxesFromGscMemoryValues();
		}

		// ===========================================================================================
		private void UpdateGscMemoryValuesFromTextboxes()
		{
			// Routine to update the GSC memory values from information within GSC textboxes

			//strMsg = string.Format("UpdateGscMemoryValuesFromTextboxes(): Entered");
			//lbMsgs.Items.Add(strMsg);

			int iCount = 0;

			// Update all GSC values from GSC TextBoxes
			for (int i = 0; i < iMaxGrps; i++)
				for (int j = 0; j < iGrpsBySyn[i].Length; j++)
				{
					xGscSynVals[iGrpsBySyn[i][j] - 1] = float.Parse(GscTextboxes[iCount].Text);
					iCount++;
				}

			// Call routine to update the AMD values and textboxes
			UpdateAMDValuesAndTextboxesFromGscMemoryValues();
		}

		// ===========================================================================================
		private void UpdateAMDValuesAndTextboxesFromGscMemoryValues()
		{
			// Routine to update AMD values and textboxes from GSC textbox values

			//strMsg = string.Format("UpdateAMDValuesAndTextboxesFromGscMemoryValues(): Entered");
			//lbMsgs.Items.Add(strMsg);

			for (int i = 0; i < iMAX_AMDS; i++)
				for (int j = 0; j < iMaxGrps - 1; j++)
					xGscAMDSynVals[i, j] = 0.0f;

			// Update AMD memory values from GSC memory values
			for (int i = 0; i < iMaxGrps; i++)
			{
				for (int j = 0; j < iGrpsBySyn[i].Length; j++)
				{
					if (i < iMaxGrps - 1)
					{
						if (xGscSynVals[iGrpsBySyn[i][j] - 1] > 0.0 && xGscSynVals[iGrpsBySyn[i][j] - 1] < 100.0)
						{
							xGscAMDSynVals[0, i] += 1.0f;
							xGscAMDSynVals[1, i] += xGscSynVals[iGrpsBySyn[i][j] - 1];
							xGscAMDSynVals[2, i] = Math.Max(xGscAMDSynVals[2, i], xGscSynVals[iGrpsBySyn[i][j] - 1]);
						}
					}
				}
			}

			for (int i = 0; i < iMaxGrps - 1; i++)
			{
				// Calculate group AVG & Dev from Avg
				if (xGscAMDSynVals[0, i] > 0.0)
					xGscAMDSynVals[1, i] = xGscAMDSynVals[1, i] / xGscAMDSynVals[0, i];
				xGscAMDSynVals[3, i] = xGscAMDSynVals[2, i] - xGscAMDSynVals[1, i];
			}

			for (int iAMD = 0; iAMD < iMAX_AMDS; iAMD++)
			{
				for (int i = 0; i < iMaxGrps - 1; i++)
				{
					// Output newly calculated AMD values to AMD textboxes
					if (iAMD == 0)
						GrpAMDTextboxes[iAMD, i].Text = ((int)xGscAMDSynVals[iAMD, i]).ToString("d");
					else
						GrpAMDTextboxes[iAMD, i].Text = xGscAMDSynVals[iAMD, i].ToString("n2");
				}
			}
		}

		// ===========================================================================================
		private void butConnect_Click(object sender, EventArgs e)
		{
			try
			{
				// Create an instance of IPAddress for the specified address string (in  
				// dotted-quad, or colon-hexadecimal notation).
				IPAddress ipAddr = IPAddress.Parse(tbIP.Text);

				if (ipAddr.ToString() == tbIP.Text)
				{
					IPAddress[] IPs = Dns.GetHostAddresses(tbIP.Text);

					// If socket is not null then we need to close the socket before trying to open it
					if (sock != null)
					{
						strMsg = string.Format("butConnect_Click(): Socket open, so closing it now");
						lbMsgs.Items.Add(strMsg);

						sock.Close();
						sock = null;
					}

					// Create socket and connect if socket == null
					if (sock == null)
					{
						try
						{
							sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

							sock.Connect(IPs[0], iServPort);

							//// Make "sock" a nonblocking Socket (i.e. Blocking = .false.)
							//sock.Blocking = false;

							// Make "sock" a blocking Socket (i.e. Blocking = .true.)
							sock.Blocking = true; 

							butReadSV180.Visible = true;
							butUpdateSV180.Visible = true;
							lblLastSV180DataTime.Visible = true;
							tbLastSV180DataTime.Visible = true;
							butContinuousReadSV180.Visible = true;

							strMsg = string.Format("butConnect_Click(): Socket creation successful");
							lbMsgs.Items.Add(strMsg);

						}
						catch (Exception ex)
						{
							strMsg = string.Format("butConnect_Click(): Socket error, ex.Message = {0}", ex.Message);
							lbMsgs.Items.Add(strMsg);

							butReadSV180.Visible = false;
							butUpdateSV180.Visible = false;
							lblLastSV180DataTime.Visible = false;
							tbLastSV180DataTime.Visible = false;
							butContinuousReadSV180.Visible = false;
						}
					}
				}
				else
				{
					strMsg = string.Format("butConnect_Click(): address.ToString() != tbIP.Text, HOLD THE BOAT");
					lbMsgs.Items.Add(strMsg);
				}
			}

			catch (ArgumentNullException ex)
			{
				strMsg = string.Format("butConnect_Click(): ArgumentNullException caught!!!");
				lbMsgs.Items.Add(strMsg);
				strMsg = string.Format("butConnect_Click(): Source : " + ex.Source);
				lbMsgs.Items.Add(strMsg);
				strMsg = string.Format("butConnect_Click(): Message : " + ex.Message);
				lbMsgs.Items.Add(strMsg);
			}

			catch (FormatException ex)
			{
				strMsg = string.Format("butConnect_Click(): FormatException caught!!!");
				lbMsgs.Items.Add(strMsg);
				strMsg = string.Format("butConnect_Click(): Source : " + ex.Source);
				lbMsgs.Items.Add(strMsg);
				strMsg = string.Format("butConnect_Click(): Message : " + ex.Message);
				lbMsgs.Items.Add(strMsg);
			}

			catch (Exception ex)
			{
				strMsg = string.Format("butConnect_Click(): Exception caught!!!");
				lbMsgs.Items.Add(strMsg);
				strMsg = string.Format("butConnect_Click(): Source : " + ex.Source);
				lbMsgs.Items.Add(strMsg);
				strMsg = string.Format("butConnect_Click(): Message : " + ex.Message);
				lbMsgs.Items.Add(strMsg);
			}

			// Update listbox selected index
			lbMsgs.SelectedIndex = lbMsgs.Items.Count - 1;

		}

		// ===========================================================================================
		private void butReadFile_Click(object sender, EventArgs e)
		{
			// Displays an OpenFileDialog so the user can select an input file.
			OpenFileDialog openFileDialog1 = new OpenFileDialog();
			openFileDialog1.Filter = "Data Text Files|*.txt";
			openFileDialog1.Title = "Select a Data Text File";
			openFileDialog1.InitialDirectory = "..\\";

			// Show the Dialog.
			// If the user clicked OK in the dialog and a *.txt file was selected, use it.
			DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
			if (result == DialogResult.OK) // Test result.
			{
				string strDataFile = openFileDialog1.FileName;
				//strMsg = string.Format("butReadFile_Click(): strDataFile = {0}", strDataFile);
				//lbMsgs.Items.Add(strMsg);
				ReadDataFile(strDataFile);

				//strMsg = string.Format("butReadFile_Click(): Calling UpdateGscTextboxesFromMemoryValues()");
				//lbMsgs.Items.Add(strMsg);

				// Call routine to update the GSC textboxes from the GSC memory values
				UpdateGscTextboxesFromMemoryValues();
				//UpdateGscMemoryValuesFromTextboxes();

				//strMsg = string.Format("butReadFile_Click(): Calling UpdateAMDValuesAndTextboxesFromGscMemoryValues()");
				//lbMsgs.Items.Add(strMsg);

				// Call routine to update the AMD values and textboxes
				UpdateAMDValuesAndTextboxesFromGscMemoryValues();
			}
			else
			{
				strMsg = string.Format("butReadFile_Click(): Error - getting data file");
				lbMsgs.Items.Add(strMsg);
			}

			// Update listbox selected index
			lbMsgs.SelectedIndex = lbMsgs.Items.Count - 1;
		}

		// ===========================================================================================
		private void ReadDataFile(string strDataFile)
		{
			int iGoodGscDataLinesRead = 0;

			try
			{
				// Create an instance of StreamReader to read from a file.
				// The "using" statement also closes the StreamReader. << *** IMPORTANT NOTE *** >>

				using (StreamReader sr = new StreamReader(strDataFile))
				{
					String strLine;
					// Read lines from the file with GSC values until the end of the file is reached.
					while ((strLine = sr.ReadLine()) != null)
					{
						//strMsg = string.Format("ReadDataFile(): strLine = {0}", strLine);
						//lbMsgs.Items.Add(strMsg);

						// Check for a good data line then parse for GSC value by Syn #
						if (strLine[0] == '!')
							// Comment line
							continue;

						// Parse the line
						string[] items = strLine.Split(',');

						//strMsg = string.Format("ReadDataFile(): items.Length = {0}", items.Length.ToString("D4"));
						//lbMsgs.Items.Add(strMsg);

						if (items.Length == 2)
						{
							//strMsg = string.Format("ReadDataFile(): items[0] = {0}", items[0]);
							//lbMsgs.Items.Add(strMsg);
							//strMsg = string.Format("ReadDataFile(): items[1] = {0}", items[1]);
							//lbMsgs.Items.Add(strMsg);

							string strSynName = items[0].Trim();
							if (strSynName.Substring(0, 5) != "GSCPT")
								continue;

							int iSyn = Int32.Parse(strSynName.Substring(5, 3));

							xGscSynVals[iSyn - 1] = float.Parse(items[1].Trim());
							iGoodGscDataLinesRead++;
						}
					}
				}
			}
			catch (Exception e)
			{
				// Let the user know what went wrong.
				strMsg = string.Format("ReadDataFile(): Exception: e.Message = {0}", e.Message);
				lbMsgs.Items.Add(strMsg);
			}
			finally
			{
				strMsg = string.Format("ReadDataFile(): iGoodGscDataLinesRead = {0}", iGoodGscDataLinesRead.ToString());
				lbMsgs.Items.Add(strMsg);
			}
		}

		// ===========================================================================================
		private void butWriteFile_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();

			saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
			saveFileDialog1.FilterIndex = 2;
			saveFileDialog1.RestoreDirectory = true;

			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				string strDataFile = saveFileDialog1.FileName;
				strMsg = string.Format("butWriteFile_Click(): strDataFile = {0}", strDataFile);
				lbMsgs.Items.Add(strMsg);
				WriteDataFile(strDataFile);
			}
			else
			{
				strMsg = string.Format("butWriteFile_Click(): Error - getting data file");
				lbMsgs.Items.Add(strMsg);
			}

			// Update listbox selected index
			lbMsgs.SelectedIndex = lbMsgs.Items.Count - 1;
		}

		// ===========================================================================================
		private void WriteDataFile(string strDataFile)
		{
			int iDataLinesWritten = 0;

			try
			{
				// Create an instance of StreamWriter to write a file.
				// The "using" statement also closes the StreamWriter. << *** IMPORTANT NOTE *** >>

				using (StreamWriter sw = new StreamWriter(strDataFile))
				{
					// Output some header lines to the file before the GSC point data
					sw.WriteLine("!   1) Exclamation point in col # 1 is comment line.");
					iDataLinesWritten++;
					sw.WriteLine("!   2) Data lines are <Synonym name> and <value> separated by a comma"); 
					iDataLinesWritten++;
					sw.WriteLine("!");
					iDataLinesWritten++;

					// Output a line for every GSC value
					for (int i = 0; i < iMaxGrps; i++)
					{
						for (int j = 0; j < iGrpsBySyn[i].Length; j++)
						{
							sw.WriteLine(string.Format("GSCPT{0},{1}", iGrpsBySyn[i][j].ToString("d3"), xGscSynVals[iGrpsBySyn[i][j] - 1].ToString("n1")));
							iDataLinesWritten++;
						}
					}
				}
			}
			catch (Exception e)
			{
				// Let the user know what went wrong.
				strMsg = string.Format("WriteDataFile(): Exception: e.Message = {0}", e.Message);
				lbMsgs.Items.Add(strMsg);
			}
			finally
			{
				strMsg = string.Format("WriteDataFile(): iDataLinesWritten = {0}", iDataLinesWritten.ToString());
				lbMsgs.Items.Add(strMsg);
			}
		}

		// ===========================================================================================
		private void butReadSV180_Click(object sender, EventArgs e)
		{
			//strMsg = string.Format("butReadSV180_Click(): Calling ReadSV180Data()");
			//lbMsgs.Items.Add(strMsg);

			// Call routine to update the GSC textboxes from the GSC memory values
			ReadSV180Data();

			//strMsg = string.Format("butReadSV180_Click(): Calling UpdateGscTextboxesFromMemoryValues()");
			//lbMsgs.Items.Add(strMsg);

			// Call routine to update the GSC textboxes from the GSC memory values
			UpdateGscTextboxesFromMemoryValues();

			//strMsg = string.Format("butReadSV180_Click(): Calling UpdateAMDValuesAndTextboxesFromGscMemoryValues()");
			//lbMsgs.Items.Add(strMsg);

			// Call routine to update the AMD values and textboxes
			UpdateAMDValuesAndTextboxesFromGscMemoryValues();

			// Update listbox selected index
			lbMsgs.SelectedIndex = lbMsgs.Items.Count - 1;
		}


		// ===========================================================================================
		private void ReadSV180Data()
		{
			// Build the header buffer for function "strMbFunctions[usFct]"
			//strMsg = string.Format("ReadSV180Data(): Build the header buffer for Reading SV180 Registers");
			//lbMsgs.Items.Add(strMsg);

			byte[] byteValueBuffer = { 0x00, 0x00, 0x00, 0x00 };

			ushort usFct = 2;
			short sHdrSize = 12;
			ushort usTransID = (ushort)(usFct + 0x00A0);
			short sMsgSize = (short)(sHdrSize - (short)5 - (short)1);

			byte[] byteRequestBuffer = new byte[sHdrSize];
			for (int j = 0; j < sHdrSize; j++)
				byteRequestBuffer[j] = (byte)0;

			byte[] byteTransID = BitConverter.GetBytes(usTransID);
			byteRequestBuffer[0] = byteTransID[0];												// Request header transaction ID
			byteRequestBuffer[1] = byteTransID[1];												// Request header transaction ID

			byte[] byteMsgSize = BitConverter.GetBytes(sMsgSize);							// Request header message size
			byteRequestBuffer[4] = byteMsgSize[1];												// Request header message size
			byteRequestBuffer[5] = byteMsgSize[0];												// Request header message size

			byteRequestBuffer[6] = byteUnitId; 													// Unit Identifier

			byteRequestBuffer[7] = 0x03;															// Query function code

			ushort usAdrLocation = usRegStartAdr;

			byte[] byteLocation = BitConverter.GetBytes(usAdrLocation);					// Query start address
			byteRequestBuffer[8] = byteLocation[1];											// Query start address
			byteRequestBuffer[9] = byteLocation[0];											// Query start address

			short sNumItems = (short)(iMaxGscSynNum);											// Query # of Data items to process
			if (iBytesPerReg == 2)
				sNumItems = (short)(2 * iMaxGscSynNum);

			// Query # of Data items to process
			byte[] byteNumItems = BitConverter.GetBytes(sNumItems);						// Query # of Data items to process
			byteRequestBuffer[10] = byteNumItems[1];											// Query # of Data items to process
			byteRequestBuffer[11] = byteNumItems[0];											// Query # of Data items to process

			//// Output byteRequestBuffer[] with information to request Holding Register data from the SV180 device
			//for (int j = 0; j < byteRequestBuffer.Length; j++)
			//{
			//   strMsg = string.Format("ReadSV180Data(): byteRequestBuffer[{0}] in Hex = {1:X2}", j, byteRequestBuffer[j]);
			//   lbMsgs.Items.Add(strMsg);
			//}

			iLoopCount++;

			//// DateTime before request
			//dt1 = DateTime.Now;

			int iRecvTrys = 0;
			int iTotalBytesSent = 0;																// Total bytes sent
			int iTotalBytesRcvd = 0;																// Total bytes received
			// Loop until all bytes of the Request have been sent to the SV180 device
			while (sock != null && iTotalBytesSent < byteRequestBuffer.Length)
			{
				// Send the encoded string to the SV180 device
				try
				{
					iRecvTrys++;
					int iBytesSent = 0;
					iBytesSent = sock.Send(byteRequestBuffer, iTotalBytesSent, byteRequestBuffer.Length - iTotalBytesSent, SocketFlags.None);
					iTotalBytesSent += iBytesSent;
				}
				catch (SocketException se)
				{
					if (se.ErrorCode == 10035)
					{
						// WSAEWOULDBLOCK: Resource temnporarily unavailable;
						strMsg = string.Format("ReadSV180Data(): Exception: Temporarily unable to SEND, will retry again later");
						lbMsgs.Items.Add(strMsg);
					}
					else
					{
						strMsg = string.Format("ReadSV180Data(): " + se.ErrorCode + ": " + se.Message);
						lbMsgs.Items.Add(strMsg);
						sock.Close();
						sock = null;
						break;
						//Environment.Exit(se.ErrorCode);
					}
				}

				if (iRecvTrys >= iMAX_TRYS && iTotalBytesSent < byteRequestBuffer.Length)
				{
					sock.Close();
					sock = null;
					strMsg = string.Format("ReadSV180Data(): *******************************\nRequest Transmission Failure:\n*******************************");
					lbMsgs.Items.Add(strMsg);
					break;
				}
				Thread.Sleep(1000);

			} // End - while (sock != null && iTotalBytesSent < byteRequestBuffer.Length)


			// DateTime before request
			dt1 = DateTime.Now;

			//Thread.Sleep(2000);

			strMsg = string.Format("{0} - ReadSV180Data(): Sent {1} total bytes to SV180 device", DateTime.Now.ToString(), iTotalBytesSent);
			lbMsgs.Items.Add(strMsg);

			byte[] byteResponseBuffer;
			// Establish byteResponseBuffer[] for information to be received from the SV180 device
			if (iBytesPerReg == 2)
				byteResponseBuffer = new byte[9 + (sNumItems * 2)];
			else
				byteResponseBuffer = new byte[9 + (sNumItems * 4)];

			iRecvTrys = 0;
			iTotalBytesRcvd = 0;									// Total bytes received
			// Loop until all bytes of the Response have been returned from the SV180 device
			while (sock != null && iTotalBytesRcvd < byteResponseBuffer.Length)
			{
				// Receive the encoded string from the SV180 device
				try
				{
					//strMsg = string.Format("ReadSV180Data(): Delay a second before reading response.");
					//lbMsgs.Items.Add(strMsg);
					//Thread.Sleep(1000);

					iRecvTrys++;
					//strMsg = string.Format("ReadSV180Data(): Response, iRecvTrys = {0}", iRecvTrys);
					//lbMsgs.Items.Add(strMsg);
					//strMsg = string.Format("ReadSV180Data(): Sent {0} total bytes to SV180 device", iTotalBytesSent);
					//lbMsgs.Items.Add(strMsg);

					int iBytesRcvd = 0;
					if ((iBytesRcvd = sock.Receive(byteResponseBuffer, iTotalBytesRcvd, byteResponseBuffer.Length - iTotalBytesRcvd, SocketFlags.None)) == 0)
					{
						strMsg = string.Format("ReadSV180Data(): iBytesRcvd == 0, Connection closed prematurely.");
						lbMsgs.Items.Add(strMsg);
						break;
					}
					iTotalBytesRcvd += iBytesRcvd;

					//strMsg = string.Format("ReadSV180Data(): Received {0} bytes this pass and iTotalBytesRcvd = {1}", iBytesRcvd, iTotalBytesRcvd);
					//lbMsgs.Items.Add(strMsg);
				}
				catch (SocketException se)
				{
					if (se.ErrorCode == 10035)
					{
						// WSAEWOULDBLOCK: Resource temnporarily unavailable
						strMsg = string.Format("ReadSV180Data(): Exception: Temporarily unable to RECEIVE, will retry again later");
						lbMsgs.Items.Add(strMsg);
						//strMsg = string.Format("ReadSV180Data(): Wait a second to catch up");
						//lbMsgs.Items.Add(strMsg);
						//Thread.Sleep(1000);
					}
					else
					{
						strMsg = string.Format("ReadSV180Data(): " + se.ErrorCode + ": " + se.Message);
						lbMsgs.Items.Add(strMsg);
						sock.Close();
						sock = null;
						break;
					}
				}

				if (iTotalBytesRcvd >= 8 && byteRequestBuffer[7] != byteResponseBuffer[7])
				{
					strMsg = string.Format("ReadSV180Data(): ************\nSV180 error: byteRequestBuffer[7] == {0} != byteResponseBuffer[7] == {1}\n************",
						byteRequestBuffer[7], byteResponseBuffer[7]);
					lbMsgs.Items.Add(strMsg);
					break;
				}

				if (iRecvTrys >= iMAX_TRYS && iTotalBytesRcvd < byteResponseBuffer.Length)
				{
					sock.Close();
					sock = null;
					strMsg = string.Format("ReadSV180Data(): *******************************\nResponse Transmission Failure:\n*******************************");
					lbMsgs.Items.Add(strMsg);
					break;
				}
				Thread.Sleep(1000);

			} // End - while (sock != null && iTotalBytesRcvd < byteResponseBuffer.Length) 
			strMsg = string.Format("{0} - ReadSV180Data(): Received {1} total bytes from SV180 device", DateTime.Now.ToString(), iTotalBytesRcvd);
			lbMsgs.Items.Add(strMsg);

			// DateTime after response
			dt2 = DateTime.Now;

			ts = dt2 - dt1;

			iDT_AvgMSecs = ((iLoopCount - 1) * iDT_AvgMSecs + (int)ts.TotalMilliseconds) / iLoopCount;

			if ((int)ts.TotalMilliseconds < iDT_MinMSecs)
			{
				iDT_MinMSecs = (int)ts.TotalMilliseconds;
				swTimingFile.WriteLine("{0} - Min MSecs = {1:D}, Avg MSecs = {2:D}, Max MSecs = {3:D} ", System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff"),
					iDT_MinMSecs, iDT_AvgMSecs, iDT_MaxMSecs);
				swTimingFile.Flush();
			}

			if ((int)ts.TotalMilliseconds > iDT_MaxMSecs)
			{
				iDT_MaxMSecs = (int)ts.TotalMilliseconds;
				swTimingFile.WriteLine("{0} - Min MSecs = {1:D}, Avg MSecs = {2:D}, Max MSecs = {3:D} ", System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff"),
					iDT_MinMSecs, iDT_AvgMSecs, iDT_MaxMSecs);
				swTimingFile.Flush();
			}

			if (iLoopCount % 30 == 0)
			{
				swTimingFile.WriteLine("{0} - Avg MSecs = {1:D}", System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff"), iDT_AvgMSecs);
				swTimingFile.Flush();
			}

			int iPid = 0;
			for (int j = 0; j < iTotalBytesRcvd; j++)
			{
				// Output byteResponseBuffer[] with information from request Holding Register data from the SV180 device
				if (j <= 12)
				{
					//strMsg = string.Format("ReadSV180Data(): byteResponseBuffer[{0}] in Hex = {1:X2}", j, byteResponseBuffer[j]);
					//lbMsgs.Items.Add(strMsg);
				}

				if (j >= 12 && j % 4 == 0)
				{
					float value;
					Array.Copy(byteResponseBuffer, j - 3, byteValueBuffer, 0, 4);
					//float value = BitConverter.ToSingle(byteValueBuffer, 0);
					//strMsg = string.Format("butReadFile_Click(): Reg = {0}, {1,17}{2,18:E7}", j / 4 - 2, BitConverter.ToString(byteValueBuffer, 0, 4), value);
					//lbMsgs.Items.Add(strMsg);

					// Reverse array byte order and try again
					Array.Reverse(byteValueBuffer);
					value = BitConverter.ToSingle(byteValueBuffer, 0);
					xGscSynVals[iPid] = value;
					iPid++;
					//strMsg = string.Format("ReadSV180Data(): Reg = {0}, {1,17}{2,18:E7}", j / 4 - 2, BitConverter.ToString(byteValueBuffer, 0, 4), value);
					//lbMsgs.Items.Add(strMsg);
				}
			}
		}

		// ===========================================================================================
		private void butUpdateSV180_Click(object sender, EventArgs e)
		{
			short sTotGscRegsWritten = 0;
			short sNumRegs = sMaxGscRegsToWrite;													// Query # of registers to force/write
			ushort usAdrLocation = usRegStartAdr;

			// Limitations on the # of External PIDs that can be updated on the SV108 scanners is around 60 (i.e. iMaxGscPtsToWrite)
			// Therefore, need to write smaller sets of PIDs
			do 
			{
				// Build the header buffer for function "strMbFunctions[usFct]"
				//strMsg = string.Format("butUpdateSV180_Click(): Build the header buffer for Writing of SV180 Registers");
				//lbMsgs.Items.Add(strMsg);

				ushort usFct = 7;

				short sByteCount = (short)(sNumRegs * 4);
				if (iBytesPerReg == 2)
				{
					sNumRegs *= 2;																			// Query # of Data items to process
					sByteCount = (short)(sNumRegs * 2);
				}

				short sHdrSize = (short)(12 + sByteCount + 1);
				ushort usTransID = (ushort)((usFct + 1) * 11);
				short sMsgSize = (short)(sHdrSize - (short)5 - (short)1);

				byte[] byteRequestBuffer = new byte[sHdrSize];
				for (int j = 0; j < sHdrSize; j++)
					byteRequestBuffer[j] = (byte)0;

				byte[] byteTransID = BitConverter.GetBytes(usTransID);
				byteRequestBuffer[0] = byteTransID[0];												// Request header transaction ID
				byteRequestBuffer[1] = byteTransID[1];												// Request header transaction ID

				byte[] byteMsgSize = BitConverter.GetBytes(sMsgSize);							// Request header message size
				byteRequestBuffer[4] = byteMsgSize[1];												// Request header message size
				byteRequestBuffer[5] = byteMsgSize[0];												// Request header message size

				byteRequestBuffer[6] = byteUnitId; 													// Unit Identifier

				byteRequestBuffer[7] = 0x10;															// Query function code


				byte[] byteLocation = BitConverter.GetBytes(usAdrLocation);					// Query start address
				byteRequestBuffer[8] = byteLocation[1];											// Query start address
				byteRequestBuffer[9] = byteLocation[0];											// Query start address

				// Kludge for Modbus simulator
				byte[] byteNumItems = BitConverter.GetBytes(sNumRegs);						// Query # of Data items to process
				byteRequestBuffer[10] = byteNumItems[1];											// Query # of Data items to process
				byteRequestBuffer[11] = byteNumItems[0];											// Query # of Data items to process

				byte[] byteNumBytes = BitConverter.GetBytes(sByteCount);						// Byte count in bytes
				byteRequestBuffer[12] = byteNumBytes[0];											// Byte count in bytes

				int idx = 12;
				int iCount = 0;

				//// Zero the "xGscSynVals" array of "iMaxGscSynNum" floats
				//for (int i = 0; i < iMaxGscSynNum; i++)
				//   xGscSynVals[i] = 0.0f;


				//// Populate all GSC values from GSC TextBoxes
				//for (int i = 0; i < iMaxGrps; i++)
				//{
				//   for (int j = 0; j < iGrpsBySyn[i].Length; j++)
				//   {
				//      xGscSynVals[iGrpsBySyn[i][j] - 1] = float.Parse(GscTextboxes[iCount].Text);
				//      iCount++;
				//   }
				//}

				// Populate the request buffer from GSC values
				iCount = 0;
				byte[] byteRegisterData;
				if (iBytesPerReg == 2)
				{
					for (int j = 0; j < sNumRegs / 2; j++)
					{
						Val_Union.fArray[0] = xGscSynVals[sTotGscRegsWritten+iCount];
						byteRegisterData = BitConverter.GetBytes(Val_Union.sArray[1]);			// Register data in bytes

						byteRequestBuffer[idx + 1] = byteRegisterData[1];							// Query Data to process
						byteRequestBuffer[idx + 2] = byteRegisterData[0];							// Query Data to process
						idx += 2;

						byteRegisterData = BitConverter.GetBytes(Val_Union.sArray[0]);			// Register data in bytes

						byteRequestBuffer[idx + 1] = byteRegisterData[1];							// Query Data to process
						byteRequestBuffer[idx + 2] = byteRegisterData[0];							// Query Data to process

						iCount++;
						idx += 2;
					}
				}
				else
				{
					for (int j = 0; j < sNumRegs; j++)
					{
						Val_Union.fArray[0] = xGscSynVals[sTotGscRegsWritten + iCount];

						//byteRegisterData = BitConverter.GetBytes(usRegisterData);				// Register data in bytes
						byteRegisterData = BitConverter.GetBytes(Val_Union.uiArray[0]);		// Register data in bytes

						byteRequestBuffer[idx + 1] = byteRegisterData[3];							// Query Data to process
						byteRequestBuffer[idx + 2] = byteRegisterData[2];							// Query Data to process
						byteRequestBuffer[idx + 3] = byteRegisterData[1];							// Query Data to process
						byteRequestBuffer[idx + 4] = byteRegisterData[0];							// Query Data to process

						iCount++;
						idx += 4;
					}
				}

				iCount = 0;
				byte[] byteValueBuffer = { 0x00, 0x00, 0x00, 0x00 };
				// Build byteRequestBuffer[] with information to request to Force Multiple Registers data to the SV180 device
				for (int j = 0; j < byteRequestBuffer.Length; j++)
				{
					if (j < 30 || j > 700)
					{
						strMsg = string.Format("butUpdateSV180_Click(): byteRequestBuffer[{0}] in Hex = {1:X2}", j, byteRequestBuffer[j]);
						lbMsgs.Items.Add(strMsg);
					}

					if (j >= 16 && j % 4 == 0)
					{
						Array.Copy(byteRequestBuffer, j - 3, byteValueBuffer, 0, 4);
						//value = BitConverter.ToSingle(byteValueBuffer, 0);
						//strOutput = String.Format("{0,17}{1,18:E7}", BitConverter.ToString(byteValueBuffer, 0, 4), value);
						//Output(sw, strOutput);

						// Reverse array byte order and try again
						Array.Reverse(byteValueBuffer);
						float value = BitConverter.ToSingle(byteValueBuffer, 0);
						iCount++;
						strMsg = string.Format("butUpdateSV180_Click(): {0,3:N}) value = {0,18:E7}", iCount, value);
						lbMsgs.Items.Add(strMsg);
					}
				}

				// Establish byteResponseBuffer[] for information to be received from the SV180 device
				byte[] byteResponseBuffer = new byte[12];

				strMsg = string.Format("butUpdateSV180_Click(): byteResponseBuffer.length = {0}", byteResponseBuffer.Length);
				lbMsgs.Items.Add(strMsg);

				strMsg = string.Format("butUpdateSV180_Click(): Sending request buffer for sNumRegs = {0} and usAdrLocation {1} = ", sNumRegs, usAdrLocation.ToString("D4"));
				lbMsgs.Items.Add(strMsg);

				int iRecvTrys = 0;
				int iTotalBytesSent = 0;											// Total bytes sent
				int iTotalBytesRcvd = 0;											// Total bytes received
				// Loop until all bytes of the Request have been sent to the SV180 device
				while (sock != null && iTotalBytesSent < byteRequestBuffer.Length)
				{
					// Send the encoded string to the SV180 device
					try
					{
						iRecvTrys++;
						strMsg = string.Format("butUpdateSV180_Click(): Request, iRecvTrys = {0}", iRecvTrys);
						lbMsgs.Items.Add(strMsg);
						int iBytesSent = 0;
						iBytesSent = sock.Send(byteRequestBuffer, iTotalBytesSent, byteRequestBuffer.Length - iTotalBytesSent, SocketFlags.None);
						iTotalBytesSent += iBytesSent;
						strMsg = string.Format("butUpdateSV180_Click(): Sent {0} Request bytes to the SV180 device this pass ...", iBytesSent);
						lbMsgs.Items.Add(strMsg);
					}
					catch (SocketException se)
					{
						if (se.ErrorCode == 10035)
						{
							// WSAEWOULDBLOCK: Resource temnporarily unavailable;
							strMsg = string.Format("butUpdateSV180_Click(): Exception: Temporarily unable to SEND, will retry again later.");
							lbMsgs.Items.Add(strMsg);
							Thread.Sleep(1000);
						}
						else
						{
							strMsg = string.Format("butUpdateSV180_Click(): " + se.ErrorCode + ": " + se.Message);
							lbMsgs.Items.Add(strMsg);
							sock.Close();
							sock = null;
							break;
							//Environment.Exit(se.ErrorCode);
						}
					}

					if (iRecvTrys >= iMAX_TRYS && iTotalBytesSent < byteRequestBuffer.Length)
					{
						sock.Close();
						sock = null;
						strMsg = string.Format("*******************************\nRequest Transmission Failure:\n*******************************");
						lbMsgs.Items.Add(strMsg);
						break;
					}
					Thread.Sleep(1000);

				} // End - while (sock != null && iTotalBytesSent < byteRequestBuffer.Length)
				strMsg = string.Format("butUpdateSV180_Click(): Sent {0} total bytes to SV180 device", iTotalBytesSent);
				lbMsgs.Items.Add(strMsg);

				iRecvTrys = 0;
				iTotalBytesRcvd = 0;									// Total bytes received
				// Loop until all bytes of the Response have been returned from the SV180 device
				while (sock != null && iTotalBytesRcvd < byteResponseBuffer.Length)
				{
					// Receive the encoded string from the SV180 device
					try
					{
						iRecvTrys++;
						strMsg = string.Format("butUpdateSV180_Click(): Response, iRecvTrys = {0}", iRecvTrys);
						lbMsgs.Items.Add(strMsg);
						int iBytesRcvd = 0;
						if ((iBytesRcvd = sock.Receive(byteResponseBuffer, iTotalBytesRcvd, byteResponseBuffer.Length - iTotalBytesRcvd, SocketFlags.None)) == 0)
						{
							strMsg = string.Format("butUpdateSV180_Click(): iBytesRcvd == 0, Connection closed prematurely.");
							lbMsgs.Items.Add(strMsg);
							break;
						}
						iTotalBytesRcvd += iBytesRcvd;
						strMsg = string.Format("butUpdateSV180_Click(): Received {0} bytes this pass and iTotalBytesRcvd = {1}", iBytesRcvd, iTotalBytesRcvd);
						lbMsgs.Items.Add(strMsg);
					}
					catch (SocketException se)
					{
						if (se.ErrorCode == 10035)
						{
							// WSAEWOULDBLOCK: Resource temnporarily unavailable
							strMsg = string.Format("butUpdateSV180_Click(): Exception: Temporarily unable to RECEIVE, will retry again later.");
							lbMsgs.Items.Add(strMsg);
							Thread.Sleep(1000);
						}
						else
						{
							strMsg = string.Format("butUpdateSV180_Click(): " + se.ErrorCode + ": " + se.Message);
							lbMsgs.Items.Add(strMsg);
							sock.Close();
							sock = null;
							break;
						}
					}

					if (iTotalBytesRcvd >= 8 && byteRequestBuffer[7] != byteResponseBuffer[7])
					{
						strMsg = string.Format("butUpdateSV180_Click(): ************\nSV180 error: byteRequestBuffer[7] == {0} != byteResponseBuffer[7] == {1}\n************",
							byteRequestBuffer[7], byteResponseBuffer[7]);
						lbMsgs.Items.Add(strMsg);
						break;
					}

					if (iRecvTrys >= iMAX_TRYS && iTotalBytesRcvd < byteResponseBuffer.Length)
					{
						sock.Close();
						sock = null;
						strMsg = string.Format("butUpdateSV180_Click(): *******************************\nResponse Transmission Failure:\n*******************************");
						lbMsgs.Items.Add(strMsg);
						break;
					}
					Thread.Sleep(1000);

				} // End - while (sock != null && iTotalBytesRcvd < byteResponseBuffer.Length)  

				strMsg = string.Format("butUpdateSV180_Click(): Received {0} total bytes from SV180 device", iTotalBytesRcvd);
				lbMsgs.Items.Add(strMsg);

				for (int j = 0; j < iTotalBytesRcvd; j++)
				{
					strMsg = string.Format("butUpdateSV180_Click(): byteResponseBuffer[{0}] in Hex = {1:X2}", j, byteResponseBuffer[j]);
					lbMsgs.Items.Add(strMsg);
				}

				// Set up next write if necessary							
				sTotGscRegsWritten += sMaxGscRegsToWrite;
				if ((sTotGscRegsWritten+sMaxGscRegsToWrite) <= iMaxGscSynNum)
				{
					sNumRegs = sMaxGscRegsToWrite;
				}
				else
				{
					sNumRegs = (short)(iMaxGscSynNum - sTotGscRegsWritten);
				}
				if (iBytesPerReg == 2)
					usAdrLocation = (ushort)(usRegStartAdr + sTotGscRegsWritten * 2);
				else
					usAdrLocation = (ushort)(usRegStartAdr + sTotGscRegsWritten);


			} while (sTotGscRegsWritten < iMaxGscSynNum);

			// Update listbox selected index
			lbMsgs.SelectedIndex = lbMsgs.Items.Count - 1;
		}

		// ===========================================================================================
		[System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit)]
		public class UintFloatShortByte_Union
		{
			// Set the offsets to the same position so that variables occupy
			// the same memory address which is essentially what a union does.

			[System.Runtime.InteropServices.FieldOffset(0)]
			public byte[] baArray;

			[System.Runtime.InteropServices.FieldOffset(0)]
			public ushort[] sArray;

			[System.Runtime.InteropServices.FieldOffset(0)]
			public uint[] uiArray;

			[System.Runtime.InteropServices.FieldOffset(0)]
			public float[] fArray;


			// ===========================================================================================
			// Setup a constructor to dimension structure's data member arrays 
			public UintFloatShortByte_Union()
			{
				uiArray = new uint[1];
				fArray = new float[1];
				sArray = new ushort[2];
				baArray = new byte[4];
			}
		}

		private void butContinuousReadSV180_Click(object sender, EventArgs e)
		{
			// Enable or Disable timer
			if (timer1.Enabled)
			{
				timer1.Enabled = false;
				butContinuousReadSV180.Text = "Set Continuous SV180 Reading";
				strMsg = string.Format("butContinuousReadSV180_Click(): Disabled SV180 Read Timer");
				lbMsgs.Items.Add(strMsg);

				swTimingFile.WriteLine("\n{0} - Continuous SV180 Reading Turned Off\n", System.DateTime.Now.ToString());
				swTimingFile.Flush();
			}
			else
			{
				timer1.Enabled = true;
				butContinuousReadSV180.Text = "Reset Continuous SV180 Reading";
				strMsg = string.Format("butContinuousReadSV180_Click(): Enabled SV180 Read Timer");
				lbMsgs.Items.Add(strMsg);

				iDT_MinMSecs = 10000;
				iDT_MaxMSecs = 0;
				iDT_AvgMSecs = 0;

				iLoopCount = 0;

				swTimingFile.WriteLine("\n{0} - Continuous SV180 Reading Turned On\n", System.DateTime.Now.ToString());
				swTimingFile.Flush();
			}

			// Update listbox selected index
			lbMsgs.SelectedIndex = lbMsgs.Items.Count - 1;
		}

		private void timer1_Tick_1(object sender, EventArgs e)
		{
			// Process on each timer tick
			if(butReadSV180.Visible)
			{
				//strMsg = string.Format("timer1_Tick_1(): Calling ReadSV180Data()");
				//lbMsgs.Items.Add(strMsg);

				// Call routine to update the GSC textboxes from the GSC memory values
				ReadSV180Data();

				//strMsg = string.Format("timer1_Tick_1(): Calling UpdateGscTextboxesFromMemoryValues()");
				//lbMsgs.Items.Add(strMsg);

				// Call routine to update the GSC textboxes from the GSC memory values
				UpdateGscTextboxesFromMemoryValues();

				//strMsg = string.Format("timer1_Tick_1(): Calling UpdateAMDValuesAndTextboxesFromGscMemoryValues()");
				//lbMsgs.Items.Add(strMsg);

				// Call routine to update the AMD values and textboxes
				UpdateAMDValuesAndTextboxesFromGscMemoryValues();
				
				tbLastSV180DataTime.Text = DateTime.Now.ToString();
			}
			else
			{
				strMsg = string.Format("timer1_Tick_1(): Need to connect to IP Address");
				lbMsgs.Items.Add(strMsg);
			}

			// Update listbox selected index
			lbMsgs.SelectedIndex = lbMsgs.Items.Count - 1;
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			// Close the 
			swTimingFile.Close();
		}



	}
}
