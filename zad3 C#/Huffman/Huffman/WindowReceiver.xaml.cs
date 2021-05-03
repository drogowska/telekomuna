using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.IO.Ports;

namespace Huffman
{
    /// <summary>
    /// Logika interakcji dla klasy Window2.xaml
    /// </summary>
    public partial class WindowReceiver : Window
    {
        private Receiver tr;
        //private byte[] buf;
        private string name;
        private int boundRate;
        Parity parity;
        StopBits stopBits;
        //byte[] bytes;
        bool crc;
        Stream file;
        string fileN;
        string text;
        byte[] buf;
        public WindowReceiver(bool crc16, string name, int bytes, Parity parity, StopBits stop)
        {
            crc = crc16;
            this.name = name;
            this.boundRate = bytes;
            this.parity = parity;
            this.stopBits = stop;
            InitializeComponent();
        }

        private void Button_Click_Choose(object sender, RoutedEventArgs e)
        {
            SaveFileDialog openFileDialog = new SaveFileDialog();
            long size;
            if (openFileDialog.ShowDialog() == true)
            {
                File.ReadAllText(openFileDialog.FileName);
                size = new FileInfo(openFileDialog.FileName).Length;
                //fileSize.Text = size.ToString();
                this.fileN = openFileDialog.FileName;

            }
            file = openFileDialog.OpenFile();
            //text = File.ReadAllText(openFileDialog.FileName);
            //buf = File.ReadAllBytes(openFileDialog.FileName);
            tr = new Receiver(name, boundRate, parity, stopBits, crc, file);
            fileName.Text = openFileDialog.FileName;

        }

        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            tr.close();
            this.Close();
        }

        private void Button_Click_Save(object sender, RoutedEventArgs e)
        {
            //tr.receiveFile();
            //fileSize.Text = new FileInfo(fileN).Length.ToString();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_Decompress(object sender, RoutedEventArgs e)
        {
            
            SaveFileDialog openFileDialog = new SaveFileDialog();
            Stream s;
            string newf;
            //Wybranie nazwy i stworzenie pustego pliku
            if (openFileDialog.ShowDialog() == true)
            {
                FileStream files = File.Create(openFileDialog.FileName);
                //files.Close();
            }
            newf = openFileDialog.FileName;
            OpenFileDialog O = new OpenFileDialog();
            //wybranie pliku do dekompresii
            if (O.ShowDialog() == true)
            {
                File.ReadAllText(O.FileName);

            }
            buf = File.ReadAllBytes(O.FileName);
            text = File.ReadAllText(O.FileName);

            //s = openFileDialog.OpenFile();
            HuffmanTree tree = new HuffmanTree();
            tree.create();
            List<bool> bools =  buf.SelectMany(GetBitsStartingFromLSB).ToList();
            string t = tree.decode(newf, bools);
            File.WriteAllText(newf, t);
            //tree.decode(fileN,);
        }

        static IEnumerable<bool> GetBitsStartingFromLSB(byte b)
        {
            for (int i = 0; i < 8; i++)
            {
                yield return (b % 2 == 0) ? false : true;
                b = (byte)(b >> 1);
            }
        }
    }
}
