﻿using System;
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
using System.IO.Ports;
using System.IO;
using Microsoft.Win32;

namespace Xmodem
{
    /// <summary>
    /// Logika interakcji dla klasy Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        bool crc;
        
        private PortTransmitter tr;
        private byte[] buf;
        private string name;
        private int boundRate;
        Parity parity;
        StopBits stopBits;
        byte[] bytes;
       
        public Window1(bool crc16, string name, int bytes, Parity parity, StopBits stop)
        {

            this.crc = crc16;
            this.name = name;
            this.boundRate = bytes;
            this.parity = parity;
            this.stopBits = stop;
            
            
            InitializeComponent();
        }

        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            tr.close();
            this.Close();

        }

        private void Button_Click_Choose(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
                File.ReadAllText(openFileDialog.FileName);

            buf = File.ReadAllBytes(openFileDialog.FileName);
            tr = new PortTransmitter(name,boundRate, parity, stopBits, buf, crc);
        }

        private void Button_Click_Send(object sender, RoutedEventArgs e)
        {
            tr.sendFile();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {


        }
    }
}
