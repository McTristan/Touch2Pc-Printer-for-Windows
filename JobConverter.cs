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
using System.Diagnostics;
using System.IO;

namespace Touch2PcPrinter
{
    internal enum JobConversionKind
    {
        PostScript,
        Pdf
    }

    internal class JobConverter : IPrintServerComponent
    {
        private readonly string programFolder;
        private readonly string pdfOutputFolder;
        private readonly Action<string> logger;

        private const string EXE_NAME = "pcl6.exe";
        private const string ARGUMENTS = "-sDEVICE={0} -dNOPAUSE -dBATCH -r600 -sOutputFile=\"{1}\" \"{2}\"";

        public JobConverter(Action<string> logger, string programFolder, string pdfOutputFolder)
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

        private static string retrieveJobExtension(JobConversionKind kind)
        {
            switch (kind)
            {
                case JobConversionKind.Pdf:
                    return "pdf";
                case JobConversionKind.PostScript:
                    return "ps";
                default:
                    throw new Exception("Unknown job conversion kind: " + kind);
            }
        }

        private static string retrieveJobDevice(JobConversionKind kind)
        {
            switch (kind)
            {
                case JobConversionKind.Pdf:
                    return "pdfwrite";
                case JobConversionKind.PostScript:
                    return "pswrite";
                default:
                    throw new Exception("Unknown job conversion kind: " + kind);
            }
        }

        public string Convert(string pclFilePath, JobConversionKind conversionKind)
        {
            if (pclFilePath == null)
            {
                throw new ArgumentNullException("pclFilePath");
            }
            var p = new Process();
            var si = p.StartInfo;
            si.FileName = Path.Combine(this.programFolder, JobConverter.EXE_NAME);

            string outputFile = Path.Combine(this.pdfOutputFolder, Path.ChangeExtension(Path.GetFileName(pclFilePath), JobConverter.retrieveJobExtension(conversionKind)));
            si.UseShellExecute = false;
            si.CreateNoWindow = true;
            si.Arguments = String.Format(JobConverter.ARGUMENTS, JobConverter.retrieveJobDevice(conversionKind), JobConverter.backwards2ForwardSlashes(outputFile), JobConverter.backwards2ForwardSlashes(pclFilePath));
            this.logger.Invoke(String.Format("Starting pcl6.exe process for conversion to {0}...", conversionKind));
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
