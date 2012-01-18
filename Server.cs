/* Copyright 2011 Corey Bonnell and Sandro Lange

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
using System.Collections.Generic;

namespace Touch2PcPrinter
{
    internal class Server : IPrintServerComponent
    {
        private readonly CancellationToken cancelToken;
        private readonly Configuration config;
        private readonly Action<string> logger;
        private readonly JobConverter jobConverter;
        private readonly PrintJobReader printJobReader;
        private readonly SnmpPrinterAgent snmpAgent;
        private readonly PostScriptPrinter postScriptPrinter;
        private readonly JobConversionKind conversionKind;

        private readonly Dictionary<PrintJobProperties, string> propertyMappings = new Dictionary<PrintJobProperties, string>();

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
            this.jobConverter = new JobConverter(this.logger, programFolder, this.config.OutputFolder);
            this.printJobReader = new PrintJobReader(this.logger, this.cancelToken);
            this.snmpAgent = new SnmpPrinterAgent(this.logger);
            if (!this.config.VirtualOnly)
            {
                this.postScriptPrinter = new PostScriptPrinter(this.logger, programFolder, this.cancelToken);
                this.conversionKind = JobConversionKind.PostScript;
            }
            else
            {
                this.conversionKind = JobConversionKind.Pdf;
            }
            this.createJobPropertyMappings();
        }

        private void createJobPropertyMappings()
        {
            this.propertyMappings.Add(new PrintJobProperties(ColorMode.BlackAndWhite, PlexMode.Simplex), this.config.OutputPrinterBlackWhiteSimplex);
            this.propertyMappings.Add(new PrintJobProperties(ColorMode.BlackAndWhite, PlexMode.Duplex), this.config.OutputPrinterBlackWhiteDuplex);
            this.propertyMappings.Add(new PrintJobProperties(ColorMode.Color, PlexMode.Simplex), this.config.OutputPrinterColorSimplex);
            this.propertyMappings.Add(new PrintJobProperties(ColorMode.Color, PlexMode.Duplex), this.config.OutputPrinterColorDuplex);
        }

        private void logConfiguration()
        {
            this.logger.Invoke(String.Format("Output folder: {0}", this.config.OutputFolder));
            this.logger.Invoke(String.Format("Output to PDF file only: {0}", this.config.VirtualOnly));
            if (!this.config.VirtualOnly)
            {
                this.logger.Invoke(String.Format("Output simplex black & white printer: {0}", this.config.OutputPrinterBlackWhiteSimplex));
                this.logger.Invoke(String.Format("Output duplex black & white printer: {0}", this.config.OutputPrinterBlackWhiteDuplex));
                this.logger.Invoke(String.Format("Output simplex color printer: {0}", this.config.OutputPrinterColorSimplex));
                this.logger.Invoke(String.Format("Output duplex color printer: {0}", this.config.OutputPrinterColorDuplex));
            }
        }

        public void Start()
        {
            if(!Directory.Exists(this.config.OutputFolder))
            {
                Directory.CreateDirectory(this.config.OutputFolder);
            }

            this.logConfiguration();

            if (this.postScriptPrinter != null)
            {
                this.postScriptPrinter.Start();
            }
            this.jobConverter.Start();
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
            if (this.postScriptPrinter != null)
            {
                this.postScriptPrinter.Stop();
            }
            this.jobConverter.Stop();
            this.printJobReader.Stop();
            this.logger.Invoke("Print job listener stopped successfully");
            this.snmpAgent.Stop();
            this.logger.Invoke("SNMP Agent stopped successfully");
        }

        public void Dispose()
        {
            if (this.postScriptPrinter != null)
            {
                this.postScriptPrinter.Dispose();
            }
            this.jobConverter.Dispose();
            this.printJobReader.Dispose();
            this.snmpAgent.Dispose();
        }

        private void runServer()
        {
            while (!this.cancelToken.IsCancellationRequested)
            {
                string tempOutputFilePath;
                string tempPclPath;
                try
                {
                    this.logger.Invoke("Waiting for incoming job...");
                    tempPclPath = this.printJobReader.ReadJobToFile();
                    var jobProperties = PrintJobReader.GetProperties(tempPclPath);
                    this.logger.Invoke(String.Format("Converting PCL file to {0}...", this.conversionKind));
                    try
                    {
                        tempOutputFilePath = jobConverter.Convert(tempPclPath, this.conversionKind);
                    }
                    finally
                    {
                        this.logger.Invoke("Deleting temporary PCL file...");
                        File.Delete(tempPclPath);
                    }
                    if (this.postScriptPrinter == null)
                    {
                        continue;
                    }
                    // if we're here, then the user selected a printer to physically print to
                    this.logger.Invoke(String.Format("Color mode for job: {0}", jobProperties.ColorMode));
                    this.logger.Invoke(String.Format("Duplex/simplex mode for job: {0}", jobProperties.PlexMode));
                    string outputPrinterName;
                    if (!this.propertyMappings.TryGetValue(jobProperties, out outputPrinterName))
                    {
                        throw new InvalidOperationException("Unsupported print job properties");
                    }

                    this.logger.Invoke("Sending PostScript file to be printed...");
                    try
                    {
                        if (outputPrinterName == null)
                        {
                            this.logger.Invoke("ERROR: No printer selected for the print job's properties");
                        }
                        else
                        {
                            this.postScriptPrinter.Print(tempOutputFilePath, outputPrinterName);
                            this.logger.Invoke("Print job complete!");
                        }
                    }
                    finally
                    {
                        this.logger.Invoke("Deleting temporary PostScript file...");
                        File.Delete(tempOutputFilePath);
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    this.logger.Invoke(String.Format("EXCEPTION OCCURRED: {0}", e));
                }
            }
        }
    }
}
