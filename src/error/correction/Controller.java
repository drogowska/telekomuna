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
        //List<Integer> codedTextAsList = algorithm.Encode(tmp);
        //textToFix = new String(algorithm.BinaryToAsci(codedTextAsList));

        //Vector<Integer> V = (Vector<Integer>) algorithm.BinaryStringToBinary(algorithm.convertCharsToBinaryString(loadedText));
        //Vector<Integer> V =

        List<Integer> codedTextAsList = algorithm.Encode(tmp);//algorithm.BinaryToAsci(codedTextAsList));
        list = codedTextAsList;


        textToFix = new String(algorithm.BinaryToAsci(codedTextAsList));

        FileChooser fileChooser = new FileChooser();
        File file = fileChooser.showSaveDialog(null);

        if(file != null){
            saveTextToFile(new String(algorithm.BinaryToAsci(codedTextAsList)), file);
        }

    }
    private List<Integer> test(List<Integer> list) {
        for(int i=0;i<list.size();i++){
                if(i%8 == 0) {
                    if (this.list.get(i) != list.get(i)) {
                        list.set(i, 1);
                    }
                }
        }
        return list;
    }

    @FXML
    public void onActionLoadFileReceiver(ActionEvent actionEvent) throws IOException {
        FileChooser fileChooser = new FileChooser();
        File selectedFile = fileChooser.showOpenDialog(null);
        String fullPath = selectedFile.getAbsolutePath();
        if (selectedFile != null) {
            BufferedReader fileReader = new BufferedReader(new FileReader(fullPath));
            String message = fileReader.readLine();
            List<Integer> v = algorithm.prepareStringToList(message);//algorithm.BinaryStringToBinary(message);

            v = test(v);
            list = v;


            textToFix = new String(algorithm.BinaryToAsci(v));
            String tmp = new String(algorithm.BinaryToAsci(algorithm.Decode(v)));
            textAreaReceiver.setText(tmp);

        }
    }

    @FXML
    public void onActionRepair(ActionEvent actionEvent){
        List<Integer> correctedText = algorithm.Correct(list);
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
