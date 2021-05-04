using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace Huffman
{
    class HuffmanTree
    {
        private List<Node> nodes = new List<Node>();
        public Node root;
        private Dictionary<char, int> freq = new Dictionary<char, int>();       //mapa litery i częstliowsci występowania


        public void create()
        {
            freq = asciiTree.initializeDictionary();                    //inicializacja drzewa
            foreach (KeyValuePair<char, int> pair in freq)
            {
                nodes.Add(new Node(pair.Key, pair.Value));           //stworzenie z par litera-prawdopodbieństwo węzłów i dodanie ich do listy
            }

            while (nodes.Count > 1)
            {
                Node node1 = nodes[0];              //pobranie pierwszego węzła z listy
                nodes.RemoveAt(0);                  //usunięcie pierwszego węzła z listy
                Node node2 = nodes[0];              //pobranie pierwszego węzła z listy
                nodes.RemoveAt(0);                  //usunięcie pierwszego węzła z listy
                nodes.Add(new Node(node1, node2));  //stworzenie nowego węzła i dodanie go do listy węzłów

                nodes.Sort();                       //posortowanie węzłów względem częstotliwości malejąco
            }
            root = nodes.FirstOrDefault();          //ustawienie pierwszego wezła jako root
        }

        //zakodowanie wiadomości do skomporesowanej postaci binarnej
        public byte[] encode(string text, Stream file)
        {
            List<bool> result = new List<bool>();           //stworzenie pustej listy przechowującej wynik funkcji
            for (int i = 0; i < text.Length; i++)
            {
                List<bool> encodedCharacter = this.root.traverseTree(text[i], new List<bool>());        //obliczenie dla każdego znaku jego reprezentacji bitowej na podstawie drzewa i zapisanie do zmiennej encodedCharacter
                foreach (bool b in encodedCharacter)  result.Add(b);                                    //dodanie elementów encodedCharacter do listy zwracanej przez funkcje
                encodedCharacter.Clear();
            }
            BitArray bits = new BitArray(result.ToArray());                                             //konwersja listy bool-i na BitArray
            int n = bits.Length / 8 + (bits.Length % 8 == 0 ? 0 : 1);                                   //ustalenie rozmiaru tablicy bajtów dzieląc ilość bitów przez 8 i dodając 1 gdy dzielenie daje reszte
            byte[] bytes = new byte[n];
    
            bits.CopyTo(bytes,0);                                                                       //przekopiowanie BitArray do tablicy bajtów
            file.Write(bytes, 0, bytes.Length);                                                         //zapisanie tablicy bajtów do pliku     
            file.Close();                                                                               //zamknięcie pliku
            return bytes;
        }
        //odczytanie ciągu bitów i zapisanie ich jako string
        public string decode(string file, List<bool> list)
        {
            Node current = this.root;                               //ustalenie wezła obecnego jako root wezeł
            string decoded = "";                                    //inicializacja wynikowego stringa
            foreach (bool bit in list)                              //pętla wykonuje się dla każdego bool-a z podanej jako prametr listy
            {
                if (bit)                                            
                {
                    if (current.right != null) current = current.right;             //jeżeli bit jest true (1) to sprawdzamy czy obecny węzeł ma wezła po prawej jeśli tak to oznaczomy go jako wezeł obecny
                }
                else
                {
                    if (current.left != null) current = current.left;               //jeśli bit jest false (0) to sprawdzamy czy wezeł ma wezła po lewej jeśli tak to zapisujemy go do zmiennej current
                }   

                if (current.IsLeaf())                           //sprawdzamy czy wezeł jest liściem, czyli nie ma wezłów po lewej i prawej
                {
                    decoded += current.symbol;                  //do zmiennej decoded zapisujemy znak z danego wezła 
                    current = this.root;                        //i ustalamy obecny wezeł na wezeł rodzica
                }
            }
            return decoded;
        }

        
    }

}
