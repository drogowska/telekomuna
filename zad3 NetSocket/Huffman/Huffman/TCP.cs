using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Huffman
{
    class TCP
    {
        public static void send(IPAddress ipv4, int port, string fileName)
        {
            IPEndPoint ipEndPoint = new IPEndPoint(ipv4, port);                                          //ustalenie punktu końcowego dla gniazda
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //utworzenie gniazda nastawionego na przesył danych protokołem TCP

            client.Connect(ipEndPoint);                                                                  //ustanowenie połączenia z punktem końcowym
            client.SendFile(fileName);                                                                   //wysłanie pliku o podanej ścieżce

            client.Shutdown(SocketShutdown.Both);                                                        //zamknięcie połączenie gniazd
            client.Close();
        }

        public static void receive(IPAddress ipv4, int port,string fileName)
        {
            TcpListener tcpListener = new TcpListener(ipv4, port);                                       //ustawienie TCPListener na porcie
            tcpListener.Start();                                                                         //zacznij nasłuchiwanie na porcie
            byte[] buffer = new byte[1024];                                                              //bufor dla odczytywanych danych
            using (FileStream fs = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write))         //otworzenie pliku do zapisu
            {
                if (fs == null) return;
                using (TcpClient client = tcpListener.AcceptTcpClient())                                 //akceptowanie oczekujących żądań połączenia
                {
                    NetworkStream stream = client.GetStream();                                           //uzyskanie obiekty strumienia do zapisu danych
                    byte[] bytes = new byte[1024];
                    int i = stream.Read(bytes, 0, bytes.Length);
                    while (i != 0)                                                                       //warunek spełniony, dopóki strumień nie jest pusty
                    {
                        fs.Write(bytes, 0, i);                                                           //zapis do pliku
                        i = stream.Read(bytes, 0, bytes.Length);
                    }
                    client.Close();                                                                      //zakończenie i zamknięcie połączenia
                }
            }
        }
    }
}
