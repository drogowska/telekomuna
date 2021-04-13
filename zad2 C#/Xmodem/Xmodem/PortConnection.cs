using System;
using System.IO.Ports;
using System.Text;

namespace Xmodem
{
    public class PortConnection
    {
        private SerialPort serialPort;
        private bool con;
        private int byteSize = 8; //8 bitów

        private byte[] header = new byte[3];
        private byte[] block = new byte[128];

        public PortConnection(String name, int boundRate, Parity parity, StopBits stopBits)
        {
            serialPort = new SerialPort(name);
            
            serialPort.BaudRate = boundRate;
            serialPort.DataBits = byteSize;
            serialPort.Parity = parity;
            serialPort.StopBits = stopBits;
            serialPort.Handshake = Handshake.None;
            serialPort.WriteTimeout = 10000; //ms
            serialPort.ReadTimeout = 1000;
            serialPort.Encoding = Encoding.UTF8;

        }

        public void write(byte[] bytes)
        {
            serialPort.Write(bytes, 0, bytes.Length);
        }

        public void write(byte buffer)
        {
            serialPort.Write(new byte[] { buffer }, 0, 1);
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
            if (serialPort.IsOpen)
                serialPort.Close();
        }

        //public void read()
        //{
        //    while (con)
        //    {
        //        try
        //        {
        //            string message = serialPort.ReadLine();
        //            Console.WriteLine(message);
        //            con = false;
        //        }
        //        catch (TimeoutException) { }
        //    }
        //}

        public byte[] read()
        {
            //List<byte> byteList = new List<byte>();
            byte[] byteArray;
            do
            {
                int bytesToRead = serialPort.BytesToRead;
                byteArray = new byte[bytesToRead];
                serialPort.Read(byteArray, 0, bytesToRead);
                //byteList.AddRange(byteArray);
                System.Threading.Tasks.Task.Delay(10).Wait();
            } while (serialPort.BytesToRead > 0);
            return byteArray;
        }

        public byte readSingleByte()
        {
            return (byte)serialPort.ReadByte();
        }


    }
}
