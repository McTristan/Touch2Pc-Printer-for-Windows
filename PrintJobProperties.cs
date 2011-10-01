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

namespace Touch2PcPrinter
{
    internal enum ColorMode
    {
        Color = 0,
        BlackAndWhite = 1,
    }

    internal enum PlexMode
    {
        Simplex = 0,
        Duplex = 1,
    }

    internal class PrintJobProperties
    {
        public PrintJobProperties(ColorMode colorMode, PlexMode plexMode)
        {
            this.ColorMode = colorMode;
            this.PlexMode = plexMode;
        }

        public ColorMode ColorMode
        {
            get;
            private set;
        }

        public PlexMode PlexMode
        {
            get;
            private set;
        }

        public override int GetHashCode()
        {
            long hash = (long)this.ColorMode.GetHashCode() + (long)this.PlexMode.GetHashCode();
            return hash.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as PrintJobProperties;
            if (other == null)
            {
                return false;
            }
            return this.ColorMode == other.ColorMode && this.PlexMode == other.PlexMode;
        }
    }
}
