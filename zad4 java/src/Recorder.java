import javax.sound.sampled.*;
import java.io.*;
import java.util.concurrent.TimeUnit;

public class Recorder {
    File wavFile;
    AudioFileFormat.Type fileType = AudioFileFormat.Type.WAVE;
    TargetDataLine line;
    //ustawienie parametrów mikrofonu
    AudioFormat format = new AudioFormat(16000, 8, 2, true, true);
    //
    TargetDataLine targetDataLine;
    public Recorder(File wavFile) {
        this.wavFile = wavFile;
    }

    public void startRecording() {
        DataLine.Info line = new DataLine.Info(TargetDataLine.class, format);
        try {
            targetDataLine = (TargetDataLine) AudioSystem.getLine(line);
            targetDataLine.open(format);
            targetDataLine.start();

            AudioInputStream input = new AudioInputStream(targetDataLine);
            AudioFileFormat.Type fileType = AudioFileFormat.Type.WAVE;
            AudioSystem.write(input, fileType, wavFile);

        } catch (LineUnavailableException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void playSound(String filename){
        File file = new File(filename);
        try {
            AudioInputStream audioStream = AudioSystem.getAudioInputStream(file);
            AudioFormat audioFormat = audioStream.getFormat();
            DataLine.Info info = new DataLine.Info(SourceDataLine.class, audioFormat);
            SourceDataLine sourceDataLine = (SourceDataLine) AudioSystem.getLine(info);
            sourceDataLine.open(audioFormat);
            sourceDataLine.start();
            int nBytesRead = 0;
            byte[] abData = new byte[128000];
            while (nBytesRead != -1) {
                try {
                    nBytesRead = audioStream.read(abData, 0, abData.length);
                } catch (IOException e) {
                    e.printStackTrace();
                }
                if (nBytesRead >= 0) {
                    int nBytesWritten = sourceDataLine.write(abData, 0, nBytesRead);
                }
            }
            sourceDataLine.drain();
            sourceDataLine.close();
        } catch (UnsupportedAudioFileException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        } catch (LineUnavailableException e) {
            e.printStackTrace();
        }

    }

    public void closeRecord() {
        line.stop();
        line.close();
        targetDataLine.close();
    }

    public static void main(String[] arg) throws InterruptedException {
        Recorder recorder = new Recorder(new File("plik.wav"));
//        recorder.startRecording();
//        TimeUnit.SECONDS.sleep(30);
//        recorder.closeRecord();

        Thread stopper = new Thread(new Runnable() {
            public void run() {
                try {
                    Thread.sleep(60000); //ms
                } catch (InterruptedException ex) {
                    ex.printStackTrace();
                }
                recorder.closeRecord();
            }
        });

        stopper.start();

        // start recording
        recorder.startRecording();
        recorder.playSound("odb.wav");
    }
}
