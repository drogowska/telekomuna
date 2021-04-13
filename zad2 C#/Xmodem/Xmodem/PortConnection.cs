using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Threading;

namespace Xmodem
{
    class PortConnection
    {
        private SerialPort serialPort;
        private bool con;
        private int byteSize = 8; //8 bitów

        private byte[] header = new byte[3];
        private byte[] block = new byte[128];

        public PortConnection(String name, int boundRate, Parity parity, StopBits stopBits)
        {
            serialPort = new SerialPort(name, boundRate, parity, byteSize, stopBits)
            {
                WriteTimeout = 10000,       //ms
                ReadTimeout = 1000,
                Handshake = Handshake.None
            };
            serialPort.Encoding = Encoding.UTF8;

        }

        public void write(byte[] bytes)
        {
            serialPort.Write(bytes, 0, bytes.Length);
        }

        //otwarcie portu
        public void open()
        {
            serialPort.Open();
            con = true;
        }

        //funkcja zamyka otwarty port
        public void close()
        {
            if(serialPort.IsOpen)
            serialPort.Close();
        }

        public void read()
        {
            while (con)
            {
                try
                {
                    string message = serialPort.ReadLine();
                    Console.WriteLine(message);
                    con = false;
                }
                catch (TimeoutException) { }
            }
        }

       
    }
}
