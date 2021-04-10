package Xmodem;

import com.sun.jna.Pointer;
import com.sun.jna.win32.*;
import jtermios.windows.WinAPI;
import jtermios.windows.WinAPI.DCB;
import java.util.Vector;

import static jtermios.windows.WinAPI.*;

public class ReaderWriter {

    private final HANDLE com;
    private final String portName;

    public ReaderWriter(String portName) throws Exception {
        this.portName = portName;
        com = CreateFile(("\\\\.\\" + portName).toString(),
                GENERIC_READ | GENERIC_WRITE,
                0,
                null, //NULL
                OPEN_EXISTING,
                0,
                null); //NULL
        if(com == INVALID_HANDLE_VALUE)
            throw new Exception("Can't open port: " + portName);

        WinAPI.DCB DBC = new DCB();
        DBC.DCBlength = DBC.size();
        DBC.BaudRate = CBR_9600;
        DBC.ByteSize = 8;
        DBC.StopBits = ONESTOPBIT;
        DBC.Parity = NOPARITY;
        SetCommState(com, DBC);

        COMMTIMEOUTS timeouts = new COMMTIMEOUTS();
        timeouts.ReadIntervalTimeout = 5000;		//in ms
        timeouts.ReadTotalTimeoutConstant = 5000;
        timeouts.ReadTotalTimeoutMultiplier = 10;
        timeouts.WriteTotalTimeoutConstant = 50;
        timeouts.WriteTotalTimeoutMultiplier = 10;
        SetCommTimeouts(com, timeouts);
    }

    public void write(Vector<Byte> bytes) throws Exception {
        int[] noOfBytesWritten = new int[bytes.size()];

        WriteFile(com,
                Pointer.createConstant(bytes.get(0)),
                bytes.size(),
                noOfBytesWritten,
                null);
        if(noOfBytesWritten.length != bytes.size()){
            throw new Exception("Can't write bytes!");
        }
    }


    public byte read() throws Exception {
        byte tmp = 0;
        int[] noOfBytesRead = new int[128];

        ReadFile(com,
                Pointer.createConstant(tmp),
                tmp,
		        noOfBytesRead,
                null);
        if(noOfBytesRead.length == 0){
            throw new Exception("Can't read byte!");
        }
        return tmp;
    }

}
