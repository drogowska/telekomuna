using System;
using System.Collections.Generic;

namespace Huffman
{
    class Node : IComparable<Node>
    {
        public char symbol { get; set; }
        public int frequency { get; set; }              //prawodpodobieństwo wystąpienia w tekscie danego znaku
        public Node right { get; set; }                 //przchowuje obiekt wezła po prawej stronie
        public Node left { get; set; }                  //przchowuje obiekt wezła po lewej stronie
        public Node parentNode;                         //przchowuje obiekt węzła rodzica
        //public string code;                             
        public Node(char symbol, int freq)
        {
            this.frequency = freq;                  //stworzenie wezła o podanym znaku i częstotliwości
            this.symbol = symbol;
            right = left = parentNode = null;           //ustalenie wezłów po prawej, lewej stronie oraz węzła rodzica jako null
        }

        public Node(Node n1, Node n2)
        {
            if (n1.frequency >= n2.frequency)                   //jeżeli prawdopodobieństwo wystąpienia znaku pierwszego wezła jest większe od prawdopodobieństwa drugiego wezła
            {
                right = n1;                                     //ustalenie wezła n1 jako wezła po prawej, a węzła n2 jako węzła po lewej stronie
                left = n2;

                right.parentNode = left.parentNode;             //mają teraz tego samego rodzica
                frequency = n1.frequency + n2.frequency;        //wartość prawdopodobieństwa rodzica jest sumą jego dzieci
            }
            else if (n1.frequency < n2.frequency)
            {
                right = n2;
                left = n1;                                      //ustalenie wezła n2 jako wezła po prawej, a węzła n1 jako węzła po lewej stronie
                right.parentNode = left.parentNode;             //mają teraz tego samego rodzica
                frequency = n1.frequency + n2.frequency;        //wartość prawdopodobieństwa rodzica jest sumą jego dzieci
            }

        }
        public bool IsLeaf()                                    //funkcja sprawdzająca czy dany węzeł jest liściem czyli czy nie posiada on węzłów po lewewj i prawej stronie
        {
            return (left == null && right == null);
        }
        public int CompareTo(Node node)                         //funkcja porównująca dwa węzły względem wielkości liczby określającej prawdopodobieństwo 
        {
            return this.frequency.CompareTo(node.frequency);
        }

        //Funkcja znajduje podany znak w drzewnie i wylicza dla niego postać binarną
        public List<bool> traverseTree(char symbol, List<bool> data)
        {
            if (this.IsLeaf())                                          //sprawdzamy czy wezeł jest liściem
            {
                if (symbol.Equals(this.symbol)) return data;            //jeśli znak liści jest taki sam jak podany symbol to zwracamy liste bool-i
                else return null;                                       //jeśli nie funkcja zwraca null
            }
            else                                                        //dla węzłów nie będącymi liściami 
            {
                List<bool> leftList = null;                             //tymczasowej tablice przechowujące bity z prawego i lewego poddrzewa
                List<bool> rightList = null;

                if (left != null)                                      
                {
                    List<bool> leftPath = new List<bool>();             //jeśli węzeł ma wezły po lewej 
                    leftPath.AddRange(data);                            //to dodaje do listy leftPath tablice data
                    leftPath.Add(false);                                //i dodaje wartość false (0)

                    leftList = left.traverseTree(symbol, leftPath);     //następnie wykonuje się funkcja rekurencyjnie idąc od liści do roota dla lewego poddrzewa
                }

                if (right != null)                                      
                {
                    List<bool> rightPath = new List<bool>();            //jeśli wezeł ma węzły po prawej 
                    rightPath.AddRange(data);                           //to dodaje do listy rightPath na koniec tablie data
                    rightPath.Add(true);                                //i dodaje wartość true (1)
                    rightList = right.traverseTree(symbol, rightPath);  //następnie wykonuje się funkcja rekurencyjnie idąc od liści do roota dla prawego poddrzewa
                }

                if (leftList != null) return leftList;                  //jeśli wezeł ma liści po lewej lub po prawej to funkcja zwraca odpowiednie tablice
                else return rightList;
            }
        }
    }

}
