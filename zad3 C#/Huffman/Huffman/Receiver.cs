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

namespace Huffman
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
        private bool crc;                                   //flaga sumy kontrolnej, true -> crc, false -> checksum
        private int byteSize = 8;                           //rozmiar bajta w bitach
        private byte[] bytes;                               //tablica przechowująca otrzymane dane wraz z nagłówkiem i sumą kontrolną
        private byte[] receivedBytes = new byte[131];       //tablica przechowująca dane z tablicy bytes ale bez nagłówka i sumy kontrolnej
        private byte[] final = new byte[128];               //tablica przechowująca wszystkie odebrane bloki danych 
        int noOfBlocks = 0;                                 //zmianna przechowująca ilość oderanych bloków danych 
        private Stream file;                                //strumien do zapisu do pliku


        public Receiver(String name, int boundRate, Parity parity, StopBits stopBits, bool crc, Stream file)
        {
            serialPort = new SerialPort(name, boundRate, parity, byteSize, stopBits);           //tworzenie obiektu klasy SerialPort z podanymi parametrami

            serialPort.Handshake = Handshake.None;      //ustawienie protokółu uzgadniania transmisji danych portu szeregowego
            serialPort.WriteTimeout = 10000;            //ustawienie liczby milisekund przed upływem limitu czasu, gdy operacja zapisu nie zostanie zakończona.
            serialPort.ReadTimeout = 10000;             //ustawienie liczby milisekund przed upływem limitu czasu, gdy operacja odczytu nie zostanie zakończona.

            this.crc = crc;
            this.file = file;
            bytes = new byte[crc ? 133 : 132];          //stworzenie tablicy bajtów 132 albo 133 bajtowej

        }

        //funkcja otwiera port jeśli jest zamknięty
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


        public void receiveFile()
        {
            //nasłuchiwanie portu i gdy w buforze portu pojawią się bajty to przekierowuje ich obsługę do metody dataReceived
            serialPort.DataReceived += new SerialDataReceivedEventHandler(dataReceived);
            open();                         //otwarcie portu jeśli nie jest otwarty
            startTransmition();             //rozpoczęcie transmisji
        }

        //funkcje inicjuje transmisje wysyłając odpowiedni znak co 10 s przez 1 min
        private void startTransmition()
        {
            DateTime time = DateTime.Now;                               //zapisanie czasu po wywołaniu funkcji
            while (DateTime.Now - time < TimeSpan.FromSeconds(60))           //wysyłanie co 10 sekund przez minutę odpowiednio od wybranego trybu albo znak NAK albo C
            {
                if (crc)
                {   //crc
                    serialPort.Write(new byte[] { C }, 0, 1);           //wysłanie znaku C
                }
                else
                { //suma algebraiczna
                    serialPort.Write(new byte[] { NAK }, 0, 1);         //wysłanie znaku NAK
                }
                Thread.Sleep(10000);
            }
        }

        //funkcja obiera wysłane bajty 
        private void dataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int toRead = serialPort.BytesToRead;                             //zapisanie ilości danych jakie zostaną odczytane
            serialPort.Read(bytes, 0, bytes.Length);                         //odczytanie z bufora bajtów i zapisanie ich do tablicy bytes

            if (bytes.Length == 0) return;                                  //jeżeli nic nie odebrano funkcja się kończy
            switch (bytes[0])
            {
                case 0x01: //SOH
                    receive();                                             //jeżli pierwszy odczytany bajt jest znakiem SOH to wywołuje sie funkcja receive
                    break;
                case 0x04: //EOT
                    serialPort.Write(new byte[] { ACK }, 0, 1);             //wysłanie znaku ACK 
                    file.Write(final, 0, final.Length);                      //zapisanie do strumienia otrzymane bloki danych
                    file.Close();                                           //zamknięcie pliku
                    return;
                case 0x15: //NAK
                    break;
                case 0x018: //CAN
                    serialPort.Close();                                     //jeśli odebrano znak odwołania "canacel" to port zostaje zamknięty i wywołuje się ponownie funkcja odbierająca plik
                    receiveFile();
                    break;
                default:
                    break;
            }

        }
        //funkcja przepisuje odebrane bajty z pominięciem nagłówka i sprawdza ich poprawność
        private void receive()
        {
            byte[] tab = new byte[128];
            if (bytes.Length < 132)
            {
                serialPort.Write(new byte[] { NAK }, 0, 1);         //jeżeli odebrano mniej niż minimalna długość bloku to wysyłamy znak NAK i metoda się kończy
                return;
            }
            else
            {
                for (int i = 3; i < 131; i++)
                {
                    receivedBytes[i - 3] = bytes[i];        //przepisanie odebranych danych do tablicy z pominięciem nagłówka (pierwszych 3 bajtów każdego bloku) i bajtów sumy kontrolnej
                }

                for (int j = 0; j < 128; j++)
                {
                    if (receivedBytes[j] == 26) tab[j] = 0;         //przepisanie otrzymanych danych do tablicy tab z zamianą 26 na 0 (tera term dopełnia 1A (hex) a nie zerami)
                    else tab[j] = receivedBytes[j];
                }
            }

            if (check(receivedBytes))       //wywołanie funkcji sprawdzającej poprawność sumy kontrolnej 
            {
                serialPort.Write(new byte[] { ACK }, 0, 1);             //jeżeli sumy kontrolne się zgadzają to wysyłamy znak ACK
                Array.Copy(tab, 0, final, noOfBlocks * 128, 128);         //tablica tab zostaje przepisana do tablicy final
                Array.Resize(ref final, final.Length + 128);            //zwiększenie rozmiaru tablicy final o 128 bajtów
                noOfBlocks++;                                           //zwiększenie ilości otrzymanych danych
            }
            else
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
                byte[] checksum = BitConverter.GetBytes(Checksum.crc16(tab));               //obliczenie sumy crc 16 dla danych z tablicy tab
                if (checksum[1] == bytes[131])                                               //sprawdzanie poprawności policzonej sumy z sumą otrzymaną
                {
                    if (checksum[0] == bytes[132])
                        isOK = true;                                                        //jeżeli się zgadzają zwracamy true
                }

            }
            else
            {
                byte checksum = Checksum.algebraicSum(tab);                                 //obliczenie sumy algebraicznie dla danych z tablicy tab
                if (checksum == bytes[131]) isOK = true;                                    //sprawdzenie poprawności policzonej sumy z sumą otrzymaną 
            }
            return isOK;

        }
    }
}
