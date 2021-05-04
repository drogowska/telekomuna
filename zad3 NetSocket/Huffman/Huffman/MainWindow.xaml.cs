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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Huffman
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_Sender(object sender, RoutedEventArgs e)
        {
            
            WindowSender window = new WindowSender();
            window.Show();
            this.Close();


        }

        private void Button_Click_Receiver(object sender, RoutedEventArgs e)
        {
            
            WindowReceiver window = new WindowReceiver();
            window.Show();
            this.Close();

        }
    }
}
