using System;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Configuration;

namespace Touch2PcPrinter
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) =>
            {
                var eventException = e.ExceptionObject as Exception ?? new Exception("Unknown exception object");
                Trace.WriteLine(String.Format("UNHANDLED EXCEPTION: {0}", (e.ExceptionObject as Exception).Message));
            };

            //sla 19.09.2011 - no longer required as there is now an AppConfig-Setting
            /*if (args.Length < 1)
            {
                Console.Error.WriteLine("usage: Touch2PcPrinter.exe [Acrobat Reader Path (AcroRd32.exe)]");
                return 1;
            }*/
            //..sla 19.09.2011 - no longer required as there is now an AppConfig-Setting
                  
            //sla 19.09.2011 - no longer required as there is now an AppConfig-Setting
            //string acroRd32Path = args[0];
            string acroRd32Path = ConfigurationManager.AppSettings["AdobeReaderPath"];
            if (!File.Exists(acroRd32Path))
            {
                Console.Error.WriteLine("Please specify the path to Acrobat Reader Path in the corresponding .cfg file of this application.");
                return 1;
            }
            //..sla 19.09.2011 - no longer required as there is now an AppConfig-Setting

            string programFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //sla 20.09.2011 - configurable output folder, issue #3
            string outputFolder = ConfigurationManager.AppSettings["OutputPath"];            
            if (string.IsNullOrEmpty(outputFolder)) // if none is given, fall back to the old scheme
                outputFolder = Path.Combine(programFolder, "output");
            //..sla 20.09.2011 - configurable output folder, issue #3
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            //sla 19.09.2011 - CleanUp files
            else
            {
                //sla 20.09.2011 - CleanUp should be configurable - issue #4
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["CleanUpFiles"]))
                {
                    string[] sFiles = Directory.GetFiles(outputFolder);
                    foreach (string sFile in sFiles)
                    {
                        try
                        {
                            File.Delete(sFile);
                        }
                        catch
                        {
                        }
                    }
                }
            }
            //..sla 19.09.2011

            var snmpAgent = new SnmpPrinterAgent();            
            var printJobServer = new PrintJobReader(outputFolder);
            var pdfConverter = new PdfConverter(programFolder);
            var acroPrinter = new AcroPrinter(acroRd32Path);

            //sla 20.09.2011 PDF-only output - issue #2
            bool bOnlyOutputToPDF = Convert.ToBoolean(ConfigurationManager.AppSettings["OnlyOutputPDF"]);
            
            //sla 20.09.2011 Configurable output printer - issue #1
            string sPrinterName = ConfigurationManager.AppSettings["PrinterName"];

            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.TraceInformation("Starting print daemon...");

            snmpAgent.Start();
            Trace.TraceInformation("SNMP Agent started successfully");
            printJobServer.Start();
            Trace.TraceInformation("Print job listener started successfully");
            
            while (true)
            {
                try
                {
                    Trace.TraceInformation("Waiting for incoming job...");
                    string tempPclPath = printJobServer.GetJob();
                    Trace.TraceInformation("Converting PCL file to PDF...");
                    string tempPdfPath = pdfConverter.ConvertToPdf(tempPclPath);

                    //sla 20.09.2011 PDF-only output - issue #2
                    if (!bOnlyOutputToPDF)
                    {
                        Trace.TraceInformation("Sending PDF file to be printed to default printer...");
                        acroPrinter.Print(tempPdfPath, sPrinterName);
                    }
                    //..sla 20.09.2011 PDF-only output - issue #2
                    Trace.TraceInformation("Print job complete!");
                    File.Delete(tempPclPath);
                }
                catch (Exception e)
                {
                    Trace.TraceError("EXCEPTION OCCURRED: {0}", e.Message);
                }
                finally
                {
                    //sla 20.09.2011 - stop the engine if no longer needed - issue #6 - it is still not ideal
                    snmpAgent.Stop();
                }
            }

            
        }
    }
}
