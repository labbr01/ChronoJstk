using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

class Program
{
    static void Main(string[] args)
    {
        // Create UDP client
        int receiverPort = 2243; // 8127;

        //UdpClient receiver = new UdpClient("192.168.0.154", receiverPort);
        //UdpClient receiver = new UdpClient("255.255.255.255", receiverPort);        
        UdpClient receiver = new UdpClient(2243);
        //IPAddress pa = IPAddress.Parse("255.255.255.255");
        //IPEndPoint ipe = new IPEndPoint(pa, 2008);
        //receiver = new UdpClient(ipe);

        // Display some information
        Console.WriteLine("Starting Upd receiving on port: " + receiverPort);
        Console.WriteLine("Press any key to quit.");
        Console.WriteLine("-------------------------------\n");

        // Start async receiving
        receiver.BeginReceive(DataReceived, receiver);

        //// Send some test messages
        //using (UdpClient sender1 = new UdpClient(19999))
        //    sender1.Send(Encoding.ASCII.GetBytes("Hi!"), 3, "localhost", receiverPort);
        //using (UdpClient sender2 = new UdpClient(20001))
        //    sender2.Send(Encoding.ASCII.GetBytes("Hi!"), 3, "192.168.0.1", receiverPort);
        //using (UdpClient sender3 = new UdpClient(20002))
        //    sender3.Send(Encoding.ASCII.GetBytes("Hi plus!"), 8, "PC-ARPVQ2", receiverPort);
        //using (UdpClient sender3 = new UdpClient(20003))
        //    sender3.Send(Encoding.ASCII.GetBytes("Hi plus plus!"), 13, "192.168.0.154", receiverPort);
        // Wait for any key to terminate application
        Console.ReadKey();
    }

    private static void DataReceived(IAsyncResult ar)
    {
        UdpClient c = (UdpClient)ar.AsyncState;
        IPEndPoint receivedIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        Byte[] receivedBytes = c.EndReceive(ar, ref receivedIpEndPoint);

        // Convert data to ASCII and print in console
        string receivedText = ASCIIEncoding.ASCII.GetString(receivedBytes);
        Console.Write(receivedIpEndPoint + ": " + receivedText + Environment.NewLine);

        // Restart listening for udp data packages
        c.BeginReceive(DataReceived, ar.AsyncState);
    }
}