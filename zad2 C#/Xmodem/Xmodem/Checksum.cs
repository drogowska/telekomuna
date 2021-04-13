using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xmodem
{
    class Checksum
    {
      
        public static byte algebraicSum(byte[] bytes)
        {
            int sum = 0;
            for (int i=0;i<bytes.Length;i++)
            {
                sum += bytes[i];
                sum %= 256;
            }
            return (byte)sum;

        }

        public static ushort crc16(byte[] bytes)
        {
            ushort crc = 0x0000;
            
            for (int i = 0; i < bytes.Length; i++)
            {
                crc ^= (ushort)(bytes[i] << 8);
                int j = 0;
                do
                {
                    if ((crc & 0x8000) > 0)
                    {
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    }
                    else
                    {
                        crc <<= 1;
                    }
                    j++;
               
                } while (j < 8);
            }
            return crc;

        }
    }
}
