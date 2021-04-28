using System;
using System.IO.Ports;
using System.Text;

namespace Huffman
{
    public class Transmitter
    {
        static byte SOH = 0x01;
        static byte EOT = 0x04;
        static byte ACK = 0x06;
        static byte NAK = 0x15;
        static byte CAN = 0x18;
        static byte C = 0x43;

        private byte[] data;                                //tablica przechowująca bajty z pliku do przesłania
        private SerialPort serialPort;
        bool flag = false;                                  //zmienna mówiąca czy przesłano pierwszy blok
        private bool crc;                                   //flaga sumy kontrolnej, true -> crc, false -> checksum
        private int byteSize = 8;                           //rozmiar bajta w bitach
        private byte[] bytes;                               //tablica przechowująca otrzymane dane wraz z nagłówkiem i sumą kontrolną
        private byte[] receivedBytes = new byte[131];       //tablica przechowująca dane z tablicy bytes ale bez nagłówka i sumy kontrolnej
        private byte[] final = new byte[128];               //tablica przechowująca wszystkie odebrane bloki danych 
        private int noOfBlock;                              //zmianna przechowująca ilość przesłanych bloków danych 


        public Transmitter(String name, int boundRate, Parity parity, StopBits stopBits, byte[] bytes, bool crc)
        {
            serialPort = new SerialPort(name, boundRate, parity, byteSize, stopBits);               //tworzenie obiektu klasy SerialPort z podanymi parametrami

            serialPort.Handshake = Handshake.None;      //ustawienie protokółu uzgadniania transmisji danych portu szeregowego
            serialPort.WriteTimeout = 10000;            //ustawienie liczby milisekund przed upływem limitu czasu, gdy operacja zapisu nie zostanie zakończona.
            serialPort.ReadTimeout = 10000;             //ustawienie liczby milisekund przed upływem limitu czasu, gdy operacja odczytu nie zostanie zakończona.
            serialPort.Encoding = Encoding.UTF8;        //ustawienie kodowania na utf8
            this.crc = crc;
            data = bytes;
        }

        //funkcja otwierająca zamknięty port
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


        public void sendFile()
        {
            //nasłuchiwanie portu i gdy w buforze portu pojawią się bajty to przekierowuje ich obsługę do metody dataReceived
            serialPort.DataReceived += new SerialDataReceivedEventHandler(dataReceived);

            //otwarcie portu
            open();
        }

        //funcka obiera przesłane bajty
        private void dataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string received = serialPort.ReadExisting();                    //otczytanie danych odebranych przez port
            byte[] bytes = Encoding.Default.GetBytes(received);             //konwersja string na bajty

            switch (bytes[0])
            {
                case 0x43: //C                                              //jeżeli odebrano znak C to wysyłamy dane z trybem crc
                    sendBytes();
                    break;
                case 0x06: //ACK    
                    if (flag)                                               //jeżli odebrano znak ACK i wysłano już pierwszy blok danych to wysyła kolejne bloki
                    {
                        sendBytes();
                        noOfBlock++;                                        //zwiększenie ilości przesłanych bloków danych
                    }
                    else
                        close();
                    break;
                case 0x15: //NAK
                    sendBytes();                                            //jeżeli odebrano znak NAK to wysyłamy dane z trybem checksum bądź wysyłamy dany blok kolejny raz ze względu na niezgodność sumy kontrolnej
                    break;
                case 0x018: //CAN                                           //jeżeli odebrano znak CAN to port zostaje zamknięty
                    close();
                    break;
                default:
                    break;
            }
        }

        private void sendBytes()
        {

            if ((noOfBlock) * 128 >= data.Length)           //jeśli rozmiar pliku jest mniejszy lub równy ilści wysłanych bloków razy ilość bajtów w bloku to znaczy że plik został w całości przesłany
            {
                serialPort.Write(new byte[] { EOT }, 0, 1);     //po przesłaniu całego pliku wysyłamy znak end of transmition i kończymy funkcje
                return;
            }
            byte[] block = new byte[crc ? 133 : 132];               //tablica przechowująca blok danych
            byte[] pom = new byte[128];                             //pomcnicza tablica przechowująca dane bez nagłówka
            flag = true;                                            //ustawienie flagi przesłania pierwszego bloku na true
            int blocks = (int)(data.Length / 128);                  //obliczenie ilości bloków jako iloraz wielkości danych podzielnych przez 128 (rozmiar bloku)
            int k = 3;                                              //zmienna do itercacji, od 3 indeksu będą wpisywane dane

            //stworzenie nagłówka 
            block[0] = SOH;                                         //znak SOH
            block[1] = (byte)(noOfBlock + 1);                         //numer bloku
            block[2] = (byte)(255 - (noOfBlock + 1));               //dopełnienie numeru bloku do 255

            for (int j = ((noOfBlock) * 128); j < ((noOfBlock + 1) * 128); j++)             //iteracja co 128 bajtów w celu uzpełnienia każdego bloku o nagłówek
            {

                if (blocks == (noOfBlock))
                {
                    if (j >= data.Length)                           //dla ostatniego bloku danych sprawdzamy czy długość danych z pliku jest większa od numeru iteracji
                    {
                        for (int l = k; l < 128; l++)
                        {
                            block[k] = 0;                           //dopełnienie zerami jeśli długość ostatniego bloku nie jest równy 128
                            pom[k - 3] = 0;
                        }
                        break;
                    }
                }
                block[k] = data[j];                                 //przepisanie danych do tablicy block z nagłówkiem 
                pom[k - 3] = data[j];                                 //przepisanie danych do tablicy pom z pominięciem nagłówka
                k++;
            }

            if (!crc)
            {
                block[131] = Checksum.algebraicSum(pom);                                    //obliczenie algebraicznej sumy kontrolnej i wpisanie ich do bloku
            }
            else
            {
                byte[] checksum = BitConverter.GetBytes(Checksum.crc16(pom));               //obliczenie sumy kontrolnej crc 16 dla danych z tabeli pom
                block[131] = checksum[1];                                                   //przepisanie wyników na koniec bloku danych
                block[132] = checksum[0];
            }
            serialPort.Write(block, 0, block.Length);                                       //wysłanie bloku danych z nagłówkiem i sumą kontrolną
        }

    }
}
