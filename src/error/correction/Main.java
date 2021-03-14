package error.correction;

import error.correction.Algorithm;

import java.io.*;
import java.util.Vector;

public class Main {

    public static void main(String[] args) throws IOException {
        Algorithm algorithm = new Algorithm();

        System.setProperty("file.encoding", "UTF-8");

        String filePath = "./message.txt";
        String binaryFile = "./binary.txt";
        String encodedFile = "./encoded.txt";

        BufferedReader fileReader = new BufferedReader(new FileReader(filePath));
        String message = fileReader.readLine();         //odczyt z pliku wiadomości
        System.out.println(message);


        PrintWriter BinWriter = new PrintWriter(binaryFile);
        //writer.println(algorithm.AsciToBinaryString(message));      //zapis do pliku binarnego zapisu wiadomości (string)
        BinWriter.print(algorithm.convertCharsToBinaryString(message));
        System.out.println(algorithm.convertCharsToBinaryString(message));
        BinWriter.close();

        PrintWriter EncWriter = new PrintWriter(encodedFile);
        Vector<Integer> V = (Vector<Integer>) algorithm.BinaryStringToBinary(algorithm.convertCharsToBinaryString(message));
        System.out.println(V);
        System.out.println(algorithm.BinaryToAsci(V));

        System.out.println(algorithm.BinaryToBinaryString(algorithm.Encode(V)));
        EncWriter.print(algorithm.BinaryToBinaryString(algorithm.Encode(V)));
        EncWriter.close();
        System.out.println("encode "+algorithm.Encode(V));

        System.out.println("decode " + algorithm.Decode(algorithm.Encode(V)));
        System.out.println(algorithm.BinaryToAsci(algorithm.Decode(algorithm.Encode(V))));



    }
}

