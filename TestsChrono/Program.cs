//using Network;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading.Tasks;

//namespace TestsChrono
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            ConnectionResult connectionResult = ConnectionResult.Timeout;


//            TcpConnection tcpConnection = ConnectionFactory.CreateTcpConnection("192.168.0.154", 2008, out connectionResult);

//            //1. Create a UDP connection. A UDP connection requires an alive TCP connection.
//            UdpConnection udpConnection = ConnectionFactory.CreateUdpConnection(tcpConnection, out connectionResult);
//            if (connectionResult != ConnectionResult.Connected) return;

//            udpConnection.ConnectionEstablished += UdpConnection_ConnectionEstablished;


//            Console.ReadLine();


//            int i = 0;
//            var client = new UdpClient();
//            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2008); // endpoint where server is listening
//            client.Connect(ep);

//            Console.WriteLine("receive data to ");
//            while (true)
//            {


//                System.Threading.Thread.Sleep(5000);
//                // send data
//                string texte = string.Format("Message a transmettre = {0}", ++i);
//                byte[] b = System.Text.Encoding.UTF8.GetBytes(texte);
//                client.Send(b, b.Length);
//                Console.WriteLine("Un message est transmis ");
//            }
//            // then receive data
//            // var receivedData = client.Receive(ref ep);

//            //Console.Write("receive data sent ");

//            //Console.Write("receive data from " + ep.ToString());

//            Console.Read();


//            // UDPListener.Main1();
//        }

//        private static void UdpConnection_ConnectionEstablished(TcpConnection arg1, UdpConnection arg2)
//        {
//            if (arg2 != null)
//            //{
//            //    Console.WriteLine($"{arg2.ToString()} Connection established");

//            //    arg2.
//            //    //3. Register what happens if we receive a packet of type "CalculationResponse"
//            //    container.RegisterPacketHandler<CalculationResponse>(calculationResponseReceived, this);
//            //    //4. Send a calculation request.
//            //    connection.Send(new CalculationRequest(10, 10), this);

//            }
//        }
//    }
//}
