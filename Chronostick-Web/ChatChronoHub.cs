using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Chronostick_Web
{
    [HubName("chatChronoHub")]
    public class ChatChronoHub : Hub
    {
        public static Dictionary<int, string> messagesPatineur = new Dictionary<int, string>();
        public static Dictionary<string, string> messageGeneric = new Dictionary<string, string>();
        public ChatChronoHub()
        {
        }

        //[HubMethodName("sendMessage")]
        //public void SendMessage(string username, string message)
        //{
        //    Clients.All.displayMessage(string.Format("{0} dit : {1}", username, message));
        //}

        [HubMethodName("message")]
        public void message(string typeMessage, string contenuMessage)
        {
            if (messageGeneric.ContainsKey(typeMessage))
            {
                messageGeneric[typeMessage] = contenuMessage;
            }
            else
            {
                messageGeneric.Add(typeMessage, contenuMessage);
            }
            Clients.All.displayMessage("{" + string.Format("\"TypeMessage\" : \"{0}\", \"Message\" : \"{1}\"", typeMessage, contenuMessage) + "}");
        }

        [HubMethodName("messagesClient")]
        public void messagesClient()
        {

            foreach (KeyValuePair<string, string> kvp in messageGeneric) {
                Clients.Client(Context.ConnectionId).displayMessage("{"  + string.Format("\"TypeMessage\" : \"{0}\", \"Message\" : \"{1}\"", kvp.Key, kvp.Value) + "}");
            }
            
        }

        [HubMethodName("tempsPatineur")]
        public void tempsPatineur(int noCasque, string message)
        {
            if (messagesPatineur.ContainsKey(noCasque))
            {
                messagesPatineur[noCasque] = message;
            }
            else
            {
                messagesPatineur.Add(noCasque, message);
            }
            Clients.All.displayMessage(message);
        }

        [HubMethodName("tempsTousPatineur")]
        public void TempsTousPatineur()
        {
            foreach (KeyValuePair<int, string> kvp in messagesPatineur)
            {
                Clients.Client(Context.ConnectionId).displayMessage(kvp.Value);
            }            
        }

        public override Task OnConnected()
        {
            //Clients.Client(Context.ConnectionId).promptForLogin();
            Clients.Client(Context.ConnectionId).displayMessage("{ \"TypeMessage\" : \"Etat\", \"Message\" : \"Connecté\"}");
            TempsTousPatineur();
            messagesClient();

            return base.OnConnected();            
        }
    }
}