using System.Diagnostics;
using System.Net;

using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Pipeline;
using System;

namespace Touch2PcPrinter
{
    internal class SnmpPrinterAgent
    {
        private const string COMMUNITY_NAME = "public";
        private static readonly OctetString UserName = new OctetString(COMMUNITY_NAME);
        private static readonly ObjectStore printerObjects = new ObjectStore();
        private const int SNMP_PORT_NUMBER = 161;

        private readonly SnmpEngine engine;

        static SnmpPrinterAgent()
        {
            SnmpPrinterAgent.printerObjects.Add(new PrinterDescriptionObject());
            SnmpPrinterAgent.printerObjects.Add(new PrinterStateObject());
            SnmpPrinterAgent.printerObjects.Add(new DeviceStateObject());
            SnmpPrinterAgent.printerObjects.Add(new PrinterErrorBitsObject());
        }

        public SnmpPrinterAgent()
        {
            var handlerMappings = new HandlerMapping[] { new HandlerMapping("v1", "GET", new GetV1MessageHandler()) };
            var appFactory = new SnmpApplicationFactory(SnmpPrinterAgent.printerObjects, new Version1MembershipProvider(UserName, UserName), new MessageHandlerFactory(handlerMappings));
            this.engine = new SnmpEngine(appFactory, new Listener(), new EngineGroup());
        }

        public void Start()
        {
            this.engine.Listener.AddBinding(new IPEndPoint(IPAddress.Any, SnmpPrinterAgent.SNMP_PORT_NUMBER));
            this.engine.Listener.ExceptionRaised += this.engine_ExceptionRaised;
            this.engine.Start();            
        }

        private void engine_ExceptionRaised(object sender, ExceptionRaisedEventArgs e)
        {
            Trace.TraceError("Exception thrown from SNMP agent: {0}", e.Exception.Message);
        }

        //sla 20.09.2011 - stop the engine if no longer needed - issue #6
        internal void Stop()
        {            
            if (engine != null)
                engine.Stop();
        }
        //..sla 20.09.2011 - stop the engine if no longer needed - issue #6
    }
}
