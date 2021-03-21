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

    @FXML
    public void onActionLoadFileSender(ActionEvent actionEvent) throws IOException {
        loadFile(textAreaSender);
    }

    @FXML
    public void onActionEncode(ActionEvent actionEvent) throws FileNotFoundException {
        // zapis do pliku zakodowanej wiadomosci
        FileChooser fileChooser = new FileChooser();
        File file = fileChooser.showSaveDialog(null);

        if(file != null){
            List<Integer> text = algorithm.prepareStringToList(textAreaReceiver.getText());
            List<Integer> codedText = algorithm.Encode(text);
            String newText = algorithm.BinaryToBinaryString(codedText);
            SaveFile(newText, file);
        }
    }

    @FXML
    public void onActionLoadFileReceiver(ActionEvent actionEvent) throws IOException {
        loadFile(textAreaReceiver);
    }

    @FXML
    public void onActionRepair(ActionEvent actionEvent) throws IOException {
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

    private void SaveFile(String content, File file){
        try {
            FileWriter fileWriter;

            fileWriter = new FileWriter(file);
            fileWriter.write(content);
            fileWriter.close();
        } catch (IOException ex) {
            System.out.println(ex);
        }

    }
}
