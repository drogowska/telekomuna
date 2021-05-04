﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

namespace Huffman
{
    /// <summary>
    /// Logika interakcji dla klasy Window1.xaml
    /// </summary>
    public partial class WindowReceiver : Window
    {
        private byte[] buf;
        string fileN;
        Stream file;
        string text;
        string newf;
        public WindowReceiver()
        {
            InitializeComponent();
        }

        private void Button_Click_Choose(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                File.ReadAllText(openFileDialog.FileName);
                this.fileN = openFileDialog.FileName;
            }
            fileName.Text = openFileDialog.FileName;
            text = File.ReadAllText(openFileDialog.FileName);
            file = openFileDialog.OpenFile();
        }

        private void Button_Click_Listen(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            if (saveFileDialog.ShowDialog() == true)
            {
                File.ReadAllText(saveFileDialog.FileName);
                this.fileN = saveFileDialog.FileName;
            }
            fileName.Text = saveFileDialog.FileName;
            text = File.ReadAllText(saveFileDialog.FileName);
            buf = File.ReadAllBytes(saveFileDialog.FileName);
            file = saveFileDialog.OpenFile();
            TCP tcp = new TCP();
            TCP.receive(IPAddress.Parse(ipAddress.Text), Convert.ToInt32(portNumber.Text), saveFileDialog.FileName);
        }

        private void Button_Click_Decompress(object sender, RoutedEventArgs e)
        {
            HuffmanTree tree = new HuffmanTree();
            tree.create();

            SaveFileDialog openFileDialog = new SaveFileDialog();
            Stream s;
            if (openFileDialog.ShowDialog() == true)
            {
                FileStream files = File.Create(openFileDialog.FileName);
                newf = openFileDialog.FileName;
                files.Close();

            }
            s = openFileDialog.OpenFile();

            byte[] bytes = tree.encode(text, s);


        }
    }
}
