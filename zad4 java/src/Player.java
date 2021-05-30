import javax.sound.sampled.*;
import java.io.File;
import java.io.IOException;

public class Player {

    static private  SourceDataLine sourceDataLine;
    static private AudioInputStream audioStream;

    //funkcja odtwarzająca dzwięk z pliku wav
    public static void playSound(String filename){
        File file = new File(filename);
        try {
            audioStream = AudioSystem.getAudioInputStream(file);           //odczytanie strumienia bajtów audio z podanego pliku i zapisanie do zmiennej
            AudioFormat audioFormat = audioStream.getFormat();                              //odczytanie formatu danych zapisanych w strumieniu
            DataLine.Info info = new DataLine.Info(SourceDataLine.class, audioFormat);      //zapisanie do zmiennej informacje o linii będącej obiektem klast SourceDataLine i o określonym formacie
            sourceDataLine = (SourceDataLine) AudioSystem.getLine(info);     //zapisanie do zmiennej lini danych pasującego do opisu z obeiktu DataLine.Info
            sourceDataLine.open(audioFormat);                                               //otwarcie lini o podanym formacie
            sourceDataLine.start();                                                         //
            int nBytesRead = 0;                                                             //zmienna przechowująca ilość odczytanychdan
            byte[] abData = new byte[128000];                                               //buffor do którego wczytywane są odczytane dane
            while (nBytesRead != -1) {                          //gdy nBytesRead wynosi -1 to znaczy że zostały odczytane wszystkie bajty ze strumienia
                try {
                    nBytesRead = audioStream.read(abData, 0, abData.length);            //odczytanie abData.length ilości znaków z tablicy abData, bez przesunięcia
                } catch (IOException e) {
                    e.printStackTrace();
                }
                if (nBytesRead >= 0) {                                                      //jeśli odczytano jakieś dane
                    int nBytesWritten = sourceDataLine.write(abData, 0, nBytesRead);     //to zapisuje bajy, z tablicy abData o długości odczytanych bajtów, do miksera
                }
            }

        } catch (UnsupportedAudioFileException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        } catch (LineUnavailableException e) {
            e.printStackTrace();
        }
    }

    static public void stop() {
        try {
            sourceDataLine.drain();                                         //zamknięcie strumienii audio
            sourceDataLine.close();
            audioStream.close();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
