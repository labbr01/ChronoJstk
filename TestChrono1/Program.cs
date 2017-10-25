using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TestChrono1
{
    class Program
    {


        static void Main(string[] args)
        {
            Form1 f = new Form1();
            f.ShowDialog();

            //UdpClient udpServer = new UdpClient(2008);
            
            //while (true)
            //{
            //    var remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2008);                
            //    var data = udpServer.Receive(ref remoteEP); // listen on port 11000
            //    string message = System.Text.Encoding.UTF8.GetString(data);
            //    Console.WriteLine("receive data from " + remoteEP.ToString() + " - contient : " + message);
            //    //udpServer.Send(new byte[] { 1 }, 1, remoteEP); // reply back
            //}
        }
    }
}
