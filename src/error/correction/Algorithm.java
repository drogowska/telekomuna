package error.correction;

import java.util.List;
import java.util.Vector;

public class Algorithm {

    //Dmin = 4 dla korekcji 1 błędu ale detekcji 2, a Dmin= 5 dla korekcji 2 błędów

    private static int[][] H = {    //Macierz H (parzystości) ma nie posiadać kolumny zerowej,
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

    public void vert(byte[] list) {
        List<Integer> tab = new Vector<>();
        for(byte b : list) {
           // tab.add(Integer.toBinaryString(Integer.bitCount()))
        }
    }

    //konwersja znaku ASCI do kodu binarnego zapisanego jako string
    private String AsciToBinaryString(char text) {
        String tmp = "";
        StringBuilder stringBuilder = new StringBuilder();
        tmp += String.valueOf(Integer.valueOf(Integer.toBinaryString(text)));

        if(tmp.length()%8 != 0) {               //uzupełnienie 0 na początku do 8 bitów
            stringBuilder.append(tmp);
            stringBuilder.insert(0,'0');
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
            for(int i=0;i<8;i++){
                znak += bin.get(7+(j*8)-i) * Math.pow(2,i); // zamiana zer i jedynek na liczbę w zapisie dzisiętnym, która jest
                                                            // interpretowana jako znak z tablicy ASCI
            }
            tab[j] = znak;
        }
        return tab;
    }


    // funkja wyknywująca mnożenie macierzy H przez wektor oraz dodająca bity parzystości
    private List<Integer> AddParityBites(List<Integer> v, int size) {
        List<Integer> tmp = new Vector<>(v.size());    //pomocnicza tablica przechowująca wynik operacji
        for(int i=0;i<v.size();i++) {
            tmp.add(v.get(i));
        }

        for (int i=0; i<n;i++) {
            int sum = 0;
            for (int j = 0; j <size; j++) {
                //System.out.println(v.get(j));
                sum += H[i][j] * v.get(j);            //wiersz H * kolumna v
            }
            sum %= 2;             //operacje bitowe są wykonywane modulo 2, aby otrzyma otrzymać tylko 0 lub 1
            tmp.add(sum);         //dodaje bit parzystości na koniec wiadomości
        }
//        List<Integer> res = multiply(v,tmp);
//        System.out.println("rs" +res);
//        return res;
        return tmp;
    }

    //zakodowanie wiadomości, dodaje bity parzystości, zwraca vektor dwa razy dłuższy niż wektor podany jako parametr
    public List<Integer> Encode(List<Integer> text) {
        List<Integer> tmp =  new Vector<>();
        List<Integer> copy = new Vector<>();
        for(int i=0; i<text.size();i++) {
            tmp.add(text.get(i));
            copy.add(text.get(i));
            if( (i+1) % 8 == 0 && i != 0) {      //wstawienie po każdej literze (1 bajt) bitów parzystości
                tmp = AddParityBites(tmp, n);
//                System.out.println("tmp"+ tmp);
//                System.out.println("copy"+copy);
                for(int j=0;j<m;j++) copy.add(tmp.get(j+8));
                tmp.clear();
            }
        }
        return copy;
    }

    //funkcja usuwa bity parzystości z podanego wektora
    public List<Integer> Decode(List<Integer> text) {
        List<Integer> list = new Vector<>();
        int j = 0;
        System.out.println(text.size());
        while (j<text.size()){
            list.add(text.get(j));      //przepisanie do pomocniczej tablicy z pomienięciem bitów parzystości
            if((j+1)%8 == 0){             //pominięcie 8 bitów po 8 bitach
                j+=9;
            }
            else j+=1;
        }
        return  list;
    }

    //metoda znajdująca i poprawiająca jeden błąd dla jednej wiadomości
    private boolean FixOneErrors(List<Integer> HE, List<Integer> T){ //nie sprawdzałam czy działa
        List<Integer> column = new Vector<>();
        int s = 0;
        for(int i=0;i<m+n;i++){
            for(int j=0;j<n;j++) {
                column.add(H[j][i]);                    //wektor zawierający kolumny macierzy H
                if (column.get(j) == HE.get(j)) s += 1;         //zmienna pomocnicza zliczająca ilość takich samych elementach w kolumnach
            }

            if(s == 8){
                T.set(i,(T.get(i) == 1? 0:1));      //jeżeli tak to znaleziono błąd i następuje negacja bitu
                return true;
            }
            column.clear();
            s = 0;
        }
        return false;
    }

    //
    private boolean FixTwoErrors(List<Integer> T){



        return false;
    }

    public List<Integer> Correct(List<Integer> T) {
        int error = 0;                  //zmienna określająca czy został znaleziony błąd czy nie
        //System.out.println("T" + T);
        List<Integer> tmp =  new Vector<>();  //przechowuje 8 bitów wiadmosci i 8 bitów parzystości
        List<Integer> result = new Vector<>();

        for(int n=0;n<T.size()/16;n++) {            //wykonuje się dla 8 bitów wiadomośći i 8 bitów parzystość
            for(int i=0;i<16;i++) {
                tmp.add(T.get(i+(n*16)));
            }
            List<Integer> HT = multiply(tmp, new Vector<Integer>()); //pomnożenie wektora tmp i macierzy H
            //System.out.print("tmp"+tmp);
            for (int value : HT) {
                if (value != 0) error = 1;       //jeżeli macież H*T zawiera 1 to znaleziono błąd
            }
            if (error==1) {
                FixOneErrors(HT, tmp);
           }
            else if(error == 2){
                //FixTwoErrors(tmp);
            }
            for(int v : tmp) result.add(v);
            tmp.clear();        //czyszczenie wektorów
            HT.clear();
            error = 0;
        }
        //System.out.println("");
        return result;
    }

    public List<Integer> multiply(List<Integer> v, List<Integer> tmp){
        //List<Integer> tmp = new Vector<>(v.size());    //pomocnicza tablica przechowująca wynik operacji

        for (int i=0; i<n;i++) {
            int sum = 0;
            for (int j = 0; j <n+m; j++) {
                //System.out.println(v.get(j));
                sum += H[i][j] * v.get(j);            //wiersz H * kolumna v
            }
            sum %= 2;             //operacje bitowe są wykonywane modulo 2, aby otrzyma otrzymać tylko 0 lub 1
            tmp.add(sum);         //dodaje bit parzystości na koniec wiadomości
        }
        return tmp;
    }
}


