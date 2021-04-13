using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

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

        private PortConnection connection;
        private bool start;
        public Receiver(PortConnection conn)
        {
            connection = conn;
        }

        private void startTransmition(bool crc)
        {
            DateTime time = DateTime.Now;
            while(DateTime.Now - time <TimeSpan.FromSeconds(60))
            {
                if (crc)
                {
                    connection.write(C);
                }
                else
                { //suma algebraiczna
                    connection.write(NAK);
                }

                if (connection.readSingleByte() == SOH)
                {
                    start = true;
                    break;
                }
                Thread.Sleep(10000);
            }  
        }

      

        public void receiveFile(bool crc)
        {
            startTransmition(crc);
            List<byte> data = new List<byte>();
            if (start)
            {
                byte b = SOH;
                do
                {
                    byte[] block = new byte[128];
                    for (int i = 0; i < 128; i++)
                    {
                        block = connection.read();
                    }
                    if (!crc)
                    {
                        checkChecksum(block,data);
                    }
                    else
                    { //crc16
                        checkCrc16(block, data);
                    }

                } while (b == SOH);
                if (b == EOT)
                {
                    connection.write(NAK);
                    //readerWriter.write(new Vector<>(ACK));
                }
                else throw new Exception("Protocol error");
            }

            //return data;
        }

        private void checkChecksum(byte[] block, List<byte> data) 
        {
            byte[] check = connection.read();
            if (Checksum.algebraicSum(block) == check[0]) {
                connection.write(ACK);
                for (int i = 0; i < block.Length; i++)
                {
                    data.Add(block[i]); //?
                }
            } else {
                connection.write(NAK);
            }
        }

        private void checkCrc16(byte[] block, List<byte> data)
        {

        }
    }
}
