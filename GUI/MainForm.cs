/* Copyright 2011 Corey Bonnell

   This file is part of Touch2PcPrinter for Windows.

    Touch2PcPrinter for Windows is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Touch2PcPrinter for Windows is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Touch2PcPrinter for Windows.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Threading;
using System.Windows.Forms;

namespace Touch2PcPrinter
{
    public partial class MainForm : Form
    {
        private readonly Configuration config = new Configuration();

        private CancellationTokenSource cancelTokenSource = null;
        private Server currentServer;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.txtOutputFolder.Text = this.config.OutputFolder;
            this.txtPdfProgram.Text = this.config.PdfProgramPath;
            this.txtPdfProgramArgs.Text = this.config.PdfProgramArgs;
            this.txtJobTimeout.Text = this.config.JobTimeoutSeconds.ToString();
            this.chkStartServer.Checked = this.config.StartServerOnOpen;

            var printerNames = PrinterUtilities.RetrievePrinterNames();
            for (int i = 0; i < printerNames.Length; i++)
            {
                this.cmbOutputPrinterBWND.Items.Add( printerNames[i].Name);                
                this.cmbOutputPrinterBWD.Items.Add(printerNames[i].Name);
                
                /*if (string.Compare(config.OutputPrinterBWD, printerNames[i].Name, true) == 0)
                    cmbOutputPrinterBWD.SelectedIndex = i;*/

                this.cmbOutputPrinterCOLND.Items.Add(printerNames[i].Name);
                this.cmbOutputPrinterCOLD.Items.Add(printerNames[i].Name);
            }
            cmbOutputPrinterBWD.SelectedItem = config.OutputPrinterBWD;
            cmbOutputPrinterBWND.SelectedItem = config.OutputPrinterBWND;
            cmbOutputPrinterCOLD.SelectedItem = config.OutputPrinterCOLD;
            cmbOutputPrinterCOLND.SelectedItem = config.OutputPrinterCOLND;
                        
            if (this.cmbOutputPrinterBWD.SelectedIndex < 0)
            {
                this.cmbOutputPrinterBWD.SelectedIndex = 0;
            }
            if (this.cmbOutputPrinterBWND.SelectedIndex < 0)
            {
                this.cmbOutputPrinterBWND.SelectedIndex = 0;
            }
            if (this.cmbOutputPrinterCOLD.SelectedIndex < 0)
            {
                this.cmbOutputPrinterCOLD.SelectedIndex = 0;
            }
            if (this.cmbOutputPrinterCOLND.SelectedIndex < 0)
            {
                this.cmbOutputPrinterCOLND.SelectedIndex = 0;
            }
            if (this.config.StartServerOnOpen)
            {
                this.enableStopDisableStartButtons();
                this.startServer();
            }

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            this.enableStopDisableStartButtons();
            this.startServer();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            this.enableStartDisableStopButtons();
            this.stopServer();
        }

        private void btnOutputFolderBrowse_Click(object sender, EventArgs e)
        {
            if (this.dlgOutputFolder.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }
            
            this.txtOutputFolder.Text = this.dlgOutputFolder.SelectedPath;
        }

        private void btnPdfProgBrowse_Click(object sender, EventArgs e)
        {
            this.dlgPdfProgram.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            if (this.dlgPdfProgram.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            this.txtPdfProgram.Text = this.dlgPdfProgram.FileName;
            this.config.PdfProgramPath = this.dlgPdfProgram.FileName;
        }

        private void cmbOutputPrinter_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.config.OutputPrinterBWND = this.cmbOutputPrinterBWND.SelectedText;
        }

        private void writeSettingsToConfig()
        {
            int timeoutSeconds;
            if (!Int32.TryParse(this.txtJobTimeout.Text, out timeoutSeconds))
            {
                MessageBox.Show("Enter a numeric value for the timeout.", "Invalid input for timeout", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (timeoutSeconds < 0)
            {
                MessageBox.Show("Enter a non-negative value for the timeout.", "Invalid input for timeout", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.config.OutputFolder = this.txtOutputFolder.Text;

            this.config.OutputPrinterBWD = cmbOutputPrinterBWD.SelectedItem.ToString();
            this.config.OutputPrinterBWND = cmbOutputPrinterBWND.SelectedItem.ToString();
            this.config.OutputPrinterCOLD = cmbOutputPrinterCOLD.SelectedItem.ToString();
            this.config.OutputPrinterCOLND = cmbOutputPrinterCOLND.SelectedItem.ToString();

            this.config.PdfProgramPath = this.txtPdfProgram.Text;
            this.config.PdfProgramArgs = this.txtPdfProgramArgs.Text;
            this.config.JobTimeoutSeconds = timeoutSeconds;
            this.config.StartServerOnOpen = this.chkStartServer.Checked;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.writeSettingsToConfig();

            this.config.Save();

            if (this.currentServer != null)
            {
                if (MessageBox.Show("The print server is currently running. Would you like to stop it to use the new settings?", "Server is running", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
                this.enableStartDisableStopButtons();
                this.stopServer();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.currentServer != null)
            {
                if (MessageBox.Show(this, "The print server is still running. Are you sure want to quit?", "Server still running", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
                this.stopServer();
            }

            this.writeSettingsToConfig();

            this.config.Save();
        }

        private void startServer()
        {
            this.writeSettingsToConfig();

            Action<string> logger = (message) =>
            {
                MethodInvoker action = () => { this.txtLog.AppendText(String.Format("{0}{1}: {2}", Environment.NewLine, DateTime.Now, message)); };
                if (this.InvokeRequired)
                {
                    this.Invoke(action);
                }
                else
                {
                    action.Invoke();
                }
            };
            this.cancelTokenSource = new CancellationTokenSource();
            this.currentServer = new Server(logger, this.cancelTokenSource.Token, this.config);
            this.currentServer.Start();
        }

        private void enableStartDisableStopButtons()
        {
            this.btnStop.Enabled = false;
            this.btnStart.Enabled = true;
        }

        private void enableStopDisableStartButtons()
        {
            this.btnStart.Enabled = false;
            this.btnStop.Enabled = true;
        }

        private void stopServer()
        {
            try
            {
                this.cancelTokenSource.Cancel();
            }
            catch (OperationCanceledException) { }
            this.currentServer.Stop();
            this.currentServer.Dispose();
            this.cancelTokenSource.Dispose();
            this.currentServer = null;
        }

        private void sendToSysTray()
        {
            this.sysTrayBalloon.Visible = true;
            this.Hide();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.sendToSysTray();
            }
        }

        private void sysTrayBalloon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.sysTrayBalloon.Visible = false;
        }
    }
}
