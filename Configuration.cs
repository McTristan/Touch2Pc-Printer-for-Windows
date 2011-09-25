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
using System.Configuration;
using System.IO;

namespace Touch2PcPrinter
{
    internal class Configuration : ApplicationSettingsBase
    {
        private static readonly string DEFAULT_OUTPUT_DIR = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Touch2PcPrinter Output");
        private const string DEFAULT_PDF_PATH = "[PDF PROGRAM PATH REQUIRED]";
        private const string DEFAULT_PDF_ARGS = "/t \"{0}\" \"{1}\"";
        private const int DEFAULT_JOB_TIMEOUT = 120;
        private const string DEFAULT_START_SERVER = "false";

        [UserScopedSetting()]
        [DefaultSettingValue(Configuration.DEFAULT_START_SERVER)]
        public bool StartServerOnOpen
        {
            get
            {
                return Convert.ToBoolean(this["StartServerOnOpen"]);
            }
            set
            {
                this["StartServerOnOpen"] = value;
            }
        }

        [UserScopedSetting()]
        public string OutputPrinterBWD
        {
            get
            {
                return this["OutputPrinterBWD"] as String ?? PrinterUtilities.VirtualPdfPrinterName;
            }
            set
            {
                this["OutputPrinterBWD"] = value;
            }
        }

        [UserScopedSetting()]
        public string OutputPrinterBWND
        {
            get
            {
                return this["OutputPrinterBWND"] as String ?? PrinterUtilities.VirtualPdfPrinterName;
            }
            set
            {
                this["OutputPrinterBWND"] = value;
            }
        }

        [UserScopedSetting()]
        public string OutputPrinterCOLD
        {
            get
            {
                return this["OutputPrinterCOLD"] as String ?? PrinterUtilities.VirtualPdfPrinterName;
            }
            set
            {
                this["OutputPrinterCOLD"] = value;
            }
        }

        [UserScopedSetting()]
        public string OutputPrinterCOLND
        {
            get
            {
                return this["OutputPrinterCOLND"] as String ?? PrinterUtilities.VirtualPdfPrinterName;
            }
            set
            {
                this["OutputPrinterCOLND"] = value;
            }
        }

        [UserScopedSetting()]
        public string OutputFolder
        {
            get
            {
                return this["OutputFolder"] as String ?? Configuration.DEFAULT_OUTPUT_DIR;
            }
            set
            {
                this["OutputFolder"] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue(Configuration.DEFAULT_PDF_PATH)]
        public string PdfProgramPath
        {
            get
            {
                return this["PdfProgramPath"] as String;
            }
            set
            {
                this["PdfProgramPath"] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue(Configuration.DEFAULT_PDF_ARGS)]
        public string PdfProgramArgs
        {
            get
            {
                return this["PdfProgramArgs"] as String;
            }
            set
            {
                this["PdfProgramArgs"] = value;
            }
        }

        [UserScopedSetting()]
        public int JobTimeoutSeconds
        {
            get
            {
                var value = this["JobTimeoutSeconds"];
                return value != null ? Convert.ToInt32(value) : Configuration.DEFAULT_JOB_TIMEOUT;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this["JobTimeoutSeconds"] = value;
            }
        }

    }
}
