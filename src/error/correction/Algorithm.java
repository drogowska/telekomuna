package error.correction;

import java.util.List;
import java.util.Vector;

public class Algorithm {

    //Dmin = 4 dla korekcji 1 błędu ale detekcji 2, a Dmin= 5 dla korekcji 2 błędów

    private static int[][] H = {    //Macierz H (parzystości) ma nie posiadać kolumny zerowej,
            //identycznych kolumn oraz
            //żadna z kolumn nie może być sumą dwóch innych

            //g(x)=100101011

            {1, 1, 1, 1, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 1, 1, 1, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0},
            {1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0},
            {0, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0},
            {0, 0, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 0},
            {1, 1, 1, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1}
    };

    private int m = 8; //zmienna określająca długość wiadomości
    private int n = 8; //zmienna określająca długośc bitów parzystości


    //konwersja znaku ASCI do kodu binarnego zapisanego jako string
    private String AsciToBinaryString(char text) {
        String tmp = "";
        StringBuilder stringBuilder = new StringBuilder();
        tmp += String.valueOf(Integer.valueOf(Integer.toBinaryString(text)));  //zamiana znaku na postać binarną
        stringBuilder.append(tmp);
        while (stringBuilder.length() % 8 != 0) {               //uzupełnienie 0 na początku do 8 bitów wiadomości
            stringBuilder.insert(0, '0');
        }
        return stringBuilder.toString().trim();             //usuwa znak nowej lini
    }

    //konwersja wyrazu (ciągu znaków ASCI) do stringa w postaci binarnej
    public String convertCharsToBinaryString(String s) {
        String s1 = "";
        for (int i = 0; i < s.length(); i++) {
            s1 += AsciToBinaryString(s.charAt(i));
        }
        return s1;
    }

    //konwersja bitów zapisanych w stringu do vektora bitów reprezentowanych przez inty
    public List<Integer> BinaryStringToBinary(String text) {
        List<Integer> v = new Vector<>();
        for (int i = 0; i < text.length(); i++) {
            v.add(text.charAt(i) == '1' ? 1 : 0);
        }
        return v;
    }

    //konwersja wektora zer i jedenek do stringa zawierającego 0 i 1
    public String BinaryToBinaryString(List<Integer> bin) {
        String string = "";
        for (int i = 0; i < bin.size(); i++) {
            string += (bin.get(i) == 1 ? '1' : '0');
        }
        return string;
    }

    //konwersja wektora zer i jedynek na znaki kodu asci
    public char[] BinaryToAsci(List<Integer> bin) {
        char[] tab = new char[bin.size() / 8];        //tablica przechowująca odkodwane znaki
        char znak;
        for (int j = 0; j < bin.size() / 8; j++) {
            znak = 0;
            for (int i = 0; i < 8; i++) {
                znak += bin.get(7 + (j * 8) - i) * Math.pow(2, i); // zamiana zer i jedynek na liczbę w zapisie dzisiętnym, która jest
                // interpretowana jako znak z tablicy ASCI
            }
            tab[j] = znak;
        }
        return tab;
    }


    // funkja wyknywująca mnożenie macierzy H przez wektor oraz dodająca bity parzystości
    private List<Integer> AddParityBites(List<Integer> v) {
        List<Integer> tmp = new Vector<>(v.size());    //pomocnicza tablica przechowująca wynik operacji
        for (int i = 0; i < v.size(); i++) {
            tmp.add(v.get(i));
        }
        return multiply(v, tmp, n);
    }

    public List<Integer> multiply(List<Integer> v, List<Integer> tmp, int size) {
        for (int i = 0; i < n; i++) {
            int sum = 0;
            for (int j = 0; j < size; j++) {
                sum += H[i][j] * v.get(j);            //wiersz H * kolumna v
            }
            sum %= 2;             //operacje bitowe są wykonywane modulo 2, aby otrzyma otrzymać tylko 0 lub 1
            tmp.add(sum);         //dodaje bit na koniec wiadomości
        }
        return tmp;
    }

