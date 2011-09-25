namespace Touch2PcPrinter
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.dlgOutputFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.dlgPdfProgram = new System.Windows.Forms.OpenFileDialog();
            this.sysTrayBalloon = new System.Windows.Forms.NotifyIcon(this.components);
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.tabPageOptions = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.txtJobTimeout = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtPdfProgramArgs = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnPdfProgBrowse = new System.Windows.Forms.Button();
            this.txtPdfProgram = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnOutputFolderBrowse = new System.Windows.Forms.Button();
            this.txtOutputFolder = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbOutputPrinterBWND = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.chkStartServer = new System.Windows.Forms.CheckBox();
            this.cmbOutputPrinterBWD = new System.Windows.Forms.ComboBox();
            this.cmbOutputPrinterCOLND = new System.Windows.Forms.ComboBox();
            this.cmbOutputPrinterCOLD = new System.Windows.Forms.ComboBox();
            this.tcMain.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.tabPageOptions.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dlgPdfProgram
            // 
            this.dlgPdfProgram.Filter = "AcroRd32.exe|AcroRd32.exe|Executable Files|*.exe";
            // 
            // sysTrayBalloon
            // 
            this.sysTrayBalloon.BalloonTipTitle = "Touch2Pc Printer";
            this.sysTrayBalloon.Icon = ((System.Drawing.Icon)(resources.GetObject("sysTrayBalloon.Icon")));
            this.sysTrayBalloon.Text = "Touch2Pc Printer";
            this.sysTrayBalloon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.sysTrayBalloon_MouseDoubleClick);
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tabPageGeneral);
            this.tcMain.Controls.Add(this.tabPageOptions);
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Location = new System.Drawing.Point(0, 0);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(746, 382);
            this.tcMain.TabIndex = 4;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.chkStartServer);
            this.tabPageGeneral.Controls.Add(this.btnStop);
            this.tabPageGeneral.Controls.Add(this.btnStart);
            this.tabPageGeneral.Controls.Add(this.groupBox2);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGeneral.Size = new System.Drawing.Size(738, 356);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // tabPageOptions
            // 
            this.tabPageOptions.Controls.Add(this.cmbOutputPrinterCOLD);
            this.tabPageOptions.Controls.Add(this.cmbOutputPrinterCOLND);
            this.tabPageOptions.Controls.Add(this.cmbOutputPrinterBWD);
            this.tabPageOptions.Controls.Add(this.label7);
            this.tabPageOptions.Controls.Add(this.label8);
            this.tabPageOptions.Controls.Add(this.label6);
            this.tabPageOptions.Controls.Add(this.txtJobTimeout);
            this.tabPageOptions.Controls.Add(this.label5);
            this.tabPageOptions.Controls.Add(this.btnSave);
            this.tabPageOptions.Controls.Add(this.txtPdfProgramArgs);
            this.tabPageOptions.Controls.Add(this.label4);
            this.tabPageOptions.Controls.Add(this.btnPdfProgBrowse);
            this.tabPageOptions.Controls.Add(this.txtPdfProgram);
            this.tabPageOptions.Controls.Add(this.label3);
            this.tabPageOptions.Controls.Add(this.btnOutputFolderBrowse);
            this.tabPageOptions.Controls.Add(this.txtOutputFolder);
            this.tabPageOptions.Controls.Add(this.label2);
            this.tabPageOptions.Controls.Add(this.cmbOutputPrinterBWND);
            this.tabPageOptions.Controls.Add(this.label1);
            this.tabPageOptions.Location = new System.Drawing.Point(4, 22);
            this.tabPageOptions.Name = "tabPageOptions";
            this.tabPageOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageOptions.Size = new System.Drawing.Size(738, 356);
            this.tabPageOptions.TabIndex = 1;
            this.tabPageOptions.Text = "Options";
            this.tabPageOptions.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.txtLog);
            this.groupBox2.Location = new System.Drawing.Point(8, 46);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(722, 307);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Processing Log";
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(13, 30);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(703, 271);
            this.txtLog.TabIndex = 1;
            this.txtLog.Text = "-------- Processing Log --------\r\n";
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(130, 6);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(116, 34);
            this.btnStop.TabIndex = 5;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(8, 6);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(116, 34);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // txtJobTimeout
            // 
            this.txtJobTimeout.Location = new System.Drawing.Point(183, 197);
            this.txtJobTimeout.Name = "txtJobTimeout";
            this.txtJobTimeout.Size = new System.Drawing.Size(49, 20);
            this.txtJobTimeout.TabIndex = 26;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 197);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 26);
            this.label5.TabIndex = 25;
            this.label5.Text = "Job Processing\r\nTimeout (seconds)";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(648, 5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(87, 24);
            this.btnSave.TabIndex = 24;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtPdfProgramArgs
            // 
            this.txtPdfProgramArgs.Location = new System.Drawing.Point(183, 165);
            this.txtPdfProgramArgs.Name = "txtPdfProgramArgs";
            this.txtPdfProgramArgs.Size = new System.Drawing.Size(319, 20);
            this.txtPdfProgramArgs.TabIndex = 23;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 168);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(94, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "PDF Program Args";
            // 
            // btnPdfProgBrowse
            // 
            this.btnPdfProgBrowse.Location = new System.Drawing.Point(519, 139);
            this.btnPdfProgBrowse.Name = "btnPdfProgBrowse";
            this.btnPdfProgBrowse.Size = new System.Drawing.Size(87, 20);
            this.btnPdfProgBrowse.TabIndex = 21;
            this.btnPdfProgBrowse.Text = "Browse...";
            this.btnPdfProgBrowse.UseVisualStyleBackColor = true;
            this.btnPdfProgBrowse.Click += new System.EventHandler(this.btnPdfProgBrowse_Click);
            // 
            // txtPdfProgram
            // 
            this.txtPdfProgram.Location = new System.Drawing.Point(183, 139);
            this.txtPdfProgram.Name = "txtPdfProgram";
            this.txtPdfProgram.Size = new System.Drawing.Size(318, 20);
            this.txtPdfProgram.TabIndex = 20;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 142);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "PDF Program Path";
            // 
            // btnOutputFolderBrowse
            // 
            this.btnOutputFolderBrowse.Location = new System.Drawing.Point(519, 114);
            this.btnOutputFolderBrowse.Name = "btnOutputFolderBrowse";
            this.btnOutputFolderBrowse.Size = new System.Drawing.Size(87, 20);
            this.btnOutputFolderBrowse.TabIndex = 18;
            this.btnOutputFolderBrowse.Text = "Browse...";
            this.btnOutputFolderBrowse.UseVisualStyleBackColor = true;
            this.btnOutputFolderBrowse.Click += new System.EventHandler(this.btnOutputFolderBrowse_Click);
            // 
            // txtOutputFolder
            // 
            this.txtOutputFolder.Location = new System.Drawing.Point(183, 114);
            this.txtOutputFolder.Name = "txtOutputFolder";
            this.txtOutputFolder.Size = new System.Drawing.Size(319, 20);
            this.txtOutputFolder.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Output Folder";
            // 
            // cmbOutputPrinterBWND
            // 
            this.cmbOutputPrinterBWND.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOutputPrinterBWND.FormattingEnabled = true;
            this.cmbOutputPrinterBWND.Location = new System.Drawing.Point(182, 8);
            this.cmbOutputPrinterBWND.Name = "cmbOutputPrinterBWND";
            this.cmbOutputPrinterBWND.Size = new System.Drawing.Size(319, 21);
            this.cmbOutputPrinterBWND.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(168, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Printer (Non-Duplex/Black&&White)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 38);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(145, 13);
            this.label6.TabIndex = 28;
            this.label6.Text = "Printer (Duplex/Black&&White)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 89);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(108, 13);
            this.label7.TabIndex = 30;
            this.label7.Text = "Printer (Duplex/Color)";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 64);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(131, 13);
            this.label8.TabIndex = 29;
            this.label8.Text = "Printer (Non-Duplex/Color)";
            // 
            // chkStartServer
            // 
            this.chkStartServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkStartServer.AutoSize = true;
            this.chkStartServer.Location = new System.Drawing.Point(546, 16);
            this.chkStartServer.Name = "chkStartServer";
            this.chkStartServer.Size = new System.Drawing.Size(184, 17);
            this.chkStartServer.TabIndex = 28;
            this.chkStartServer.Text = "Start server upon application start";
            this.chkStartServer.UseVisualStyleBackColor = true;
            // 
            // cmbOutputPrinterBWD
            // 
            this.cmbOutputPrinterBWD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOutputPrinterBWD.FormattingEnabled = true;
            this.cmbOutputPrinterBWD.Location = new System.Drawing.Point(182, 35);
            this.cmbOutputPrinterBWD.Name = "cmbOutputPrinterBWD";
            this.cmbOutputPrinterBWD.Size = new System.Drawing.Size(319, 21);
            this.cmbOutputPrinterBWD.TabIndex = 31;
            // 
            // cmbOutputPrinterCOLND
            // 
            this.cmbOutputPrinterCOLND.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOutputPrinterCOLND.FormattingEnabled = true;
            this.cmbOutputPrinterCOLND.Location = new System.Drawing.Point(182, 61);
            this.cmbOutputPrinterCOLND.Name = "cmbOutputPrinterCOLND";
            this.cmbOutputPrinterCOLND.Size = new System.Drawing.Size(319, 21);
            this.cmbOutputPrinterCOLND.TabIndex = 32;
            // 
            // cmbOutputPrinterCOLD
            // 
            this.cmbOutputPrinterCOLD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOutputPrinterCOLD.FormattingEnabled = true;
            this.cmbOutputPrinterCOLD.Location = new System.Drawing.Point(182, 86);
            this.cmbOutputPrinterCOLD.Name = "cmbOutputPrinterCOLD";
            this.cmbOutputPrinterCOLD.Size = new System.Drawing.Size(319, 21);
            this.cmbOutputPrinterCOLD.TabIndex = 33;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(746, 382);
            this.Controls.Add(this.tcMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(2048, 2048);
            this.MinimumSize = new System.Drawing.Size(762, 420);
            this.Name = "MainForm";
            this.Text = "TouchPad2PC Printer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.tcMain.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.tabPageGeneral.PerformLayout();
            this.tabPageOptions.ResumeLayout(false);
            this.tabPageOptions.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog dlgOutputFolder;
        private System.Windows.Forms.OpenFileDialog dlgPdfProgram;
        private System.Windows.Forms.NotifyIcon sysTrayBalloon;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tabPageGeneral;
        private System.Windows.Forms.CheckBox chkStartServer;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.TabPage tabPageOptions;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtJobTimeout;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtPdfProgramArgs;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnPdfProgBrowse;
        private System.Windows.Forms.TextBox txtPdfProgram;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnOutputFolderBrowse;
        private System.Windows.Forms.TextBox txtOutputFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbOutputPrinterBWND;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbOutputPrinterCOLD;
        private System.Windows.Forms.ComboBox cmbOutputPrinterCOLND;
        private System.Windows.Forms.ComboBox cmbOutputPrinterBWD;
    }
}

