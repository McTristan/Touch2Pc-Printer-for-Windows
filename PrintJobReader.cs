using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace Touch2PcPrinter
{
    internal class PrintJobReader
    {
        private const int JOB_LISTEN_PORT = 9100;
        private const int UTF16_HEADER_SIZE = 0x1A2;

        private readonly TcpListener listener;
        private readonly string outputPath;

        public PrintJobReader(string outputPath)
        {
            if (outputPath == null)
            {
                throw new ArgumentNullException("outputPath");
            }
            if (!Directory.Exists(outputPath))
            {
                throw new DirectoryNotFoundException(String.Format("Directory not found: {0}", outputPath));
            }
            this.outputPath = outputPath;
            this.listener = new TcpListener(IPAddress.Any, PrintJobReader.JOB_LISTEN_PORT);
        }

        public void Start()
        {
            this.listener.Start(10);
        }

        //sla 22.09.2011 - 1st part of issue #8 - SNMP engine does not seem to get any messages
        public void Stop()
        {
            this.listener.Stop();
        }
        //..sla 22.09.2011 - 1st part of issue #8 - SNMP engine does not seem to get any messages

        public string GetJob()
        {
            string filePath;
            using (TcpClient client = listener.AcceptTcpClient())
            {
                do
                {
                    filePath = Path.Combine(this.outputPath, Path.GetRandomFileName());
                } while (File.Exists(filePath) || Directory.Exists(filePath));

                using (FileStream stream = File.OpenWrite(filePath))
                {
                    IPEndPoint remoteIpData = (client.Client.RemoteEndPoint as IPEndPoint);
                    Trace.TraceInformation("Incoming print job from {0}:{1}", remoteIpData.Address, remoteIpData.Port);
                    byte[] buffer = new byte[4096];
                    int readCount = 0;
                    do
                    {
                        int read = client.GetStream().Read(buffer, 0, buffer.Length);
                        if (read < 1)
                        {
                            break;
                        }
                        readCount += read;
                        stream.Write(buffer, 0, read);
                    } while (true);
                    stream.Close();
                    client.Close();
                    Trace.TraceInformation("Wrote print job file \"{0}\", {1} bytes in size", filePath, readCount);
                }
                
            }
            return filePath;
        }
    }
}
