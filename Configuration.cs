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
using System.Configuration;
using System.IO;

namespace Touch2PcPrinter
{
    internal class Configuration : ApplicationSettingsBase
    {
        private static readonly string DEFAULT_OUTPUT_DIR = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Touch2PcPrinter Output");
        private const string DEFAULT_START_SERVER = "false";
        private const string DEFAULT_VIRTUAL_PDF = "false";

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
        [DefaultSettingValue(Configuration.DEFAULT_VIRTUAL_PDF)]
        public bool VirtualOnly
        {
            get
            {
                return Convert.ToBoolean(this["VirtualOnly"]);
            }
            set
            {
                this["VirtualOnly"] = value;
            }
        }

        [UserScopedSetting()]
        public string OutputPrinterBlackWhiteSimplex
        {
            get
            {
                return this["OutputPrinterBlackWhiteSimplex"] as String;
            }
            set
            {
                this["OutputPrinterBlackWhiteSimplex"] = value;
            }
        }

        [UserScopedSetting()]
        public string OutputPrinterBlackWhiteDuplex
        {
            get
            {
                return this["OutputPrinterBlackWhiteDuplex"] as String;
            }
            set
            {
                this["OutputPrinterBlackWhiteDuplex"] = value;
            }
        }

        [UserScopedSetting()]
        public string OutputPrinterColorSimplex
        {
            get
            {
                return this["OutputPrinterColorSimplex"] as String;
            }
            set
            {
                this["OutputPrinterColorSimplex"] = value;
            }
        }

        [UserScopedSetting()]
        public string OutputPrinterColorDuplex
        {
            get
            {
                return this["OutputPrinterColorDuplex"] as String;
            }
            set
            {
                this["OutputPrinterColorDuplex"] = value;
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

    }
}
