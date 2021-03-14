package error.correction;

import java.util.List;
import java.util.Vector;

public class Algorithm {

    private static int[][] H = {    //Macierz H ma nie posiadać kolumny zerowej,
            //identycznych kolumn oraz
            //żadna z kolumn nie może być sumą dwóch innych
            {0,1,1,1,1,1,1,1,   1,0,0,0,0,0,0,0},
            {1,0,1,1,1,1,1,1,   0,1,0,0,0,0,0,0},
            {1,1,0,0,1,1,1,1,   0,0,1,0,0,0,0,0},
            {1,0,1,0,0,1,1,1,   0,0,0,1,0,0,0,0},
            {1,0,0,1,0,0,1,1,   0,0,0,0,1,0,0,0},
            {1,0,0,0,1,0,0,1,   0,0,0,0,0,1,0,0},
            {1,0,0,0,0,1,0,0,   0,0,0,0,0,0,1,0},
            {1,0,0,0,0,0,1,0,   0,0,0,0,0,0,0,1}
    };

    private int m = 8; //zmienna określająca długość wiadomości
    private int n = 8; //zmienna określająca długośc bitów parzystości


    //konwersja znaku ASCI do kodu binarnego zapisanego jako string
    private String AsciToBinaryString(char text) {
        String tmp = "";
        StringBuilder stringBuilder = new StringBuilder();
        tmp += String.valueOf(Integer.valueOf(Integer.toBinaryString(text)));

        if(tmp.length()%8 != 0) {               //uzupełnienie 0 na początku do 8 bitów
            stringBuilder.append(tmp);
            stringBuilder.insert(0,'0');
//            System.out.println("tu");
//            System.out.println(stringBuilder.toString());
        } else {
            stringBuilder.append(tmp);
        }
        return stringBuilder.toString().trim();
    }

    //konwersja wyrazu (ciągu znaków ASCI) do stringa w postaci binarnej
    public String convertCharsToBinaryString(String s) {
        String s1 = "";
        for(int i=0;i<s.length();i++) {
            s1 += AsciToBinaryString(s.charAt(i));
        }
        return s1;
    }

    //konwersja bitów zapisanych w stringu do vektora bitów reprezentowanych przez inty
    public List<Integer> BinaryStringToBinary(String text) {
        List<Integer> v = new Vector<Integer>();
        for( int i=0; i<text.length(); i++ ) {
            v.add(text.charAt(i) == '1'? 1:0);
        }
        return v;
    }

    //konwersja wektora zer i jedenek do stringa zawierającego 0 i 1
    public String BinaryToBinaryString(List<Integer> bin) {
        String string = "";
        for( int i=0; i<bin.size(); i++ ) {
            string += (bin.get(i) == 1 ? '1':'0');
        }
        return string;
    }

    //konwersja wektora zer i jedenek na znaki kodu asci
    public char[] BinaryToAsci(List<Integer> bin) {
        char[] tab = new char[bin.size()/8];        //tablica przechowująca odkodwane znaki
        char znak;
        for (int j=0;j<bin.size()/8;j++){
            znak = 0;
//            for(i=j*8;i<8*(j+1);i++){
            for(int i=0;i<8;i++){
                znak += bin.get(7+(j*8)-i) * Math.pow(2,i); // zamiana zer i jedynek na liczbę w zapisie dzisiętnym, która jest
                // interpretowana jako znak z tablicy ASCI
            }
            tab[j] = znak;
        }
        return tab;
    }


    // funkja wyknywująca mnożenie macierzy H przez wektor oraz dodająca bity parzystości
    private void multiplyVectorByMatrixH(List<Integer> v) {
        List<Integer> tmp = v;    //pomocnicza tablica przechowująca wynik operacji
        for (int i=0; i<n;i++) {
            int sum = 0;
            for (int j = 0; j < v.size(); j++) {
                sum += H[i][j] * tmp.get(i);
            }
            sum %= 2;           //operacje bitowe są wykonywane modulo 2, aby otrzyma otrzymać tylko 0 lub 1
            v.add(sum);         //dodaje bit parzystości na koniec wiadomości
        }
        //return tmp;
    }

    //zakodowanie wiadomości, dodaje bity parzystości, zwraca vektor dwa razy dłuższy niż wektor podany jako parametr
    public List<Integer> Encode(List<Integer> text) {
        List<Integer> tmp =  new Vector<>();
        List<Integer> copy = new Vector<>();
        for(int i=0; i<text.size();i++) {
            tmp.add(text.get(i));
            copy.add(text.get(i));
            if( (i+1) % 8 == 0 && i != 0) {      //wstawienie po każdej literze (1 bajt) bitów parzystości
                multiplyVectorByMatrixH(tmp);
                System.out.println(tmp);
                for(int j=0;j<m;j++) copy.add(tmp.get(j+8));
                tmp.clear();
            }
        }
        return copy;
    }

    public void Correct(List<Integer> T) {
        boolean error = false;      //zmienna określająca czy został znaleziony błąd czy nie
        multiplyVectorByMatrixH(T);

        int[] column = new int[n];
        for(int i=0; i< T.size(); i++) {
            if(T.get(i) != 0 ) error = true;
        }
    }



    //funkcja usuwa bity parzystości z podanego wektora
    public List<Integer> Decode(List<Integer> text) {
        List<Integer> list = new Vector<>();
        int j = 0, n = 0;
        System.out.println(text.size());
        while (j<text.size()){
            list.add(text.get(j));      //przepisanie do pomocniczej tablicy z pomienięciem bitów parzystości
            if((j+1)%8==0){
                j+=9;
                n+=1;
            }
            else j+=1;
        }
        return  list;
    }
    
    private void FixOneErrors(List<Integer> HT, List<Integer> T){ //nie
        List<Integer> column = new Vector<>();
        for(int i=0;i<m+n;i++){
            for(int j=0;j<n;j++){
                column.add(H[j][i]);        //wektor zawierający kolumny macierzy H
                if(column.get(j) == HT.get(j)){      //sprawdzenie czy iloczyn HT ma takie same kolumny co macierz H,
                    T.set(i,(T.get(i) == 1? 0:1));      // jeżeli tak to znaleziono błąd i następuje negacja bitu
                }
            }
            column.clear();
        }
    }
}


