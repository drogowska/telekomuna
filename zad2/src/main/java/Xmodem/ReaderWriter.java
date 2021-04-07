package Xmodem;

import com.sun.jna.Pointer;
import com.sun.jna.win32.*;
import jtermios.windows.WinAPI;
import jtermios.windows.WinAPI.DCB;
import static jtermios.windows.WinAPI.*;

public class ReaderWriter {

    private HANDLE com;
    private final String portName;

    public ReaderWriter(String portName) throws Exception {
        this.portName = portName;
        com = CreateFile(("\\\\.\\" + portName).toString(),
                GENERIC_READ | GENERIC_WRITE,
                0,
                new SECURITY_ATTRIBUTES(), //NULL
                OPEN_EXISTING,
                0,
                Pointer.NULL); //NULL
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
}
