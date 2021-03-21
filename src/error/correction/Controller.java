package error.correction;

import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.scene.control.TextArea;
import javafx.stage.FileChooser;

import java.io.*;
import java.util.List;

public class Controller {
    public TextArea textAreaSender;
    public TextArea textAreaReceiver;
    Algorithm algorithm = new Algorithm();
    private String loadedText;

    @FXML
    public void onActionLoadFileSender(ActionEvent actionEvent) throws IOException {
        loadFile(textAreaSender);
        loadedText = textAreaSender.getText();
    }

    @FXML
    public void onActionEncode(ActionEvent actionEvent){
        List<Integer> tmp = algorithm.prepareStringToList(loadedText);
        List<Integer> codedTextAsList = algorithm.Encode(tmp);
        String codedTextAsString = new String(algorithm.BinaryToAsci(codedTextAsList));

        FileChooser fileChooser = new FileChooser();
        File file = fileChooser.showSaveDialog(null);

        if(file != null){
            saveTextToFile(codedTextAsString, file);
        }
    }

    @FXML
    public void onActionLoadFileReceiver(ActionEvent actionEvent) throws IOException {
        loadFile(textAreaReceiver);
    }

    @FXML
    public void onActionRepair(ActionEvent actionEvent){
        List<Integer> textToCorrect = algorithm.prepareStringToList(textAreaReceiver.getText());
        String correctedText = algorithm.Correct(textToCorrect);
        textAreaReceiver.setText(correctedText);
    }

    private void loadFile(TextArea textArea) throws IOException {
        FileChooser fileChooser = new FileChooser();
        File selectedFile = fileChooser.showOpenDialog(null);
        String fullPath = selectedFile.getAbsolutePath();
        if (selectedFile != null) {
            BufferedReader fileReader = new BufferedReader(new FileReader(fullPath));
            String message = fileReader.readLine();
            textArea.setText(message);
        }
    }

    private void saveTextToFile(String content, File file) {
        try {
            PrintWriter writer;
            writer = new PrintWriter(file);
            writer.println(content);
            writer.close();
        } catch (IOException ex) {
            System.out.println(ex);
        }
    }
}
