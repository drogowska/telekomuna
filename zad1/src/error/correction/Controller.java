package error.correction;

import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.scene.control.TextArea;
import javafx.stage.FileChooser;

import java.io.*;
import java.util.List;
import java.util.Vector;

public class Controller {
    public TextArea textAreaSender;
    public TextArea textAreaReceiver;
    Algorithm algorithm = new Algorithm();
    private String loadedText;
    private String textToFix;
    private List<Integer> list;

    @FXML
    public void onActionLoadFileSender(ActionEvent actionEvent) throws IOException {
        loadFile(textAreaSender);
        loadedText = textAreaSender.getText();
    }

    @FXML
    public void onActionEncode(ActionEvent actionEvent){
        List<Integer> tmp = algorithm.prepareStringToList(loadedText);
        List<Integer> codedTextAsList = algorithm.Encode(tmp);

        list = codedTextAsList;

        textToFix = new String(algorithm.BinaryToAsci(codedTextAsList));

        FileChooser fileChooser = new FileChooser();
        File file = fileChooser.showSaveDialog(null);

        if(file != null){
            saveTextToFile(new String(algorithm.BinaryToAsci(codedTextAsList)), file);
        }

    }
    private List<Integer> test(List<Integer> lista) {
        for(int i=8;i<lista.size();i+=16){
            if (this.list.get(i) != lista.get(i)) {
                lista.set(i, 1);
            }
        }
        return lista;
    }

    @FXML
    public void onActionLoadFileReceiver(ActionEvent actionEvent) throws IOException {
        FileChooser fileChooser = new FileChooser();
        File selectedFile = fileChooser.showOpenDialog(null);
        String fullPath = selectedFile.getAbsolutePath();
        if (selectedFile != null) {
            BufferedReader fileReader = new BufferedReader(new FileReader(fullPath));
            textToFix = fileReader.readLine();
            String m = algorithm.prepareListToString(algorithm.Decode(algorithm.prepareStringToList(textToFix)));
            textAreaReceiver.setText(m);
        }
    }

    @FXML
    public void onActionRepair(ActionEvent actionEvent){
        List<Integer> toFix = algorithm.prepareStringToList(textToFix);
        toFix = test(toFix);
        List<Integer> correctedText = algorithm.Correct(toFix);
        String tmp = new String(algorithm.BinaryToAsci(algorithm.Decode(correctedText)));

        textAreaReceiver.setText(tmp);
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
            FileWriter fw = new FileWriter(file);
            fw.write(content);
            fw.close();
        } catch (IOException ex) {
            System.out.println(ex);
        }
    }
}
