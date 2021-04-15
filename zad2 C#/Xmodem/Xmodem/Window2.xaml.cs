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

namespace Xmodem
{
    /// <summary>
    /// Logika interakcji dla klasy Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        private Receiver tr;
        private byte[] buf;
        private string name;
        private int boundRate;
        Parity parity;
        StopBits stopBits;
        byte[] bytes;
        bool crc;

        public Window2(bool crc16, string name, int bytes, Parity parity, StopBits stop)
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
            if (openFileDialog.ShowDialog() == true)
            {
                File.ReadAllText(openFileDialog.FileName);
                //file = openFileDialog.OpenFile();

                buf = File.ReadAllBytes(openFileDialog.FileName);// ReadAllBytes(openFileDialog.FileName);
                tr = new Receiver(name, boundRate, parity, stopBits, buf, crc);
            }
        }

        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            tr.close();
            this.Close();
        }

        private void Button_Click_Save(object sender, RoutedEventArgs e)
        {
            tr.ReceiveBytes();

            //Receiver re = new Receiver(com);          
            //List<byte> bytes = re.receiveFile(crc);
            //foreach(byte b in bytes)
            //{
            //    file.WriteByte(b);
            //}
            //file.Close();
            
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
