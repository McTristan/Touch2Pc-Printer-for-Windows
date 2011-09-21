using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing.Printing;
using Atom8.Monitors;
using System.Configuration;

namespace Touch2PcPrinter
{
    internal class AcroPrinter
    {
        private const string ARGUMENTS = "/n /t \"{0}\" \"{1}\"";
        
        private readonly string acroRd32Path;        

        private static volatile bool m_bCloseAcrobat = false;
        static AcroPrinter()        {            
           
        }            


        public AcroPrinter(string acroRd32Path)
        {
            if (string.IsNullOrEmpty(acroRd32Path))
            {
                throw new ArgumentNullException("acroRd32Path");
            }
            this.acroRd32Path = acroRd32Path;
        }

        public void Print(string pdfFilePath, string sPrinterName)
        {
            if (string.IsNullOrEmpty(sPrinterName))
            {
                //it's propably better to get the default printer with every new print job as it might have changed over time
                PrinterSettings defaultPrinter = new PrinterSettings();

                //sla 21.09.2011 - the following code shouldn't be needed but just in case - maybe it fixes issue #7
                if (string.IsNullOrEmpty(defaultPrinter.PrinterName))
                {
                    PrinterSettings settings = new PrinterSettings();
                    foreach (string printer in PrinterSettings.InstalledPrinters)
                    {
                        settings.PrinterName = printer;
                        if (settings.IsDefaultPrinter)
                            break;
                    }
                }
                sPrinterName = defaultPrinter.PrinterName;
            }
            //..sla 21.09.2011
                        
            if (pdfFilePath == null)
            {
                throw new ArgumentNullException("pdfFilePath");
            }
            var p = new Process();
            var si = p.StartInfo;
            //sla 19.09.2011 - new approach
            /*si.FileName = this.acroRd32Path;
            si.Arguments = String.Format(AcroPrinter.ARGUMENTS, pdfFilePath, AcroPrinter.defaultPrinter.PrinterName);
            si.UseShellExecute = false;
            si.CreateNoWindow = true;
            Trace.TraceInformation("Starting AcroRd32.exe for printing to default printer...");
            p.Start();
            
            // we can't wait for Adobe Reader to exit, since it stays open after printing...
            //
            //p.WaitForExit();
            //if(p.ExitCode != 0)
            //{
            //    throw new Exception(String.Format("AcroRd32.exe returned with abnormal return code {0}", p.ExitCode));
            //}
            //Trace.TraceInformation("AcroRd32.exe exited normally");
            */

            
            //sla 19.09.2011 - new approach to close the window after spooling is complete + redirecting the standard-output so it might run from a service later on
            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
             * The following Class PrintQueueMonitor was taken from an CodeProject-Project from Atom8 IT Solutions (P) Ltd which stands under the OPL-License (see http://www.codeproject.com/info/cpol10.aspx for licensing information)             * 
             * */
            PrintQueueMonitor pqm = new PrintQueueMonitor(sPrinterName); 
            
            pqm.OnJobStatusChange += new PrintJobStatusChanged(pqm_OnJobStatusChange);
            si.FileName = this.acroRd32Path;
            si.Verb = "printto";
            si.Arguments = String.Format(AcroPrinter.ARGUMENTS, pdfFilePath, sPrinterName);
            si.CreateNoWindow = true;
            si.RedirectStandardOutput = true;
            si.UseShellExecute = false;
            Trace.TraceInformation("Starting AcroRd32.exe for printing to default printer...");            
            p.Start();
                        
            p.CloseMainWindow();
            
            DateTime start = DateTime.Now;
            IntPtr handle = IntPtr.Zero;
            int nTimeOutInSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["PrintJobTimeOutInSeconds"]);
            while (handle == IntPtr.Zero && DateTime.Now - start <= TimeSpan.FromSeconds(nTimeOutInSeconds) && !m_bCloseAcrobat)
            {
                try
                {
                    System.Threading.Thread.Sleep(50);
                    handle = p.MainWindowHandle;
                }

                catch (Exception) { }
            }
            p.Kill();
          
            //..sla 19.09.2011 - new approach to close the window after printing is complete + redirecting the standard-output so it might run from a service later on
          
        }

        void pqm_OnJobStatusChange(object Sender, PrintJobChangeEventArgs e)
        {
            if (e.JobStatus == Atom8.API.PrintSpool.JOBSTATUS.JOB_STATUS_PRINTING) // the status is no longer spooling - we can safely close Acrobat Reader / alternatively we could check for "printed"
            {
                m_bCloseAcrobat = true;
            }
        }

    }
}