    //zakodowanie wiadomości, dodaje bity parzystości, zwraca vektor dwa razy dłuższy niż wektor podany jako parametr
    public List<Integer> Encode(List<Integer> text) {
        List<Integer> tmp = new Vector<>();
        List<Integer> copy = new Vector<>();
        for (int i = 0; i < text.size(); i++) {
            tmp.add(text.get(i));
            copy.add(text.get(i));
            if ((i + 1) % 8 == 0 && i != 0) {      //wstawienie po każdej literze (1 bajt) bitów parzystości
                tmp = AddParityBites(tmp);
                for (int j = 0; j < m; j++) copy.add(tmp.get(j + 8));
                tmp.clear();
            }
        }
        return copy;
    }

    //funkcja usuwa bity parzystości z podanego wektora
    public List<Integer> Decode(List<Integer> text) {
        List<Integer> list = new Vector<>();
        int j = 0;
        while (j < text.size()) {
            list.add(text.get(j));         //przepisanie do pomocniczej tablicy z pomienięciem bitów parzystości
            if ((j + 1) % 8 == 0) {              //pominięcie 8 bitów po 8 bitach
                j += 9;
            } else j += 1;
        }
        return list;
    }

    //metoda znajdująca i poprawiająca jeden błąd dla jednej wiadomości
    private boolean FixOneError(List<Integer> HE, List<Integer> T) {
        List<Integer> column = new Vector<>();
        int s = 0;
        for (int i = 0; i < m + n; i++) {
            for (int j = 0; j < n; j++) {
                column.add(H[j][i]);                    //wektor zawierający kolumny macierzy H
                if (column.get(j) == HE.get(j))
                    s += 1; //zmienna pomocnicza zliczająca ilość takich samych elementach w kolumnach HE i H
            }

            if (s == 8) {
                T.set(i, (T.get(i) == 1 ? 0 : 1));      //jeżeli tak to znaleziono błąd i następuje negacja bitu
                return true;
            }
            column.clear();     //czyszczenie zmiennych pomocniczych
            s = 0;
        }
        return false;
    }

    //funkcja znajdująca miejsce dwóch błędów i poprawiająca je
    private boolean FixTwoErrors(List<Integer> HT, List<Integer> T) {
        List<Integer> sum = new Vector<>();
        boolean tmp;
        for (int i = 0; i < m + n; i++) {
            for (int j = i + 1; j < n + m; j++) {
                tmp = false;
                for (int k = 0; k < n; k++) {             //obliczenie sumy elementów dwóch kolejnych kolumn i przepisanie wyniku do wektora sum
                    sum.add((H[k][i] + H[k][j]) % 2);   //operacja %2 aby otrzymać 0 lub 1

                    if (HT.get(k) == sum.get(k))
                        tmp = true;//sprawdzenie czy suma dwóch kolumn jest równa kolumnie z macierzy HT
                }
                    if (tmp) {
                        T.set(i, (T.get(i) == 1 ? 0 : 1));      //jeżeli tak to znaleziono błędy i następuje negacja bitu
                        T.set(j, (T.get(j) == 1 ? 0 : 1));
                        return true;
                    }
            }
        }

        return false;
    }

    //funkcja sprawdzająca czy wiadomość posiada błędy, jeśli tak to znajduje je i poprawia
    public List<Integer> Correct(List<Integer> T) {
        int error = 0;                          //zmienna określająca czy został znaleziony błąd
        List<Integer> tmp = new Vector<>();
        List<Integer> result = new Vector<>();

        for (int n = 0; n < T.size() / 16; n++) {
            for (int i = 0; i < 16; i++) {
                tmp.add(T.get(i + (n * 16)));       //przepisanie 8 bitów wiadmosci i 8 bitów parzystości
            }
            List<Integer> HT = multiply(tmp, new Vector<>(), this.n + m); //pomnożenie wektora tmp i macierzy H
            for (int value : HT) {
                if (value != 0) error = 1;       //jeżeli macież H*T zawiera 1 to znaleziono błąd
            }

            if (error != 0) {
                if (!FixTwoErrors(HT, tmp)) {        //poszukiwanie i poprawa najpierw dwóch błędów a jeśli nie znaleziono to jednego błędu
                    FixOneError(HT, tmp);
                }
            }
            for (int v : tmp) result.add(v);         //przepisanie wyników pojedyńczej iteracji do wektora
            tmp.clear();        //czyszczenie zmiennych pomocniczych
            HT.clear();
            error = 0;
        }
        return result;
    }
}