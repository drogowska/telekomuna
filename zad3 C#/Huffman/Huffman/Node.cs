using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huffman
{
    class Node
    {
        public char symbol { get; set; }
        public int frequency { get; set; }
        public Node right { get; set; }
        public Node left { get; set; }
        public Node parentNode;
        //public List<bool> code;
        public string code;
        public Node(char symbol, int freq)
        {
            this.frequency = freq;
            this.symbol = symbol;
            right = left = parentNode = null;
        }

        public Node(Node n1, Node n2)
        {
            if (n1.frequency >= n2.frequency)
            {
                right = n1;                                     //
                left = n2;

                right.parentNode = left.parentNode;             //mają teraz tego samego rodzica
                frequency = n1.frequency + n2.frequency;        //wartość prawdopodobieństwa rodzica jest sumą jego dzieci
                //symbol = n1.symbol + n2.symbol;                 //nazwa rodzica jest złączeniem nazw dzieci
            }
            else if (n1.frequency < n2.frequency)
            {
                right = n2;
                left = n1;
                right.parentNode = left.parentNode;             //mają teraz tego samego rodzica
                frequency = n1.frequency + n2.frequency;        //wartość prawdopodobieństwa rodzica jest sumą jego dzieci
                //symbol = n1.symbol + n2.symbol;                 //nazwa rodzica jest złączeniem nazw dzieci
            }

        }
        public int CompareTo(Node node) 
        {
            return this.frequency.CompareTo(node.frequency);
        }

        //Funkcja znajduje podany znak w drzewnie i wylicza dla niego postać binarną
        public List<bool> traverseTree(char symbol, List<bool> data)
        {
            if (right == null && left == null)
            {
                if (symbol.Equals(this.symbol)) return data;
                else return null;
            }
            else
            {
                List<bool> leftList = null;
                List<bool> rightList = null;

                if (left != null)
                {
                    List<bool> leftPath = new List<bool>();
                    leftPath.AddRange(data);
                    leftPath.Add(false);

                    leftList = left.traverseTree(symbol, leftPath);
                }

                if (right != null)
                {
                    List<bool> rightPath = new List<bool>();
                    rightPath.AddRange(data);
                    rightPath.Add(true);
                    rightList = right.traverseTree(symbol, rightPath);
                }

                if (leftList != null) return leftList;
                else return rightList;
            }
        }
    }

}
