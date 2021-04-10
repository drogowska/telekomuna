package Xmodem;

import java.util.Vector;

public class Checksum {

    public static byte algebraicSum(Vector<Byte> bytes) {
        int sum = 0;
        for(byte b :bytes) {
            sum += b;
            sum %= 256;
        }
        return (byte) sum;
    }

    public static byte crc16(Vector<Byte> bytes) {  //?
       return 0x00;
    }
}
