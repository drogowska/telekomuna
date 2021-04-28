using System;
using System.IO.Ports;
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
        private bool flagCrc;
        public MainWindow()
        {

            InitializeComponent();
            string[] ports = SerialPort.GetPortNames();
            for (int i = 0; i < ports.Length; i++) com.Items.Add(ports[i]);

        }

        private void RadioButton_Checksum(object sender, RoutedEventArgs e)
        {
            flagCrc = false;
        }

        private void RadioButton_CRC16(object sender, RoutedEventArgs e)
        {
            flagCrc = true;
        }

        private void Button_Click_Send(object sender, RoutedEventArgs e)
        {
            Parity p = getParity();
            StopBits s = getStopBit();
            string name = com.Text;
            WindowSender window = new WindowSender(flagCrc, name, Convert.ToInt32(speed.Text), p, s);
            window.Show();
            this.Hide();


        }

        private void Button_Click_Receive(object sender, RoutedEventArgs e)
        {
            Parity p = getParity();
            StopBits s = getStopBit();
            WindowReceiver window = new WindowReceiver(flagCrc, com.Text, Convert.ToInt32(speed.Text), p, s);
            window.Show();
            this.Close();

        }
        private StopBits getStopBit()
        {
            //StopBits p;
            ComboBoxItem typeItem = (ComboBoxItem)stopBit.SelectedItem;
            if (typeItem.Content.ToString() == "One")
            {
                return System.IO.Ports.StopBits.One;
            }
            else if (typeItem.Content.ToString() == "Two")
            {
                return System.IO.Ports.StopBits.Two;
            }
            else
            {
                return System.IO.Ports.StopBits.None;
            }
        }

        private Parity getParity()
        {
            Parity p;
            ComboBoxItem typeItem = (ComboBoxItem)parity.SelectedItem;
            switch (typeItem.Content.ToString())
            {
                case "Odd":
                    {
                        p = System.IO.Ports.Parity.Odd;
                        break;
                    }
                case "Even":
                    {
                        p = System.IO.Ports.Parity.Even;
                        break;
                    }
                default:
                    {
                        p = System.IO.Ports.Parity.None;
                        break;
                    }

            }
            return p;
        }

    }
}
