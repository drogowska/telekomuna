using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huffman
{
    class Checksum
    {
        //funckcja licząca sumę alebraiczną, podanych jako parametr, bajtów
        public static byte algebraicSum(byte[] bytes)
        {
            int sum = 0;
            for (int i = 0; i < 128; i++)
            {
                sum += bytes[i];                //obliczenie sumy
            }
            return (byte)(sum % 256);            //zwrócenie sumy modulo 256 aby nie wyszła poza zakres
        }

        //funkcja licząca CRC 16 dla protokołu Xmodem
        public static ushort crc16(byte[] bytes)
        {
            ushort crc = 0x0000;
            for (int i = 0; i < bytes.Length; i++)
            {
                crc ^= (ushort)(bytes[i] << 8);                             //xor policzonej wcześniej wartości i wartości odczytanej z tablicy przesuniętej o 8 miejsc w lewo
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) > 0)                                 //koniunkcja obliczonej wartości i liczby 0x8000 jeśli jest większa od zera to
                    {
                        crc = (ushort)((crc << 1) ^ 0x1021);                //wykonanie operacji przesunięcia w lewo o 1 obliczonej wartości i następnie xor wyniku tej operacji z liczbą 0x1021
                    }
                    else
                    {
                        crc <<= 1;                                          //jeżeli nie to obliczoną wartości przesuwamy o 1 w lewo
                    }
                }
            }

            return crc;

        }
    }
}
