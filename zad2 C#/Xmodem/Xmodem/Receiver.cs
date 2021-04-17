using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Diagnostics;
using System.ComponentModel;
using System.IO.Ports;
using System.IO;
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
        private byte[] bytes;                               //tablica przechowująca otrzymane dane wraz z nagłówkiem i sumą kontrolną
        private byte[] receivedBytes = new byte[131];       //tablica przechowująca dane z tablicy bytes ale bez nagłówka i sumy kontrolnej
        private byte[] final = new byte[128];               //tablica przechowująca wszystkie odebrane bloki danych 
        int noOfBlocks = 0;
        private Stream file;


        public Receiver(String name, int boundRate, Parity parity, StopBits stopBits, bool crc, Stream file)
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
            this.file = file;
            bytes = new byte[crc ? 133 : 132];

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

     
        public void receiveBytes()
        {
            serialPort.DataReceived += new SerialDataReceivedEventHandler(dataReceived);
            open();

            if (crc)
            {
                serialPort.Write(new byte[] { C }, 0, 1);
                //serialPort.Close();
            }
            else
            { //suma algebraiczna
                serialPort.Write(new byte[] { NAK }, 0, 1);
            }

            //return receivedBytes; 

        }
        private void dataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int toRead = serialPort.BytesToRead;
            serialPort.Read(bytes, 0,bytes.Length);

            if (bytes.Length == 0) return;
            switch (bytes[0])
            {
                case 0x01: //SOH
                    receiveFile();
                    break;
                case 0x04: //EOT
                    serialPort.Write(new byte[] { ACK }, 0, 1);
                    file.Write(final,0, final.Length);
                    file.Close();
                    break;
                case 0x15: //NAK
                    break;
                case 0x018: //CAN
                    serialPort.Close();
                    receiveBytes();
                    break;
                default:
                    break;
            }

        }
        public void receiveFile()
        {
            byte[] tab = new byte[128];
            if (bytes.Length < 132) {
                serialPort.Write(new byte[] { NAK }, 0, 1);         //jeżeli odebrano mniej niż minimalna długość bloku to wysyłamy znak NAK
                return;
            }
            else
            {
                for (int i = 3; i < 131; i++)
                {
                    receivedBytes[i-3] = bytes[i];        //przepisanie odebranych danych do tablicy z pominięciem nagłówka (pierwszych 3 bajtów każdego bloku) i bajtów sumy kontrolnej
                }
               
                for(int j=0;j<128;j++)
                {
                    if (receivedBytes[j] == 26) tab[j] = 0;
                    else tab[j] = receivedBytes[j];
                }
            }

            if (check(receivedBytes))       //wywołanie funkcji sprawdzającej poprawność sumy kontrolnej 
            {
                serialPort.Write(new byte[] { ACK }, 0, 1);     //jeżeli sumy kontrolne się zgadzają to wysyłamy znak ACK
                Array.Copy(tab, 0, final, noOfBlocks*128, 128);
                Array.Resize(ref final, final.Length + 128);
                noOfBlocks++;
            } else
            {
                serialPort.Write(new byte[] { NAK }, 0, 1);     //jeżeli sumy kontrolne się różnią to wysyłamy znak NAK
            }  
        }     

        //funkcja sprawdzająca otrzymane sumy kontrlne z sumamą kontrolną obliczonyą na podstawie bloku danych podanych jako parametr
        private bool check(byte[] tab)
        {
            bool isOK = false;
            if (crc)
            {
                byte[] checksum = BitConverter.GetBytes(Checksum.crc16(tab));
                if(checksum[1] == bytes[131])
                {
                    if (checksum[0] == bytes[132])
                        isOK = true;
                }
               
            } else
            {
                byte checksum = Checksum.algebraicSum(tab);
                if (checksum == bytes[131]) isOK = true;                
            }
            return isOK;

        }
    }
}
