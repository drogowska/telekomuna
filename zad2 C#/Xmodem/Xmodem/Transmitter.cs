using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xmodem
{

    class Transmitter
    {
        static byte SOH = 0x01;
        static byte EOT = 0x04;
        static byte ACK = 0x06;
        static byte NAK = 0x15;
        static byte CAN = 0x18;
        static byte C = 0x43;
        private PortConnection connection;
        bool start = false;

        public Transmitter(PortConnection conn)
        {
            connection = conn;
        }

        private byte[] createHeader(int i)
        {
            byte[] header = new byte[3];
            header[0] = SOH;
            header[1] = (byte)(i + 1);
            header[2] = (byte)(255 - (i + 1));
            return header;
        }

        public void sendFile(List<byte> data, bool crc)
        {
            if(!crc)                            //oczekiwanie na odbiornik
                while (connection.readSingleByte() != NAK);
            else //crc
                while (connection.readSingleByte() != C);

            int noOfBlock = (int)Math.Ceiling(d: (decimal)data.Count() / 128);

            for(int i = 0; i<noOfBlock; i++){
                List<byte> tmp = new List<byte>();
                for(int j=i*128;j<(i+1)*128;j++) {
                    tmp.Add(data.ElementAt(j));
                }
                if(tmp.Count < 128) {
                    for (int j = tmp.Count(); j< 128; j++)
                        tmp.Add((byte) 0);               //dopełnienie zerami
                                }
                byte[] header = createHeader(i);

                connection.write(header);             //wysłanie
                connection.write(tmp.ToArray());

                if (!crc)
                {
                    connection.write(Checksum.algebraicSum(tmp.ToArray()));
                }
                else
                {
                    connection.write((byte)Checksum.crc16(tmp.ToArray()));
                }

                byte response = connection.readSingleByte();
                if (response == NAK)
                {
                    i--;
                }
                else if (response == CAN)
                {
                    throw new Exception("Connection canceled!");
                }
                else if (response == ACK)
                {
                    throw new Exception("Protocol error.");
                }

                }
                connection.write(EOT);
            }
    }
}
