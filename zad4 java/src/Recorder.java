import javax.sound.sampled.*;
import java.io.*;

public class Recorder {
    File wavFile;                              //plik w którym ma zostać zapisane audio
    AudioFileFormat.Type fileType = AudioFileFormat.Type.WAVE;      //ustawienie formatu pliku audio na plik wav
    //tworzenie obiektu klasy AudioForamt korzystającego z liniwego kodowania PCM (pulse code modulation)
    AudioFormat format = new AudioFormat(16000,       //liczba próbek na sekundę
                                        8,         //liczba bitów w każdej próbce dźwięku
                                             2,         //liczba kanałów audio
                                            true,         //określenie czy dane są podpisane czy nie
                                          true);        //określenie czy dane dla pojedynczej próbki są przechowywane w kolejności bajtów big-endian, czy little-endian
    TargetDataLine targetDataLine;             //zmienna do zapisu audio, linia danych do przehwycenia z urządzenie dźwięku

    public Recorder(String wavFile) {
        this.wavFile = new File(wavFile);               //stworzenie pliku o podanej nazwie, wraz z rozszerzeniem
    }

    //funkcja pobirająca dźwięk z urządzenia i zapisująća go do podanego pliku
    public void startRecording() {
        DataLine.Info line = new DataLine.Info(TargetDataLine.class, format);           //zapisanie do zmiennej informacje o linii będącej obiektem klast TragetDataLine i o określonym formacie
        try {
            targetDataLine = (TargetDataLine) AudioSystem.getLine(line);            //zapisanie do zmiennej lini danych pasującego do opisu z obiektu DataLine.Info
            targetDataLine.open(format);                                            //otwarcie lini o podanym formacie
            targetDataLine.start();                                                 //przechwytywanie dźwięku

            AudioInputStream input = new AudioInputStream(targetDataLine);          //stworzenie obiektu strumienia wyjściowego audio, który odczytuje dane z podanej linii docelowej
            AudioSystem.write(input, fileType, wavFile);                            //zapisanie strumienia bajtów, zawierających wiadomość audio, do pliu o podanym formacie
        } catch (LineUnavailableException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
    //funkcja odtwarzająca dzwięk z pliku wav
    public void playSound(String filename){
        File file = new File(filename);
        try {
            AudioInputStream audioStream = AudioSystem.getAudioInputStream(file);           //odczytanie strumienia bajtów audio z podanego pliku i zapisanie do zmiennej
            AudioFormat audioFormat = audioStream.getFormat();                              //odczytanie formatu danych zapisanych w strumieniu
            DataLine.Info info = new DataLine.Info(SourceDataLine.class, audioFormat);      //zapisanie do zmiennej informacje o linii będącej obiektem klast SourceDataLine i o określonym formacie
            SourceDataLine sourceDataLine = (SourceDataLine) AudioSystem.getLine(info);     //zapisanie do zmiennej lini danych pasującego do opisu z obeiktu DataLine.Info
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
            sourceDataLine.drain();                                         //zamknięcie strumienii audio
            sourceDataLine.close();
            audioStream.close();
        } catch (UnsupportedAudioFileException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        } catch (LineUnavailableException e) {
            e.printStackTrace();
        }

    }

    public void closeRecord() {
        targetDataLine.stop();  //zamknięcie strumieni
        targetDataLine.close();

    }

    public static void main(String[] arg) throws InterruptedException {
        Recorder recorder = new Recorder("plik");
//        recorder.startRecording();
//        TimeUnit.SECONDS.sleep(30);
//        recorder.closeRecord();

        Thread thread = new Thread(()->{
                try {
                    Thread.sleep(30000); //ms
                } catch (InterruptedException ex) {
                    ex.printStackTrace();
                }
                recorder.closeRecord();
            }
        );

        thread.start();

        // start recording
        recorder.startRecording();
        recorder.playSound("plik.wav");
    }
}
