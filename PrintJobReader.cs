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
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Threading;

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
        private readonly Action<string> logger;
        private readonly CancellationToken cancelToken;

        public eColorMode ColorMode { get; set; }
        public eDuplexMode DuplexMode { get; set; }

        public PrintJobReader(Action<string> logger, CancellationToken cancelToken)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            this.logger = logger;
            this.cancelToken = cancelToken;
            this.listener = new TcpListener(IPAddress.Any, PrintJobReader.JOB_LISTEN_PORT);
        }

        public void Start()
        {
            this.listener.Start();
        }

        public void Stop()
        {
            this.listener.Stop();
        }

        public void Dispose() { }

        public string GetJob()
        {
            string filePath;
            do
            {
                Thread.Sleep(250);
                this.cancelToken.ThrowIfCancellationRequested();
            } while (!this.listener.Pending());
            using (var client = this.listener.AcceptTcpClient())
            {
                filePath = Path.GetTempFileName();
                using (var stream = File.OpenWrite(filePath))
                {
                    var remoteIpData = (client.Client.RemoteEndPoint as IPEndPoint);
                    this.logger.Invoke(String.Format("Incoming print job from {0}:{1}", remoteIpData.Address, remoteIpData.Port));
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
                   this.logger.Invoke(String.Format("Wrote print job file \"{0}\", {1} bytes in size", filePath, readCount));
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

                    this.logger.Invoke("Duplex set to: " + System.Enum.GetName(typeof(eDuplexMode), DuplexMode));
                    this.logger.Invoke("ColorMode set to: " + System.Enum.GetName(typeof(eColorMode), ColorMode));                    
              
                }
                
            }
            return filePath;
        }
    }
}
