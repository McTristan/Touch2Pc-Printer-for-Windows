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
using System.Threading;
using System.Runtime.InteropServices;
using System.Text;

namespace Touch2PcPrinter
{
    internal class PostScriptPrinter : IPrintServerComponent
    {
        private static readonly string[] PROGRAM_ARGUMENTS = new string[] { "gswin32c.exe", "-q", "-dQUIET", "-dBATCH", "-dNOPAUSE", "{0}", "{0}" };
        private const int STATIC_ARGS_COUNT = 5;
        private const string OUTPUT_PRINTER_TOKEN = "!DESTINATION_PRINTER_NAME!";


        private readonly Action<string> logger;
        private readonly string programFolder;
        private readonly CancellationToken cancelToken;

        public PostScriptPrinter(Action<string> logger, string programFolder, CancellationToken cancelToken)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            if (programFolder == null)
            {
                throw new ArgumentNullException("programFolder");
            }
            this.logger = logger;
            this.programFolder = programFolder;
            this.cancelToken = cancelToken;
        }

        public void Start() { }

        public void Stop() { }

        public void Dispose() { }

        private string writeSetupFile(string outputPrinter)
        {
            if (outputPrinter == null)
            {
                throw new ArgumentNullException("printerName");
            }
            string fileContent = Encoding.UTF8.GetString(Properties.Resources.setupPostScript);
            fileContent = fileContent.Replace(PostScriptPrinter.OUTPUT_PRINTER_TOKEN, outputPrinter);
            string path = Path.GetTempFileName();
            File.WriteAllText(path, fileContent);
            this.logger.Invoke(String.Format("Wrote job setup file to \"{0}\"", path));
            return path;
        }

        private string[] createArgs(string psFilePath, string outputPrinter, string setupFilePath)
        {
            if (psFilePath == null)
            {
                throw new ArgumentNullException("psFilePath");
            }
            if (outputPrinter == null)
            {
                throw new ArgumentNullException("printerName");
            }
            if (setupFilePath == null)
            {
                throw new ArgumentNullException("setupFilePath");
            }
            var args = new string[PostScriptPrinter.PROGRAM_ARGUMENTS.Length];
            Array.Copy(PostScriptPrinter.PROGRAM_ARGUMENTS, args, PostScriptPrinter.STATIC_ARGS_COUNT);
            args[PostScriptPrinter.STATIC_ARGS_COUNT] = String.Format(PostScriptPrinter.PROGRAM_ARGUMENTS[PostScriptPrinter.STATIC_ARGS_COUNT], setupFilePath);
            args[PostScriptPrinter.STATIC_ARGS_COUNT + 1] = String.Format(PostScriptPrinter.PROGRAM_ARGUMENTS[PostScriptPrinter.STATIC_ARGS_COUNT + 1], psFilePath);
            return args;
        }

        public void Print(string psFilePath, string outputPrinter)
        {

            if (psFilePath == null)
            {
                throw new ArgumentNullException("psFilePath");
            }
            if (outputPrinter == null)
            {
                throw new ArgumentNullException("printerName");
            }
            string setupFilePath = null;
            try // try/finally block for setup file
            {
                setupFilePath = this.writeSetupFile(outputPrinter);
                var args = this.createArgs(psFilePath, outputPrinter, setupFilePath);
                var hInstance = IntPtr.Zero;

                try // try/finally block for GhostScript handle
                {
                    int errorCode = NativeMethods.gsapi_new_instance(ref hInstance, IntPtr.Zero);
                    if (errorCode < 0)
                    {
                        throw new Exception(String.Format("gsapi_new_instance returned error code {0}", errorCode));
                    }
                    errorCode = NativeMethods.gsapi_init_with_args(hInstance, args.Length, args);
                    if (errorCode < 0)
                    {
                        throw new Exception(String.Format("gsapi_init_with_args returned error code {0}", errorCode));
                    }
                    NativeMethods.gsapi_exit(hInstance);
                    this.logger.Invoke("GhostScript completed processing successfully");
                }
                finally
                {
                    if (hInstance != null)
                    {
                        NativeMethods.gsapi_delete_instance(hInstance);
                    }
                }
            }
            finally
            {
                if (setupFilePath != null)
                {
                    File.Delete(setupFilePath);
                    this.logger.Invoke("Deleted job setup file");
                }
            }
        }

        private static class NativeMethods
        {
            [DllImport("gsdll32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "gsapi_new_instance")]
            public static extern int gsapi_new_instance(ref IntPtr hInstance, IntPtr callingHandle);

            [DllImport("gsdll32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "gsapi_delete_instance")]
            public static extern int gsapi_delete_instance(IntPtr hInstance);

            [DllImport("gsdll32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "gsapi_init_with_args")]
            public static extern int gsapi_init_with_args(IntPtr hInstance, int argc, string[] argv);

            [DllImport("gsdll32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "gsapi_exit")]
            public static extern int gsapi_exit(IntPtr hInstance);
        }


    }
}
