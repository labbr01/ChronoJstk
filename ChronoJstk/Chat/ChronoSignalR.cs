using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChronoJstk.Chat
{
    public class ChronoSignalR
    {     
        HubConnection _connection;
        IHubProxy proxy;

        public ChronoSignalR()
        {
            string url = "http://localhost:62751/signalr/hubs";
            string hub = "chatChronoHub";
            if (ConfigurationManager.AppSettings.HasKeys() && ConfigurationManager.AppSettings.AllKeys.Contains("SignalRServer"))
            {
                url = ConfigurationManager.AppSettings["SignalRServer"];
                hub = ConfigurationManager.AppSettings["SignalRHub"];
            }
            _connection = new HubConnection(url);
            proxy = _connection.CreateHubProxy(hub);
            _connection.Start().Wait(TimeSpan.FromMinutes(1));
        }

        public void AfficherWeb(int noPat, string message)
        {
            proxy.Invoke("tempsPatineur", noPat, message);
        }

        public void AfficherMessage(TypeMessage type, string valeur)
        {
            proxy.Invoke("message", type.ToString(), valeur);
        }

        public void AfficherMessageClient(TypeMessage type, string valeur)
        {
            proxy.Invoke("messagesClient", type.ToString(), valeur);
        }

        public enum TypeMessage
        {
            Bloc = 0,
            Serie = 1,
            Vague = 2,
            Trace = 3,
            NbTour = 4,
            TypeCourse = 5,
            EtatCourse = 6,
            DepartCourse = 7,
            ChronoCourse = 8,
            FinCourse = 9
        }
    }
}
