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

namespace Xmodem
{
    /// <summary>
    /// Logika interakcji dla klasy Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        private bool crc;
        private PortConnection com;
        public Window2(bool crc16, PortConnection port)
        {
            crc = crc16;
            com = port;
            InitializeComponent();
        }

        private void Button_Click_Choose(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                File.ReadAllText(openFileDialog.FileName);
        }

        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            com.close();
            this.Close();
        }

        private void Button_Click_Recive(object sender, RoutedEventArgs e)
        {

        }
    }
}
