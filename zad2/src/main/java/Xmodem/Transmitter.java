package Xmodem;

import java.util.Vector;

public class Transmitter {
    static final byte SOH = 0x01;
    static final byte EOT = 0x04;
    static final byte ACK = 0x06;
    static final byte NAK = 0x15;
    static final byte CAN = 0x18;
    static final byte C = 0x43;

    private ReaderWriter readerWriter;
    boolean start = false;

    public Transmitter(ReaderWriter readerWriter) {
        this.readerWriter = readerWriter;
    }

    public void sendFile(Vector<Byte> data, boolean checksum) throws Exception {
        if(checksum)                            //oczekiwanie na odbiornik
            while (readerWriter.read() != NAK);
        else //crc
            while (readerWriter.read() != C);

        int noOfBlock = Math.round(data.size()/128);

        for(int i = 0; i< noOfBlock; i++){
            Vector<Byte> tmp = new Vector<>();
            for(int j=i*128;j<(i+1)*128;j++) {
                tmp.add(data.get(j));
            }
            if(tmp.size() < 128) {
                for (int j = tmp.size(); j < 128; j++)
                    tmp.add((byte) 0);               //dopełnienie zerami
            }
            Vector<Byte> header = createHeader(i);

            readerWriter.write(header);             //wysłanie
            readerWriter.write(tmp);

            if(checksum) {
                readerWriter.write(new Vector<>(Checksum.algebraicSum(tmp)));
            } else {
                readerWriter.write(new Vector<>(Checksum.crc16(tmp)));
            }

            byte response = readerWriter.read();
            if(response == NAK) {
                i--;
            } else if(response == CAN) {
                throw new Exception("Connection canceled!");
            } else if(response == ACK){
                throw new Exception("Protocol error.");
            }

        }
        readerWriter.write(new Vector<>(EOT));
    }

    //tworzenie nagłówka
    private Vector<Byte> createHeader(int i){
        Vector<Byte> header =new Vector<>();
        header.add(SOH);                        //tworzenie nagłówka
        header.add((byte) (i+1));
        header.add((byte) (255 - (i+1)));
        return header;
    }

}
