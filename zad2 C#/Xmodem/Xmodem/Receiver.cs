using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Diagnostics;
using System.ComponentModel;
using System.IO.Ports;

namespace Xmodem
{
    class Receiver 
    {
        static byte SOH = 0x01;
        static byte EOT = 0x04;
        static byte ACK = 0x06;
        static byte NAK = 0x15;
        static byte CAN = 0x18;
        static byte C = 0x43;

        private bool start;
        private SerialPort serialPort;
        private bool crc;
        private int byteSize = 8;
        private byte[] data;
        private byte[] receivedBytes;


        public Receiver(String name, int boundRate, Parity parity, StopBits stopBits, byte[] bytes, bool crc)
        {
            serialPort = new SerialPort(name);

            serialPort.BaudRate = boundRate;
            serialPort.DataBits = byteSize;
            serialPort.Parity = parity;
            serialPort.StopBits = stopBits;
            serialPort.Handshake = Handshake.None;
            serialPort.WriteTimeout = 10000; //ms
            serialPort.ReadTimeout = 100000;
            serialPort.Encoding = Encoding.UTF8;
            this.crc = crc;

            data = bytes;
            serialPort.Open();
        }

        //otwarcie portu
        public void open()
        {
            if (!serialPort.IsOpen)
                serialPort.Open();

        }

        //funkcja zamyka otwarty port
        public void close()
        {
            if (serialPort.IsOpen)
                serialPort.Close();
        }

        private void startTransmition(bool crc)
        {
            DateTime time = DateTime.Now;
            while(true)//(DateTime.Now - time <TimeSpan.FromSeconds(60))
            {
                if (crc)
                {
                    serialPort.Write(new byte[] { C },0,1);
                }
                else
                { //suma algebraiczna
                    open();
                    serialPort.Write( new byte[] { NAK },0,1);
                }
                Thread.Sleep(3000);
                serialPort.DataReceived += new SerialDataReceivedEventHandler(dataReceived);
                Console.ReadLine();
               
            }  
        }

      
        public byte[] ReceiveBytes()
        {
            //startTransmition(crc);
            if (crc)
            {
                serialPort.Write(new byte[] { C }, 0, 1);
            }
            else
            { //suma algebraiczna
                open();
                serialPort.Write(new byte[] { NAK }, 0, 1);
            }
            serialPort.DataReceived += new SerialDataReceivedEventHandler(dataReceived);
            //if (!serialPort.IsOpen) serialPort.Open();
            return data; 

        }
        private void dataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string received = serialPort.ReadExisting();
            byte[] bytes = Encoding.Default.GetBytes(received);
            Console.WriteLine(received);
            Console.WriteLine(bytes[0]);

            if (bytes.Length == 0) return;
            switch (bytes[0])
            {
                case 0x01: //SOH
                    start = true;
                    receiveFile(bytes);
                    break;
                case 0x04: //EOT
                    serialPort.Write(new byte[] { ACK }, 0, 1);

                    break;
                case 0x15: //NAK

                    break;
                case 0x018: //CAN
                    serialPort.Close();
                    break;
                default:
                    break;
            }

        }
        public void receiveFile(byte[] bytes)
        {
            for (int i = 0; i <128;i++)
            {
                data[i] = bytes[3+i];
            }
            
           
        }

        //private void checkChecksum(byte[] block, List<byte> data) 
        //{
        //    byte[] check = connection.read();
        //    if (Checksum.algebraicSum(block) == check[0]) {
        //        connection.write(ACK);
        //        for (int i = 0; i < block.Length; i++)
        //        {
        //            data.Add(block[i]); //?
        //        }
        //    } else {
        //        connection.write(NAK);
        //    }
        //}

        

        private bool check()
        {
            bool isOK = false;
            if(crc)
            {
                byte[] checksum = BitConverter.GetBytes(Checksum.crc16(data));
                if(checksum[0] == receivedBytes[131])
                {
                    if (checksum[1] == receivedBytes[132])
                        isOK = true;
                }
               
            } else
            {
                byte checksum = Checksum.algebraicSum(data);
                if (checksum == receivedBytes[131]) isOK = true;
            }
            return isOK;

        }
    }
}
