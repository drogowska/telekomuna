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

        private SerialPort serialPort;
        private bool crc;
        private int byteSize = 8;
        private byte[] bytes;
        private byte[] receivedBytes = new byte[128];
        private byte[] final;
        int noOfBlocks = 0;



        public Receiver(String name, int boundRate, Parity parity, StopBits stopBits, bool crc)
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

            //data = bytes;
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

        //private void startTransmition(bool crc)
        //{
        //    DateTime time = DateTime.Now;
        //    while(true)//(DateTime.Now - time <TimeSpan.FromSeconds(60))
        //    {
        //        if (crc)
        //        {
        //            serialPort.Write(new byte[] { C },0,1);
        //        }
        //        else
        //        { //suma algebraiczna
        //            open();
        //            serialPort.Write( new byte[] { NAK },0,1);
        //        }
        //        Thread.Sleep(3000);
        //        serialPort.DataReceived += new SerialDataReceivedEventHandler(dataReceived);
        //        Console.ReadLine();
               
        //    }  
        //}

      
        public byte[] ReceiveBytes()
        {
            //startTransmition(crc);
            if (crc)
            {
                open();
                serialPort.Write(new byte[] { C }, 0, 1);
            }
            else
            { //suma algebraiczna
                open();
                serialPort.Write(new byte[] { NAK }, 0, 1);
            }
            serialPort.DataReceived += new SerialDataReceivedEventHandler(dataReceived);
            //if (!serialPort.IsOpen) serialPort.Open();

            //byte[] tab = new byte[128 * noOfBlocks];
            //int k = 0;
            //for(int i = 0; i < tab.Length; i++)
            //{
            //    if (i % 128 == 0) k = 0;
            //    tab[i] = receivedBytes[k];
            //    k++;
            //}

            return final; 

        }
        private void dataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string received = serialPort.ReadExisting();
            bytes = Encoding.Default.GetBytes(received);
            Console.WriteLine(received);
            Console.WriteLine(bytes[0]);

            if (bytes.Length == 0) return;
            switch (bytes[0])
            {
                case 0x01: //SOH
                    noOfBlocks++;
                    receiveFile(bytes);
                    break;
                case 0x04: //EOT
                    serialPort.Write(new byte[] { ACK }, 0, 1);

                    break;
                case 0x15: //NAK

                    break;
                case 0x018: //CAN
                    //serialPort.Close();

                    ReceiveBytes();
                    //open();
                    break;
                default:
                    break;
            }

        }
        public void receiveFile(byte[] bytes)
        {

            byte[] tab = new byte[131];
            if (bytes.Length < 132) serialPort.Write(new byte[] { NAK }, 0, 1);
            else
            {
                for (int i = 3; i < 131; i++)
                {
                    receivedBytes[i - 3] = bytes[i];
                }
            }

            //for(int j = 0; j < 130; j++)
            //{
            //    tab[j] = bytes[j];
            //}
            if (check(receivedBytes))
            {
                serialPort.Write(new byte[] { ACK }, 0, 1);
                final = new byte[128];
                Array.Copy(receivedBytes, 0, final, noOfBlocks*128, 128);
                Array.Resize(ref final, final.Length+128);
                ////noOfBlocks++;
            } else
            {
                serialPort.Write(new byte[] { NAK }, 0, 1);
            }  
        }     

        private bool check(byte[] tab)
        {
            bool isOK = false;
            if (crc)
            {
                byte[] checksum = BitConverter.GetBytes(Checksum.crc16(receivedBytes));
                if(checksum[1] == bytes[131])
                {
                    if (checksum[0] == bytes[132])
                        isOK = true;
                }
               
            } else
            {
                byte checksum = Checksum.algebraicSum(receivedBytes);
                if (checksum == bytes[131]) isOK = true;                
            }
            return isOK;

        }
    }
}
