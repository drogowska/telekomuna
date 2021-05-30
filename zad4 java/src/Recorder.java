import javafx.concurrent.Task;

import javax.sound.sampled.*;
import java.io.*;

public class Recorder extends Task<Void>  {
    File wavFile;                              //plik w którym ma zostać zapisane audio
    AudioFileFormat.Type fileType = AudioFileFormat.Type.WAVE;      //ustawienie formatu pliku audio na plik wav
    //tworzenie obiektu klasy AudioForamt korzystającego z liniwego kodowania PCM (pulse code modulation)
    AudioFormat format = new AudioFormat(16000,       //liczba próbek na sekundę
                                        8,         //liczba bitów w każdej próbce dźwięku
                                             2,         //liczba kanałów audio
                                            true,         //określenie czy dane są podpisane czy nie
                                          true);        //określenie czy dane dla pojedynczej próbki są przechowywane w kolejności bajtów big-endian, czy little-endian
    TargetDataLine targetDataLine;             //zmienna do zapisu audio, linia danych do przehwycenia z urządzenie dźwięku
    double duration;                            //zmienna przechowująca długość nagrania

    public double getDuration() {
        return duration;
    }

    public Recorder(String wavFile) {
        this.wavFile = new File(wavFile);               //stworzenie pliku o podanej nazwie, wraz z rozszerzeniem
    }

    //funkcja pobirająca dźwięk z urządzenia i zapisująća go do podanego pliku
    public void run() {
        DataLine.Info line = new DataLine.Info(TargetDataLine.class, format);           //zapisanie do zmiennej informacje o linii będącej obiektem klast TragetDataLine i o określonym formacie
        try {
            if (!AudioSystem.isLineSupported(line)) {                               //sprawdzenie czy system wspiera linie danych
                System.out.println("Linia nie wspierana");
                System.exit(0);
            }
            targetDataLine = (TargetDataLine) AudioSystem.getLine(line);            //zapisanie do zmiennej lini danych pasującego do opisu z obiektu DataLine.Info
            targetDataLine.open(format);                                            //otwarcie lini o podanym formacie
            targetDataLine.start();                                                 //przechwytywanie dźwięku
            System.out.println("Rozpoczęto nagrywanie...");
            AudioInputStream input = new AudioInputStream(targetDataLine);          //stworzenie obiektu strumienia wyjściowego audio, który odczytuje dane z podanej linii docelowej
            AudioSystem.write(input, fileType, wavFile);                            //zapisanie strumienia bajtów, zawierających wiadomość audio, do pliu o podanym formacie
            long milliseconds = (long) ((input.getFrameLength() * 1000) / format.getFrameRate());
            duration = milliseconds /1000.0;                //obliczenie czasu trwania nagrania
        } catch (LineUnavailableException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void closeRecord() {
        targetDataLine.stop();  //zamknięcie strumieni
        targetDataLine.close();
        System.out.println("Zakończono nagrywanie...");
    }


    @Override
    protected Void call() {                //funkcja uruchamia nagrywanie w tle
        this.run();
        return null;
    }
}
