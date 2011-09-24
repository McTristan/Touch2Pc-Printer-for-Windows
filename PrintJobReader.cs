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
    enum eColorMode
    {
        Color = 0,
        BlackWhite = 1
    }

    enum eDuplexMode
    {
        NonDuplex = 0,
        Duplex = 1
    }

    internal class PrintJobReader
    {
        private const int JOB_LISTEN_PORT = 9100;
        private const int UTF16_HEADER_SIZE = 0x1A2;

        private readonly TcpListener listener;
        private readonly string outputPath;

        //sla 24.09.2011 - Color- & Duplex-Mode
        public eColorMode ColorMode { get; set; }
        public eDuplexMode DuplexMode { get; set; }
        //..sla 24.09.2011 - Color- & Duplex-Mode

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

                    Trace.TraceInformation("Wrote print job file \"{0}\", {1} bytes in size", filePath, readCount);

                    stream.Close();

                    client.Close();
                }

                using (FileStream stream = File.OpenRead(filePath))
                {
                    stream.Seek(0x36, SeekOrigin.Begin); // get Duplexmode
                    byte[] bArray = new byte[2];
                    if (stream.Read(bArray, 0, 2) == 2)
                    {
                        if (bArray[0] == 0x4F && bArray[1] == 0x4E)
                            DuplexMode = eDuplexMode.Duplex;
                    }

                    bArray = new byte[3];
                    stream.Seek(0, SeekOrigin.Begin); // get BlackWhite
                    for (int i = 0; i <= 512; i++)
                    {
                        if (stream.ReadByte() == 0x26)
                        {
                            if (stream.Read(bArray, 0, 3) == 3) //should contain "&b1M" for Black/White
                            {
                                if (bArray[0] == 0x62 && bArray[1] == 0x31 && bArray[2] == 0x4D)
                                {
                                    ColorMode = eColorMode.BlackWhite;                                    
                                }
                            }
                            else
                                stream.Seek(-2, SeekOrigin.Current);
                        }
                    }

                    Trace.TraceInformation("Duplex set to: " + System.Enum.GetName(typeof(eDuplexMode), DuplexMode));
                    Trace.TraceInformation("ColorMode set to: " + System.Enum.GetName(typeof(eColorMode), ColorMode));                    
              
                }
                
            }
            return filePath;
        }
    }
}
