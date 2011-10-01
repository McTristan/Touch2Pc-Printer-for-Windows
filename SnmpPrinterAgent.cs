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
using System.Net;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Pipeline;

namespace Touch2PcPrinter
{
    internal class SnmpPrinterAgent : IPrintServerComponent
    {
        private const string COMMUNITY_NAME = "public";
        private static readonly OctetString UserName = new OctetString(COMMUNITY_NAME);
        private static readonly ObjectStore printerObjects = new ObjectStore();
        private const int SNMP_PORT_NUMBER = 161;

        private readonly SnmpEngine engine;
        private readonly Action<string> logger;

        static SnmpPrinterAgent()
        {
            SnmpPrinterAgent.printerObjects.Add(new PrinterDescriptionObject());
            SnmpPrinterAgent.printerObjects.Add(new PrinterStateObject());
            SnmpPrinterAgent.printerObjects.Add(new DeviceStateObject());
            SnmpPrinterAgent.printerObjects.Add(new PrinterErrorBitsObject());
        }

        public SnmpPrinterAgent(Action<string> logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            this.logger = logger;

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

        public void Stop()
        {
            this.engine.Stop();
        }

        private void engine_ExceptionRaised(object sender, ExceptionRaisedEventArgs e)
        {
            this.logger.Invoke(String.Format("Exception thrown from SNMP agent: {0}", e.Exception));
        }

        private bool disposed = false;
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }
            this.engine.Dispose();
        }
    }
}
