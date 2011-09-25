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
using System.Collections.Generic;
using System.Drawing.Printing;

namespace Touch2PcPrinter
{
    internal static class PrinterUtilities
    {
        private const string VIRTUAL_PDF_PRINTER_NAME = "[None - Only Generate PDF]";

        public static string VirtualPdfPrinterName
        {
            get
            {
                return PrinterUtilities.VIRTUAL_PDF_PRINTER_NAME;
            }
        }

        public static Printer VirtualPdfPrinter
        {
            get
            {
                return new Printer(PrinterUtilities.VIRTUAL_PDF_PRINTER_NAME, false);
            }
        }

        public static Printer[] RetrievePrinterNames()
        {
            var printerEntries = new List<Printer>();
            printerEntries.Add(PrinterUtilities.VirtualPdfPrinter);
            var defaultPrinterName = new PrinterSettings().PrinterName;
            foreach (string printerName in PrinterSettings.InstalledPrinters)
            {
                bool isDefault = String.Equals(defaultPrinterName, printerName, StringComparison.InvariantCultureIgnoreCase);
                printerEntries.Add(new Printer(printerName, isDefault));
            }
            return printerEntries.ToArray();
        }
    }

    internal class Printer
    {
        public Printer(string name, bool isDefault)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            this.Name = name;
            this.IsDefault = isDefault;
        }

        public string Name { get; private set; }

        public bool IsDefault { get; private set; }

        public override string ToString()
        {
            return this.IsDefault ? String.Format("{0} (default)", this.Name) : this.Name;
        }
    }
}
