using System;

using Lextm.SharpSnmpLib.Pipeline;
using Lextm.SharpSnmpLib;

namespace CBonnell.TouchPrintDaemon
{
    internal class PrinterDescriptionObject : ScalarObject
    {
        private const string REPORTED_PRINTER_NAME = "hp LaserJet CP6015 PCL6";

        private static readonly OctetString printerDescription = new OctetString(String.Format("MFG:Hewlett-Packard;CMD:PJL,MLC,PCL,POSTSCRIPT,PCLXL;MDL:{0};CLS:PRINTER;DES:Hewlett-Packard LaserJet 3380;MEM:23MB;COMMENT:RES=1200x1;", PrinterDescriptionObject.REPORTED_PRINTER_NAME));

        public PrinterDescriptionObject() : base(new ObjectIdentifier("1.3.6.1.4.1.11.2.3.9.1.1.7.0")) { }

        public override ISnmpData Data
        {
            get
            {
                return PrinterDescriptionObject.printerDescription;
            }
            set
            {
                throw new AccessFailureException();
            }
        }
    }

    internal class PrinterStateObject : ScalarObject
    {
        private static readonly Integer32 printerState = new Integer32(2);

        public PrinterStateObject() : base(new ObjectIdentifier("1.3.6.1.2.1.25.3.2.1.5.1")) { }

        public override ISnmpData Data
        {
            get
            {
                return PrinterStateObject.printerState;
            }
            set
            {
                throw new AccessFailureException();
            }
        }
    }

    internal class DeviceStateObject : ScalarObject
    {
        private static readonly Integer32 deviceState = new Integer32(3);

        public DeviceStateObject() : base(new ObjectIdentifier("1.3.6.1.2.1.25.3.5.1.1.1")) { }

        public override ISnmpData Data
        {
            get
            {
                return DeviceStateObject.deviceState;
            }
            set
            {
                throw new AccessFailureException();
            }
        }
    }

    internal class PrinterErrorBitsObject : ScalarObject
    {
        private static readonly OctetString errorBits = new OctetString("00");

        public PrinterErrorBitsObject() : base(new ObjectIdentifier("1.3.6.1.2.1.25.3.5.1.2.1")) { }

        public override ISnmpData Data
        {
            get
            {
                return PrinterErrorBitsObject.errorBits;
            }
            set
            {
                throw new AccessFailureException();
            }
        }
    }
}
