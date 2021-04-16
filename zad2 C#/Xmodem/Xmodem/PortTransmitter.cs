using System;
using System.IO.Ports;
using System.Text;

namespace Xmodem
{
    public class PortTransmitter
    {
        private SerialPort serialPort;
        private bool crc;
        private int byteSize = 8; //8 bitów

        static byte SOH = 0x01;
        static byte EOT = 0x04;
        static byte ACK = 0x06;
        static byte NAK = 0x15;
        static byte CAN = 0x18;
        static byte C = 0x43;

        private byte[] data;
        private int noOfBlock;
        bool flag = false;
        public PortTransmitter(String name, int boundRate, Parity parity, StopBits stopBits, byte[] bytes, bool crc)
        {
            serialPort = new SerialPort(name);

            serialPort.BaudRate = boundRate;
            serialPort.DataBits = byteSize;
            serialPort.Parity = parity;
            serialPort.StopBits = stopBits;
            serialPort.Handshake = Handshake.None;
            serialPort.WriteTimeout = 10000; //ms
            serialPort.ReadTimeout = 10000;
            serialPort.Encoding = Encoding.UTF8;
            this.crc = crc;

            data = bytes;
        }

        //otwarcie portu
        public void open()
        {
            if(!serialPort.IsOpen)
            serialPort.Open();

        }

        //funkcja zamyka otwarty port
        public void close()
        {
            if (serialPort.IsOpen)
                serialPort.Close();
        }


        public void sendFile()
        {
            //nasłuchiwanie portu i gdy w buforze portu pojawią się bajty to przekierowuje ich obsługę do metody dataReceived
            serialPort.DataReceived += new SerialDataReceivedEventHandler(dataReceived);

            //otwarcie portu
            open();
            Console.ReadLine();
        }

        private void dataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string received = serialPort.ReadExisting();
            byte[] bytes = Encoding.Default.GetBytes(received);
            Console.WriteLine(received);
            Console.WriteLine(bytes[0]);

            switch (bytes[0])
            {
                case 0x43: //c
                    //crc = true;
                    
                    sendBytes();
                    
                    break;
                case 0x06: //ack
                    if (flag)
                    {
                        sendBytes();
                        noOfBlock++;
                    } else
                        close();
                    break;
                case 0x15: //nak
                    sendBytes();
                    break;
                case 0x018: //can
                    close();
                    break;
                default:
                    break;
            }


        }

        private void sendBytes()
        {

            if ((noOfBlock) * 128 >= data.Length)
            {
                serialPort.Write(new byte[] { EOT }, 0, 1);
                return;
            }

            //byte[] header = new byte[3];                //talica przechowująca nagłówek 
            byte[] block = new byte[crc ? 133 : 132];               //tablica przechowująca blok danych
            byte[] pom = new byte[128];                                                 //byte[] all = new byte[crc ? 133 : 132];     //tablica będąca konkatenacją tablicy header i block
            flag = true;

            //int blocks = (int)Math.Ceiling(d: (decimal)data.Length / 128);       //obliczenie ilości bloków jako sufit wielkości danych podzielnych przez 128 (rozmiar bloku)
            int blocks = (int)(data.Length / 128);       //obliczenie ilości bloków jako sufit wielkości danych podzielnych przez 128 (rozmiar bloku)

            //stworzenie nagłówka 
            block[0] = SOH;                            //znak SOH
            block[1] = (byte)(noOfBlock+1);          //numer bloku
            block[2] = (byte)(255 - (noOfBlock + 1));  //dopełnienie numeru bloku do 255

            int k = 3;
            int tmp = 0;
            int s = 0;
            Console.WriteLine(noOfBlock);
            
            for (int j = ((noOfBlock) * 128); j < ((noOfBlock+1) * 128); j++)
            {
                   
                if (blocks == (noOfBlock))      //ostatni blok
                {
                    if (j >= data.Length)
                    {
                        for(int l = k; l < 128; l++)
                        {
                            block[k] = 0;   //dopełnienie zerami
                            pom[s] = 0;
                        }
                        break;
                    }
                       
                }
                block[k] = data[j];
                pom[s] = data[j];
                k++;
                s++;
            }



            if (!crc)
            {       //obliczenie algebraicznej sumy kontrolnej i wpisanie ich do bloku
                block[131] = Checksum.algebraicSum(pom);
            }
            else
            {       //obliczenie crc16 i wpisanie wyników do bloku
                byte[] checksum = BitConverter.GetBytes(Checksum.crc16(pom));
                block[131] = checksum[1];
                block[132] = checksum[0];
            }
            serialPort.Write(block, 0, block.Length);
        }

    }
}
