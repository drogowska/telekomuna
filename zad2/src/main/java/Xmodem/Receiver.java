package Xmodem;

import java.sql.SQLOutput;
import java.sql.Time;
import java.time.Clock;
import java.time.Duration;
import java.time.LocalTime;
import java.time.temporal.ChronoUnit;
import java.util.Date;
import java.util.Locale;

import java.util.concurrent.TimeUnit;

public class Receiver {

    static final byte SOH = 0x01;
    static final byte EOT = 0x04;
    static final byte ACK = 0x06;
    static final byte NAK = 0x15;
    static final byte CAN = 0x18;
    static final byte C   = 0x43;




    private static void start() throws InterruptedException {
        LocalTime time = LocalTime.now();
        Duration duration;
        do {
            duration = Duration.between(time,LocalTime.now());
            TimeUnit.SECONDS.sleep(10);

            System.out.println(duration.getSeconds()); //wysyła NACK

        } while (duration.getSeconds() < 60);

    }

    public static void main(String[] args) throws InterruptedException {
        start();
    }
}
