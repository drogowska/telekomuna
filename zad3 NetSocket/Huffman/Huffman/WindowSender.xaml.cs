using Microsoft.Win32;
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
    public partial class WindowSender : Window
    {
        string fileN;
        Stream file;
        string text;
        string newf;
        public WindowSender()
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

        private void Button_Click_Send(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                File.ReadAllText(openFileDialog.FileName);
                this.fileN = openFileDialog.FileName;
            }
            fileName.Text = openFileDialog.FileName;
            TCP tcp = new TCP();
            TCP.send(IPAddress.Parse(ipAddress.Text), Convert.ToInt32(portNumber.Text), openFileDialog.FileName);
        }

        private void Button_Click_Compress(object sender, RoutedEventArgs e)
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
