using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
