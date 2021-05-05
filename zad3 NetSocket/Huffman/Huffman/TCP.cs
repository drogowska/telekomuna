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
            TcpListener tcpListener = new TcpListener(ipv4, port);
            tcpListener.Start();
            byte[] buffer = new byte[1024];
            using (FileStream fs = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                if (fs == null) return;
                using (TcpClient client = tcpListener.AcceptTcpClient())
                {
                    NetworkStream stream = client.GetStream();
                    byte[] bytes = new byte[1024];
                    int i = stream.Read(bytes, 0, bytes.Length);
                    while (i != 0)
                    {
                        fs.Write(bytes, 0, i);
                        i = stream.Read(bytes, 0, bytes.Length);
                    }
                    client.Close();
                }
            }
        }
    }
}
