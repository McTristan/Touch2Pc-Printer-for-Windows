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

namespace Touch2PcPrinter
{
    internal class PdfConverter : IPrintServerComponent
    {
        private readonly string programFolder;
        private readonly string pdfOutputFolder;
        private readonly Action<string> logger;

        private const string EXE_NAME = "pcl6.exe";
        private const string ARGUMENTS = "-sDEVICE=pdfwrite -dNOPAUSE -sOutputFile=\"{0}\" \"{1}\"";

        public PdfConverter(Action<string> logger, string programFolder, string pdfOutputFolder)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            if (programFolder == null)
            {
                throw new ArgumentNullException("programFolder");
            }
            if (pdfOutputFolder == null)
            {
                throw new ArgumentNullException("pdfOutputFolder");
            }
            this.logger = logger;
            this.programFolder = programFolder;
            this.pdfOutputFolder = pdfOutputFolder;
        }

        public void Start() { }

        public void Stop() { }

        public string ConvertToPdf(string pclFilePath)
        {
            if (pclFilePath == null)
            {
                throw new ArgumentNullException("pclFilePath");
            }
            var p = new Process();
            var si = p.StartInfo;
            si.FileName = Path.Combine(this.programFolder, PdfConverter.EXE_NAME);

            string outputFile = Path.Combine(this.pdfOutputFolder, Path.ChangeExtension(Path.GetFileName(pclFilePath), "pdf"));
            si.UseShellExecute = false;
            si.CreateNoWindow = true;
            si.Arguments = String.Format(PdfConverter.ARGUMENTS, PdfConverter.backwards2ForwardSlashes(outputFile), PdfConverter.backwards2ForwardSlashes(pclFilePath));
            this.logger.Invoke("Starting pcl6.exe process for conversion to PDF...");
            p.Start();
            p.WaitForExit();
            if (p.ExitCode != 0)
            {
                throw new Exception(String.Format("pcl6.exe returned with abnormal return code {0}", p.ExitCode));
            }
            this.logger.Invoke("pcl6.exe exited normally");
            return outputFile;
        }

        public void Dispose() { }

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
