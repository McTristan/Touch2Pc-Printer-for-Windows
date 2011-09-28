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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace Touch2PcPrinter
{
    internal class PrintJobReader : IPrintServerComponent
    {
        private const int JOB_LISTEN_PORT = 9100;

        private readonly TcpListener listener;
        private readonly Action<string> logger;
        private readonly CancellationToken cancelToken;

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

        public string ReadJobToFile()
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
            }
            return filePath;
        }

        private const int DUPLEX_SETTING_OFFSET = 0x36;
        private const string DUPLEX_SETTING_STRING = "ON";
        private const int SEARCH_SIZE = 512;
        private const string BLACK_WHITE_STRING = "&b1M";

        public static PrintJobProperties GetProperties(string pclFilePath)
        {
            if (pclFilePath == null)
            {
                throw new ArgumentNullException("pclFilePath");
            }
            byte[] initialBytes = new byte[PrintJobReader.SEARCH_SIZE];
            using (var stream = File.OpenRead(pclFilePath))
            {
                int totalRead = 0;
                do
                {
                    int read = stream.Read(initialBytes, totalRead, initialBytes.Length - totalRead);
                    if (read < 1)
                    {
                        throw new EndOfStreamException("PCL file is too small, cannot read job properties");
                    }
                    totalRead += read;
                } while (totalRead < PrintJobReader.SEARCH_SIZE);
            }
            PlexMode plexMode;
            ColorMode colorMode;

            if (String.Equals(PrintJobReader.DUPLEX_SETTING_STRING, Encoding.ASCII.GetString(initialBytes, PrintJobReader.DUPLEX_SETTING_OFFSET, DUPLEX_SETTING_STRING.Length), StringComparison.Ordinal))
            {
                plexMode = PlexMode.Duplex;
            }
            else
            {
                plexMode = PlexMode.Simplex;
            }
            string searchString = Encoding.ASCII.GetString(initialBytes);
            if (searchString.Contains(PrintJobReader.BLACK_WHITE_STRING))
            {
                colorMode = ColorMode.BlackAndWhite;
            }
            else
            {
                colorMode = ColorMode.Color;
            }
            return new PrintJobProperties(colorMode, plexMode);
        }

    }
}
