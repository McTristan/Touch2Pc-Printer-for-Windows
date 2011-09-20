using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Touch2PcPrinter
{
    internal class PdfConverter
    {
        private readonly string programFolder;

        private const string EXE_NAME = "pcl6.exe";
        private const string ARGUMENTS = "-sDEVICE=pdfwrite -dNOPAUSE -sOutputFile=\"{0}\" \"{1}\"";

        public PdfConverter(string programFolder)
        {
            if (programFolder == null)
            {
                throw new ArgumentNullException("programFolder");
            }
            this.programFolder = programFolder;
        }

        public string ConvertToPdf(string pclFilePath)
        {
            if (pclFilePath == null)
            {
                throw new ArgumentNullException("pclFilePath");
            }
            var p = new Process();
            var si = p.StartInfo;
            si.FileName = Path.Combine(this.programFolder, PdfConverter.EXE_NAME);
            string outputFile = Path.ChangeExtension(pclFilePath, "pdf");
            si.UseShellExecute = false;
            si.CreateNoWindow = true;
            si.Arguments = String.Format(PdfConverter.ARGUMENTS, PdfConverter.backwards2ForwardSlashes(outputFile), PdfConverter.backwards2ForwardSlashes(pclFilePath));
            Trace.TraceInformation("Starting pcl6.exe process for conversion to PDF...");
            p.Start();
            p.WaitForExit();
            if (p.ExitCode != 0)
            {
                throw new Exception(String.Format("pcl6.exe returned with abnormal return code {0}", p.ExitCode));
            }
            Trace.TraceInformation("pcl6.exe exited normally");
            return outputFile;
        }

        // pcl6.exe expects forward-slashes as the path separator character
        private static string backwards2ForwardSlashes(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            return path.Replace('\\', '/');
        }

    }
}
