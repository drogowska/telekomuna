﻿using System.Collections;
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
            freq = asciiTree.initializeDictionary();
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
            root = nodes.FirstOrDefault();
            //List<bool> list = new List<bool>();
            //setBitsInTree(root, "");

        }


        //public void setBitsInTree(Node node, string v)
        //{
        //    if (node == null) return;
        //    if (node.left == null && node.right == null)
        //    {
        //        node.code = v;
        //        return;
        //    }
        //    if (node.left != null)
        //    {
        //        //v.Add(false);              
        //        setBitsInTree(node.left, v+"0");
        //    } if (node.right != null) 
        //    {
        //        //v.Add(true);
        //        setBitsInTree(node.right, v+"1");
        //    }
        //}

        public List<bool> getBitsFromTree()
        {
            List<bool> result = new List<bool>();

            return result;
        }
        //zakodowanie wiadomości do skomporesowanej postaci binarnej
        public byte[] encode(string text, Stream file)
        {
            List<bool> result = new List<bool>();
            for (int i = 0; i < text.Length; i++)
            {
                List<bool> encodedCharacter = this.root.traverseTree(text[i], new List<bool>());
                foreach(bool b in encodedCharacter)  result.Add(b);
                encodedCharacter.Clear();
                //result.AddRange(encodedCharacter);
            }
            BitArray bits = new BitArray(result.ToArray());

            //string r = result.ToString();
            int n = bits.Length / 8 + (bits.Length % 8 == 0 ? 0 : 1);
            byte[] bytes = new byte[n];
            //for(int j = 0; j < n; j++)
            //{
            //    byte tmp = 0;
            //    for (int i = 0; i < 8; i ++)
            //    {
            //        if( j == n-1 )          //ostatni bajt danych
            //        {
            //            if(j%8 != 0)        //ilość bitów nie jest wielokrotnością 8
            //            {
                           
            //            }
            //        } else
            //        {
            //        int x = (bits[7 + (j * 8) - i] == true ? 1 : 0);
            //        tmp += (byte) (x * Math.Pow(2, i));
            //        }
                    
            //        //tmp += bits[i];
            //    }
            //    bytes[j] = tmp;
            //}
            
            
            //
            //byte[] bytes = new byte[bits.Length / 8 + (bits.Length % 8 == 0 ? 0 : 1)];
            bits.CopyTo(bytes,0);
            //Console.WriteLine(bytes.ToString());
            ////foreach (byte b in bytes) file.WriteByte(b);
            //File.WriteAllBytes(file, bytes);
            file.Write(bytes, 0, bytes.Length);
            file.Close();
            return bytes;
        }
        //odczytanie ciągu bitów i zapisanie ich jako string
        public string decode(string file, List<bool> list)
        {
            Node current = this.root;
            string decoded = "";
            foreach (bool bit in list)
            {
                if (bit)
                {
                    if (current.right != null) current = current.right;
                }
                else
                {
                    if (current.left != null) current = current.left;
                }

                if (current.IsLeaf())
                {
                    decoded += current.symbol;
                    current = this.root;
                }
            }
            File.WriteAllText(file, decoded);
            
            //file.Write(decode);
            return decoded;
        }

        
    }

}
