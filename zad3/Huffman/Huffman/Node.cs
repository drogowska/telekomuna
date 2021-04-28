using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huffman
{
    class Node
    {
        public string symbol { get; set; }
        public int frequency { get; set; }
        public Node right { get; set; }
        public Node left { get; set; }
        public Node parentNode;

        public Node(string symbol, int freq)
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
                symbol = n1.symbol + n2.symbol;                 //nazwa rodzica jest złączeniem nazw dzieci
            }
            else if (n1.frequency < n2.frequency)
            {
                right = n2;
                left = n1;
                right.parentNode = left.parentNode;             //mają teraz tego samego rodzica
                frequency = n1.frequency + n2.frequency;        //wartość prawdopodobieństwa rodzica jest sumą jego dzieci
                symbol = n1.symbol + n2.symbol;                 //nazwa rodzica jest złączeniem nazw dzieci
            }

        }
    }
}
