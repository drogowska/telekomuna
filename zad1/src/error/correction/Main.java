package error.correction;

//import error.correction.Algorithm;
import java.io.BufferedReader;
import java.io.FileReader;
import java.io.IOException;
import java.io.PrintWriter;
import java.nio.charset.StandardCharsets;
import java.util.List;
import java.util.Vector;

public class Main {

    public static void main(String[] args) throws IOException {
        Algorithm algorithm = new Algorithm();

        System.setProperty("file.encoding", "UTF-8");

        String filePath = "./message";
        String binaryFile = "./binary.txt";
        String encodedFile = "./encoded";
        String correctedFile = "./corrected.txt";

     //   Charset UTF_8 = Charset.forName("UTF-8");

        BufferedReader fileReader = new BufferedReader(new FileReader(filePath));
        String message = fileReader.readLine();         //odczyt z pliku wiadomości
        System.out.println(message);



        PrintWriter BinWriter = new PrintWriter(binaryFile);
        //writer.println(algorithm.AsciToBinaryString(message));      //zapis do pliku binarnego zapisu wiadomości (string)
        BinWriter.print(algorithm.convertCharsToBinaryString(message));
//        System.out.println(algorithm.convertCharsToBinaryString(message));
        BinWriter.close();
        //System.out.println(Integer.toBinaryString(32));

        PrintWriter EncWriter = new PrintWriter(encodedFile);
        Vector<Integer> V = (Vector<Integer>) algorithm.BinaryStringToBinary(algorithm.convertCharsToBinaryString(message));
        //System.out.println("to str" + V.toString());
//        System.out.println(algorithm.BinaryToAsci(V));
//
//        System.out.println(algorithm.BinaryToBinaryString(algorithm.Encode(V)));
        //EncWriter.print(algorithm.BinaryToBinaryString(algorithm.Encode(V)));
        //EncWriter.print(algorithm.Encode(V));
        List<Integer> en = algorithm.Encode(V);
        EncWriter.print(algorithm.BinaryToAsci(en));
        //EncWriter.print(new String(algorithm.BinaryToAsci(en).getBytes(), StandardCharsets.US_ASCII));
        System.out.println("aa  " + algorithm.BinaryToBinaryString(algorithm.Encode(V)) );


        //System.out.println("bb" + new String(algorithm.BinaryToAsci(en).getBytes(), StandardCharsets.US_ASCII));
        System.out.println("c " + (algorithm.BinaryToAsci(en)));
        EncWriter.close();
        System.out.println("encode "+algorithm.Encode(V));


        //System.out.println(en.equals(algorithm.Encode(V)));
        System.out.println("decode " + algorithm.Decode(en));
        System.out.println(algorithm.BinaryToAsci(algorithm.Decode(en)));
        System.out.println("ss " + algorithm.BinaryToAsci(algorithm.Decode(algorithm.prepareStringToList(message))));
        List<Integer> v1 = en;
        System.out.println("V" + en);
        System.out.println("v1" + v1);
//        v1.set(1,0);
//        v1.set(0,1);
        System.out.println("w1" + algorithm.BinaryToAsci(v1));
        System.out.println("v1" + v1);
        //algorithm.Correct(v1);

        PrintWriter corWriter = new PrintWriter(correctedFile);
        System.out.print("");
        System.out.println("correct" + algorithm.Correct(v1));
        corWriter.write(algorithm.BinaryToAsci(algorithm.Decode(algorithm.Correct(v1))));
        corWriter.close();
        System.out.println(algorithm.Decode(algorithm.Correct(v1)));
        System.out.println(algorithm.BinaryToAsci(algorithm.Decode(algorithm.Correct(v1))));


    }
}

