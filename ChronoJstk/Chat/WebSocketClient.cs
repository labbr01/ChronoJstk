//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Linq;
//using System.Net.WebSockets;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace ChronoJstk.Chat
//{
//    class WebSocketClient
//    {
//        static ClientWebSocket ws = null;

//        public WebSocketClient()
//        {
//            string url = ConfigurationManager.AppSettings["DiffusionWeb"];
//            Uri serverUri = new Uri(url);
//        }

//        public static async Task Init()
//        {
//            string url = ConfigurationManager.AppSettings["DiffusionWeb"];
//            Uri serverUri = new Uri(url);
            
//            ws = new ClientWebSocket();
//            await ws.ConnectAsync(serverUri, CancellationToken.None);
//        }


//        public static async Task Talk(string message)
//        {
//                string msg = message;
//                if (msg == "1")
//                {
//                    msg = "{ \"Casque\":1, \"Nom\": \"Bruno Labrecque1\", \"Club\": \"Québec\", \"Tour\": 3  , \"Temps\" : \"Bruno Labrecque\" }";
//                }
//                if (msg == "2")
//                {
//                    msg = "{ \"Casque\":2, \"Nom\": \"Bruno Labrecque2\", \"Club\": \"Québec\", \"Tour\": 3  , \"Temps\" : \"Bruno Labrecque\" }";
//                }
//            if (msg == "3")
//            {
//                msg = "{ \"Casque\":3, \"Nom\": \"Bruno Labrecque3\", \"Club\": \"Québec\", \"Tour\": 3  , \"Temps\" : \"Bruno Labrecque\" }";
//            }
//            if (msg == "4")
//            {
//                msg = "{ \"Casque\":4, \"Nom\": \"Bruno Labrecque4\", \"Club\": \"Québec\", \"Tour\": 3  , \"Temps\" : \"Bruno Labrecque\" }";
//            }
//            if (msg == "5")
//            {
//                msg = "{ \"Casque\":5, \"Nom\": \"Bruno Labrecque5\", \"Club\": \"Québec\", \"Tour\": 3  , \"Temps\" : \"Bruno Labrecque\" }";
//            }
//            ArraySegment<byte> bytesToSend = new ArraySegment<byte>(
//                        Encoding.UTF8.GetBytes(msg));
//                await ws.SendAsync(
//                    bytesToSend, WebSocketMessageType.Text,
//                    true, CancellationToken.None);
//                ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
//                WebSocketReceiveResult result = await ws.ReceiveAsync(
//                    bytesReceived, CancellationToken.None);
//                Console.WriteLine(Encoding.UTF8.GetString(
//                    bytesReceived.Array, 0, result.Count));            
//        }
//    }
//}
