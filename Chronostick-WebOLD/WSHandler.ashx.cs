using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebSockets;

namespace Chronostick_Web
{
    /// <summary>
    /// Description résumée de WSHandler
    /// </summary>
    public class WSHandler : IHttpHandler
    {
        // Liste des sockets connectés
        static List<WebSocket> lws = new List<WebSocket>();

        /// <summary>
        /// Traitement de la requête
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            if (context.IsWebSocketRequest)
            {
                context.AcceptWebSocketRequest(ProcessWSChat);                
            }
        }

        public bool IsReusable { get { return false; } }

        private async Task ProcessWSChat(AspNetWebSocketContext context)
        {            
            WebSocket socket = context.WebSocket;
            if (!lws.Contains(socket))
            {
                lws.Add(socket);
            }
            while (true)
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
                WebSocketState wss = socket.State;
                WebSocketReceiveResult result = await socket.ReceiveAsync(
                    buffer, CancellationToken.None);
                if (socket.State == WebSocketState.Open)
                {
                    string userMessage = Encoding.UTF8.GetString(
                        buffer.Array, 0, result.Count);
                    //userMessage = "You sent: " + userMessage + " at " +
                    //    DateTime.Now.ToLongTimeString();
                    buffer = new ArraySegment<byte>(
                        Encoding.UTF8.GetBytes(userMessage));
                    await socket.SendAsync(
                        buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                    //lws.Where(z => z != socket).ToList().ForEach(z => z.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None));
                }
                else
                {
                    if (socket.State == WebSocketState.Closed)
                    {
                        if (lws.Contains(socket))
                        {
                            lws.Remove(socket);
                        }
                    }

                    break;
                }
            }
        }
    }
}