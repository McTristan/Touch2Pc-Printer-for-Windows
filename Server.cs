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
using System.IO;
using System.Reflection;
using System.Threading;

namespace Touch2PcPrinter
{
    internal class Server : IPrintServerComponent
    {
        private readonly CancellationToken cancelToken;
        private readonly Configuration config;
        private readonly Action<string> logger;
        private readonly PdfConverter pdfConverter;
        private readonly PrintJobReader printJobReader;
        private readonly SnmpPrinterAgent snmpAgent;
        private readonly PdfPrinter pdfPrinter;

        public Server(Action<string> logger, CancellationToken cancelToken, Configuration config)
        {
            if(logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            this.logger = logger;
            this.cancelToken = cancelToken;
            this.config = config;

            string programFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            this.pdfConverter = new PdfConverter(this.logger, programFolder, this.config.OutputFolder);
            this.printJobReader = new PrintJobReader(this.logger, this.cancelToken);
            this.snmpAgent = new SnmpPrinterAgent(this.logger);
            if (!String.Equals(this.config.OutputPrinterBWND, PrinterUtilities.VirtualPdfPrinterName, StringComparison.InvariantCultureIgnoreCase))
            {
                this.pdfPrinter = new PdfPrinter(this.logger, this.config.PdfProgramPath, this.config.PdfProgramArgs, this.config.JobTimeoutSeconds, this.cancelToken);
            }
        }

        private void logConfiguration()
        {
            this.logger.Invoke(String.Format("Output folder: {0}", this.config.OutputFolder));
            this.logger.Invoke(String.Format("Output printer: {0}", this.config.OutputPrinterBWND));
            this.logger.Invoke(String.Format("PDF program path: {0}", this.config.PdfProgramPath));
            this.logger.Invoke(String.Format("PDF program arguments: {0}", this.config.PdfProgramArgs));
            this.logger.Invoke(String.Format("Job timeout: {0} second(s)", this.config.JobTimeoutSeconds));
        }

        public void Start()
        {
            if(!Directory.Exists(this.config.OutputFolder))
            {
                Directory.CreateDirectory(this.config.OutputFolder);
            }

            this.logConfiguration();

            if (this.pdfPrinter != null)
            {
                this.pdfPrinter.Start();
            }
            this.pdfConverter.Start();
            this.printJobReader.Start();
            this.logger.Invoke("Print job listener started successfully");
            this.snmpAgent.Start();
            this.logger.Invoke("SNMP Agent started successfully");
            var serviceThread = new Thread(this.runServer);
            serviceThread.IsBackground = true;
            serviceThread.Start();
        }

        public void Stop()
        {
            if (this.pdfPrinter != null)
            {
                this.pdfPrinter.Stop();
            }
            this.pdfConverter.Stop();
            this.printJobReader.Stop();
            this.logger.Invoke("Print job listener stopped successfully");
            this.snmpAgent.Stop();
            this.logger.Invoke("SNMP Agent stopped successfully");
        }

        public void Dispose()
        {
            if (this.pdfPrinter != null)
            {
                this.pdfPrinter.Dispose();
            }
            this.pdfConverter.Dispose();
            this.printJobReader.Dispose();
            this.snmpAgent.Dispose();
        }

        private void runServer()
        {
            while (!this.cancelToken.IsCancellationRequested)
            {
                string tempPdfPath;
                string tempPclPath;
                try
                {
                    this.logger.Invoke("Waiting for incoming job...");
                    tempPclPath = this.printJobReader.GetJob();
                    this.logger.Invoke("Converting PCL file to PDF...");
                    try
                    {                        
                        tempPdfPath = pdfConverter.ConvertToPdf(tempPclPath);
                    }
                    finally
                    {
                        this.logger.Invoke("Deleting temporary PCL file...");
                        File.Delete(tempPclPath);
                    }
                    if (String.Equals(this.config.OutputPrinterBWND, PrinterUtilities.VirtualPdfPrinterName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }
                    // if we're here, then the user selected a printer to physically print to
                    this.logger.Invoke("Sending PDF file to be printed...");
                    try
                    {
                        string sPrinter = config.OutputPrinterBWND;

                        switch (printJobReader.ColorMode)
                        {
                            case eColorMode.BlackWhite:
                                if (printJobReader.DuplexMode == eDuplexMode.Duplex)
                                    sPrinter = config.OutputPrinterBWD;
                                else
                                    sPrinter = config.OutputPrinterBWND;
                                break;
                            case eColorMode.Color:
                                if (printJobReader.DuplexMode == eDuplexMode.Duplex)
                                    sPrinter = config.OutputPrinterCOLD;
                                else
                                    sPrinter = config.OutputPrinterCOLND;
                                break;
                        }
                        this.pdfPrinter.Print(tempPdfPath, sPrinter);
                        this.logger.Invoke("Print job complete!");
                    }
                    finally
                    {
                        this.logger.Invoke("Deleting temporary PDF file...");
                        File.Delete(tempPdfPath);
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    this.logger.Invoke(String.Format("EXCEPTION OCCURRED: {0}", e));
                }
                finally
                {

                }
            }
        }
    }
}
