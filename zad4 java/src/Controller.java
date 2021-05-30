import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.scene.control.Button;
import javafx.scene.control.TextArea;
import javafx.stage.FileChooser;

import java.io.File;

public class Controller {
    @FXML
    private Button stop;
    @FXML
    private Button start;
    @FXML
    private Button play;

    String fullPath = "";
    Recorder recorder;
    Thread thread;

    @FXML
    private void start() {
        recorder =  new Recorder(fullPath);
        thread = new Thread(recorder);
        thread.start();
        start.setDisable(true);
        stop.setDisable(false);
        System.out.println("start");
    }

    @FXML
    private void play() {
        if(play.getText().equals("Odtwórz")) {
            Player.playSound(fullPath);
            play.setText("Zatrzymaj");
        } else {
            Player.stop();
            play.setText("Odtwórz");
        }
    }

    @FXML
    public void stop() {
        stop.setDisable(true);
        start.setDisable(false);
        recorder.closeRecord();
        recorder.cancel();
        thread = null;
    }

    @FXML
    public void chooseFile() {
        FileChooser fileChooser = new FileChooser();
        File selectedFile = fileChooser.showOpenDialog(null);
        fullPath = selectedFile.getAbsolutePath();

    }
}
