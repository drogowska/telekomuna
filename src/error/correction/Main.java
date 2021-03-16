package error.correction;

//import error.correction.Algorithm;
import java.io.*;
import java.util.List;
import java.util.Vector;

public class Main {

    public static void main(String[] args) throws IOException {
        Algorithm algorithm = new Algorithm();

        System.setProperty("file.encoding", "UTF-8");

        String filePath = "./message.txt";
        String binaryFile = "./binary.txt";
        String encodedFile = "./encoded.txt";
        String correctedFile = "./corrected.txt";

        BufferedReader fileReader = new BufferedReader(new FileReader(filePath));
        String message = fileReader.readLine();         //odczyt z pliku wiadomości
        System.out.println(message);
//        byte[] t = message.getBytes();
//        for(byte i : t)
//        {
//            System.out.println("binary " + i );
//            System.out.println("a " + Integer.toBinaryString(i));
//        }



        PrintWriter BinWriter = new PrintWriter(binaryFile);
        //writer.println(algorithm.AsciToBinaryString(message));      //zapis do pliku binarnego zapisu wiadomości (string)
        BinWriter.print(algorithm.convertCharsToBinaryString(message));
//        System.out.println(algorithm.convertCharsToBinaryString(message));
        BinWriter.close();

        PrintWriter EncWriter = new PrintWriter(encodedFile);
        Vector<Integer> V = (Vector<Integer>) algorithm.BinaryStringToBinary(algorithm.convertCharsToBinaryString(message));
//        System.out.println(V);
//        System.out.println(algorithm.BinaryToAsci(V));
//
//        System.out.println(algorithm.BinaryToBinaryString(algorithm.Encode(V)));
        EncWriter.print(algorithm.BinaryToBinaryString(algorithm.Encode(V)));
        EncWriter.close();
        System.out.println("encode "+algorithm.Encode(V));

        List<Integer> en = algorithm.Encode(V);
        //System.out.println(en.equals(algorithm.Encode(V)));
        System.out.println("decode " + algorithm.Decode(en));
        System.out.println(algorithm.BinaryToAsci(algorithm.Decode(en)));

        List<Integer> v1 = en;
        System.out.println("V" + en);
        System.out.println("v1" + v1);
        v1.set(18,0);
        v1.set(19,1);
        System.out.println("v1" + v1);
        //algorithm.Correct(v1);

        PrintWriter corWriter = new PrintWriter(correctedFile);
        System.out.print("");
        System.out.println("correct" + algorithm.Correct(v1));
        corWriter.write(algorithm.BinaryToBinaryString(v1));


    }
}

