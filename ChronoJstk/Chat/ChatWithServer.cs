//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.IO;
//using System.Linq;
//using System.Net.WebSockets;
//using System.Runtime.Serialization.Json;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Windows;

//namespace ChronoJstk.Chat
//{
//    class ChatWithServer
//    {
//        ClientWebSocket ws = null;
//        public ChatWithServer()
//        {
//            ws = new ClientWebSocket();            
//        }

//        //public void init2()
//        //{
//        //    string url = ConfigurationManager.AppSettings["DiffusionWeb"];
//        //    Uri serverUri = new Uri(url);
//        //    ws.ConnectAsync(serverUri, CancellationToken.None);
//        //}

//        async public Task init()
//        {
//            string url = ConfigurationManager.AppSettings["DiffusionWeb"];
//            Uri serverUri = new Uri(url);
//            await ws.ConnectAsync(serverUri, CancellationToken.None);
//            MessageBox.Show("ConnectAsync fait");
//            //Uri serverUri = new Uri("ws://localhost:61195/WSHandler.ashx");
//            //await ws.ConnectAsync(serverUri, CancellationToken.None);

//        }

//        async public Task TourPatineur(EvenementPatineur ev)
//        {
//            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(EvenementPatineur));
//            MemoryStream stream1 = new MemoryStream();
//            ser.WriteObject(stream1, ev);
//            byte[] b = stream1.GetBuffer();
//            ArraySegment<byte> bytesToSend = new ArraySegment<byte>(b);
//            await ws.SendAsync(
//                      bytesToSend, WebSocketMessageType.Text,
//                      true, CancellationToken.None);
//            //ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
//        }

//        async public Task StatutCourse(string statut)
//        {
//            string statut1 = "{\"TypeMessage\" : \"Etat\", \"Valeur\" : \"" + statut + "\"  }";
//            ArraySegment<byte> bytesToSend = new ArraySegment<byte>(
//                       Encoding.UTF8.GetBytes(statut1));
//            await ws.SendAsync(
//                bytesToSend, WebSocketMessageType.Text,
//                true, CancellationToken.None);
//            ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
//            WebSocketReceiveResult result = await ws.ReceiveAsync(
//                bytesReceived, CancellationToken.None);
//            Console.WriteLine(Encoding.UTF8.GetString(
//                bytesReceived.Array, 0, result.Count));
//            if (ws.State != WebSocketState.Open)
//            {
//                //break;
//            }
//            //statut = string.Format("{\"TypeMessage\" : \"Etat\", \"Valeur\" : \"{0}\"  }", statut);
//            //byte[] b = System.Text.UTF8Encoding.UTF8.GetBytes(statut);
//            //ArraySegment<byte> bytesToSend = new ArraySegment<byte>(b);
//            //await ws.SendAsync(
//            //          bytesToSend, WebSocketMessageType.Text,
//            //          true, CancellationToken.None);
//            ////ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
//        }

//        private async Task InitChat()
//        {
//            using (ClientWebSocket ws = new ClientWebSocket())
//            {
//                Uri serverUri = new Uri("ws://localhost:61195/WSHandler.ashx");
//                await ws.ConnectAsync(serverUri, CancellationToken.None);

//                //while (true)
//                //{
//                //    Console.Write("Input message ('exit' to exit): ");
//                //    string msg = Console.ReadLine();
//                //    if (msg == "exit")
//                //    {
//                //        break;
//                //    }
//                //    if (msg == "1")
//                //    {
//                //        msg = "{ \"Casque\":1, \"Nom\": \"Bruno Labrecque\", \"Club\": \"Québec\", \"Tour\": 3  , \"Temps\" : \"Bruno Labrecque\" }";
//                //    }
//                //    ArraySegment<byte> bytesToSend = new ArraySegment<byte>(
//                //        Encoding.UTF8.GetBytes(msg));
//                //    await ws.SendAsync(
//                //        bytesToSend, WebSocketMessageType.Text,
//                //        true, CancellationToken.None);
//                //    ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
//                //    WebSocketReceiveResult result = await ws.ReceiveAsync(
//                //        bytesReceived, CancellationToken.None);
//                //    Console.WriteLine(Encoding.UTF8.GetString(
//                //        bytesReceived.Array, 0, result.Count));
//                //    if (ws.State != WebSocketState.Open)
//                //    {
//                //        break;
//                //    }
//                //}
//            }
//        }
//    }
//}
