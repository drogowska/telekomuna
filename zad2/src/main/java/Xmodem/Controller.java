package Xmodem;

import javafx.fxml.FXML;
import javafx.scene.control.RadioButton;

public class Controller {

    @FXML
    RadioButton checksum;
    @FXML
    RadioButton crc16;

    private boolean crc;

    @FXML
    private void chooseChecksum() {
        if (this.checksum.isSelected()) {
            crc = true;
        } else {
            crc = false;
        }
    }
}
