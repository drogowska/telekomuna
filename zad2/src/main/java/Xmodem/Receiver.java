package Xmodem;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.time.Duration;
import java.time.LocalTime;

import java.util.Vector;
import java.util.concurrent.TimeUnit;

public class Receiver {

    static final byte SOH = 0x01;
    static final byte EOT = 0x04;
    static final byte ACK = 0x06;
    static final byte NAK = 0x15;
    static final byte CAN = 0x18;
    static final byte C = 0x43;

//    private final InputStream inputStream;
//    private final OutputStream outputStream;

    private ReaderWriter readerWriter;
    boolean start = false;


    public Receiver(ReaderWriter rw) {
        readerWriter = rw;
    }


    private void startTransmission(boolean checksum) throws Exception {
        LocalTime time = LocalTime.now();
        //Duration duration;
        Vector<Byte> data = new Vector<>();

        while (Duration.between(time, LocalTime.now()).getSeconds() < 60) {
            if (checksum) { //suma algebraiczna
                readerWriter.write(new Vector<Byte>(NAK));
            } else { //crc
                readerWriter.write(new Vector<Byte>(C));
            }
            if (readerWriter.read() == SOH) {
                start = true;
                break;
            }
            TimeUnit.SECONDS.sleep(10);
            //sendByte(NAK);

        }
    }

    private Vector<Byte> receiveFile(boolean checksum) throws Exception {
        Vector<Byte> data = new Vector<>();
        startTransmission(checksum);

        if (start) {
            byte b = SOH;
            do {
                Vector<Byte> block = new Vector<>();
                for (int i = 0; i < 128; i++) {
                    block.add(readerWriter.read());
                }
                if (checksum) {
                    checkChecksum(block, data);
                } else { //crc16
                    checkCrc16(block, data);
                }

            } while (b == SOH);
            if (b == EOT) {
                readerWriter.write(new Vector<>(ACK));
            } else throw new Exception("Protocol error");
        }

        return data;
    }

    private void checkChecksum(Vector<Byte> block, Vector<Byte> data) throws Exception {
        byte check = readerWriter.read();
        if (Checksum.algebraicSum(block) == check) {
            readerWriter.write(new Vector<>(ACK));
            for (byte c : block) data.add(c); //?
        } else {
            readerWriter.write(new Vector<>(NAK));
        }
    }

    private void checkCrc16(Vector<Byte> block, Vector<Byte> data) {

    }
}

//    private byte readByte() throws IOException {
//        while (true) {
//            if (inputStream.available() > 0) {
//                int b = inputStream.read();
//                return (byte) b;
//            }
//            shortSleep();
//        }
//    }
//
//    public void sendByte(byte b) throws IOException {
//        outputStream.write(b);
//        outputStream.flush();
//    }
//
//    private void shortSleep() {
//        try {
//            Thread.sleep(10);
//        } catch (InterruptedException e) {
//            try {
//                interruptTransmission();
//            } catch (IOException ignore) {
//            }
//            throw new RuntimeException("Transmission was interrupted", e);
//        }
//    }
//
//    public void interruptTransmission() throws IOException {
//        sendByte(CAN);
//        sendByte(CAN);
//    }
//
//    public static void main(String[] args) throws InterruptedException {
//        //start();
//    }
//}
