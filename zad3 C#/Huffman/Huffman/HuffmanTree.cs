using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Collections;


namespace Huffman
{
    class HuffmanTree
    {
        private List<Node> nodes = new List<Node>();
        public Node root;
        private Dictionary<char, int> freq = new Dictionary<char, int>();       //mapa litery i częstliowsci występowania


        public void create(string text)
        {
            if (text.Length > 10000)
            {
                //this.freq = asciiTree.initializeDictionary();
            }
            /*
            int n = text.Length;
            for (int i = 0; i < n; i++)
            {
                if (!freq.ContainsKey(text[i]))         //sprawdzenie czy w dzewie jest już taki sam klucz
                {
                    freq.Add(text[i], 0);               //dodanie litery do drzewa z prwdopodobieństwem równym 0
                }
                freq[text[i]]++;                        //zwiększenie ilości występowania litery o 1
            }
            */

            foreach (KeyValuePair<char, int> pair in freq)
            {
                nodes.Add(new Node(pair.Key.ToString(), pair.Value));           //stworzenie z par litera-prawdopodbieństwo węzłów i dodanie ich do listy
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
            root = nodes.FirstOrDefault();
            List<bool> list = new List<bool>();
            setBitsInTree(root, "");

        }


        public void setBitsInTree(Node node, string v)
        {
            if (node == null) return;
            if (node.left == null && node.right == null)
            {
                node.code = v;
                return;
            }
            if (node.left != null)
            {
                //v.Add(false);              
                setBitsInTree(node.left, v+"0");
            } if (node.right != null) 
            {
                //v.Add(true);
                setBitsInTree(node.right, v+"1");
            }
        }

        public List<bool> getBitsFromTree()
        {
            List<bool> result = new List<bool>();

            return result;
        }

        public void encode(string text, Stream file)
        {
            //HuffmanTree tree = new HuffmanTree();
            this.create(text);
            for (int i = 0; i < text.Length; i++)
            {
                //file.Write(nodes.ElementAt(i).code);


            }

        }

        public void decode(Stream file, BitArray list)
        {
            Node current = this.root;
            string decoded = "";
            foreach (bool bit in list)
            {
                if(bit)
                {
                    if (current.right != null)
                    {
                        current = current.right;
                    }
                }
                else
                {
                    if (current.left != null)
                    {
                        current = current.left;
                    }
                }

                if (IsLeaf(current))
                {
                    decoded += current.symbol;
                    current = this.root;
                }


            }

        }

        public bool IsLeaf(Node node)
        {
            return (node.left == null && node.right == null);
        }
    }

}
