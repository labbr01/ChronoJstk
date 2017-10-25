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
//    public static class ChatWithServer2
//    {       
//            private static async Task ChatWithServer(AutoResetEvent are)
//            {
//                using (ClientWebSocket ws = new ClientWebSocket())
//                {
//                string url = ConfigurationManager.AppSettings["DiffusionWeb"];
//                Uri serverUri = new Uri(url);
//                //Uri serverUri = new Uri("ws://localhost:61195/WSHandler.ashx");
//                    await ws.ConnectAsync(serverUri, CancellationToken.None);
//                //ws.
//                //are.WaitOne()                
//                //are.Reset();
//                bool etat = are.WaitOne();
//                    while (etat)
//                    {
//                    //Console.Write("Input message ('exit' to exit): ");
//                    //string msg = Console.ReadLine();
//                    string msg = Msg;
//                        if (msg == "exit")
//                        {
//                            break;
//                        }
//                        if (msg == "1")
//                        {
//                            msg = "{ \"Casque\":1, \"Nom\": \"Bruno Labrecque\", \"Club\": \"Québec\", \"Tour\": 3  , \"Temps\" : \"Bruno Labrecque\" }";
//                        }
//                    if (msg == "2")
//                    {
//                        msg = "{ \"Casque\":2, \"Nom\": \"Bruno Labrecque\", \"Club\": \"Québec\", \"Tour\": 3  , \"Temps\" : \"Bruno Labrecque\" }";
//                    }
//                    ArraySegment<byte> bytesToSend = new ArraySegment<byte>(
//                            Encoding.UTF8.GetBytes(msg));
//                        await ws.SendAsync(
//                            bytesToSend, WebSocketMessageType.Text,
//                            true, CancellationToken.None);
//                        ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
//                        WebSocketReceiveResult result = await ws.ReceiveAsync(
//                            bytesReceived, CancellationToken.None);
//                        Console.WriteLine(Encoding.UTF8.GetString(
//                            bytesReceived.Array, 0, result.Count));
//                        if (ws.State != WebSocketState.Open)
//                        {
//                            break;
//                        }
//                    }
//                }
//            }

//            public static void Main(AutoResetEvent are)
//            {
//                Task t = ChatWithServer(are);
            
//                //t.Wait();
//            }        

//        public static string Msg { get; set; }
//    }
//}
