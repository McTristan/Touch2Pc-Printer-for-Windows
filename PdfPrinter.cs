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
using System.Diagnostics;
using System.IO;
using System.Threading;
using Atom8.API.PrintSpool;
using Atom8.Monitors;

namespace Touch2PcPrinter
{
    internal class PdfPrinter : IPrintServerComponent
    {   
        private readonly string pdfProgramPath;
        private readonly string pdfProgramArgs;
        private readonly Action<string> logger;
        private readonly int printTimeout;
        private readonly CancellationToken cancelToken;

        public PdfPrinter(Action<string> logger, string pdfProgramPath, string pdfProgramArgs, int printTimeout, CancellationToken cancelToken)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            if (pdfProgramPath == null)
            {
                throw new ArgumentNullException("pdfProgramPath");
            }
            if (pdfProgramArgs == null)
            {
                throw new ArgumentNullException("pdfProgramArgs");
            }
            
            if (printTimeout < 0)
            {
                throw new ArgumentOutOfRangeException("printTimeout");
            }
            this.logger = logger;
            this.pdfProgramPath = pdfProgramPath;
            this.pdfProgramArgs = pdfProgramArgs;         
            this.printTimeout = printTimeout;
            this.cancelToken = cancelToken;
        }

        public void Start() { }

        public void Stop() { }

        public void Dispose() { }

        public void Print(string pdfFilePath, string printerName)
        {
            if (pdfFilePath == null)
            {
                throw new ArgumentNullException("pdfFilePath");
            }
            PrintQueueMonitor printMonitor = null;
            PrintJobStatusChanged jobStatusChanged = null;
            try
            {
                bool jobCompleted = false;
                printMonitor = new PrintQueueMonitor(printerName);
                jobStatusChanged = (object sender, PrintJobChangeEventArgs e) =>
                {
                    if((int)(e.JobStatus & (JOBSTATUS.JOB_STATUS_PRINTING | JOBSTATUS.JOB_STATUS_PRINTED)) != 0)
                    {
                        jobCompleted = true;
                    }
                };
                printMonitor.OnJobStatusChange += jobStatusChanged;

                var p = new Process();
                var si = p.StartInfo;
                si.FileName = this.pdfProgramPath;
                si.Arguments = String.Format(this.pdfProgramArgs, pdfFilePath, printerName);
                si.UseShellExecute = false;

                this.logger.Invoke(String.Format("Starting \"{0}\" for printing to \"{1}\"...", Path.GetFileName(this.pdfProgramPath), printerName));

                p.Start();
                p.WaitForInputIdle();

                int elapsed = 0;
                while (elapsed < this.printTimeout && !jobCompleted)
                {
                    this.cancelToken.ThrowIfCancellationRequested();
                    try
                    {
                        Thread.Sleep(1000);
                    }
                    catch (ThreadInterruptedException) { }
                    elapsed += 1;
                }

                try // kind of messy, but Acrobat doesn't want to close after printing
                {
                    p.Kill();
                    p.WaitForExit();
                }
                catch (Exception) { } // if the process already exited, then exceptions are thrown

                if (!jobCompleted) // the job timed out
                {
                    throw new TimeoutException(String.Format("Print job wait of {0} seconds has timed out", this.printTimeout));
                }            
            }
            finally 
            {
                if(printMonitor != null)
                {
                    printMonitor.OnJobStatusChange -= jobStatusChanged;
                    printMonitor.Stop();
                }
            }
            

          
        }

    }
}
