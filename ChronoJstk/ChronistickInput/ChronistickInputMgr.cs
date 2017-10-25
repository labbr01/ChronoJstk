using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using ChronoStick_Affaires;

namespace ChronoJstk.ChronistickInput
{
    public delegate void DelDepartEventHandler(object sender, ChronstickInputEventArgs e);
    public delegate void DelTourEventHandler(object sender, ChronstickInputEventArgs e);

    class ChronistickInputMgr
    {
        bool enTerminaison = false;
        // A delegate type for hooking up change notifications.
       
        // An event that clients can use to be notified whenever the
        // elements of the list change.
        public event DelDepartEventHandler DepartEventHandler ;
        public event DelTourEventHandler TourEventHandler;
        private int nbPat = 5;

        private Socket sock;
        private DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(MessageConfig));
        private DataContractJsonSerializer msgEvenement = new DataContractJsonSerializer(typeof(Evenement));
        private IPEndPoint endPoint = null;

        public ChronistickInputMgr()
        {
            System.Threading.Thread.Sleep(3000);
            // Vérifier si Chronostick-input tourne
            string prg = System.Configuration.ConfigurationManager.AppSettings["ChronistickInputExe"];
            System.IO.FileInfo fi = new FileInfo(prg);
            if (!fi.Exists)
            {
                MessageBox.Show(String.Format("Erreur de configuration, {0} n'existe pas!", prg), "ERREUR", MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }

            Process[] pname = Process.GetProcessesByName(fi.Name.Substring(0,fi.Name.IndexOf('.')));
            if (pname.Length == 0 ) {
                ProcessStartInfo psi = new ProcessStartInfo(prg);
                Process.Start(psi);
                System.Threading.Thread.Sleep(3000);
            }
            


            sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);            
            IPAddress serverAddr = null;

            string host = System.Configuration.ConfigurationManager.AppSettings["HostConfig"];
            string portStr = System.Configuration.ConfigurationManager.AppSettings["PortConfig"];
            int port = System.Convert.ToInt32(portStr);

            serverAddr = IPAddress.Parse(host);  //"10.15.32.161"); //"192.168.0.154");
            endPoint = new IPEndPoint(serverAddr, port);

            // Initialiser la réception de configuraiton
            string portInputStr = System.Configuration.ConfigurationManager.AppSettings["PortInput"]; 
            int portInput = System.Convert.ToInt32(portInputStr);
            UdpClient udpClient = new UdpClient();
            udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, portInput));

            var from = new IPEndPoint(0, 0);
            Task.Run(() =>
            {
                while (true)
                {
                    var recvBuffer = udpClient.Receive(ref from);
                    this.TraiterMessage(recvBuffer);
                }
            });
        }

        private void TraiterMessage(byte[] message)
        {
            int i = message.Length - 1;
            while (message[i] == 0)
                --i;
            // now message[i] is the last non-zero byte
            byte[] messaget = new byte[i + 1];
            Array.Copy(message, messaget, i + 1);

            MemoryStream ms = new MemoryStream(messaget);
            Evenement msgCfg = (Evenement)this.msgEvenement.ReadObject(ms);
            switch(msgCfg.Action)
            {
                case TypeEvenement.Depart:
                    {
                        if (this.DepartEventHandler != null) {
                           DepartEventHandler(this, new ChronstickInputEventArgs() { Evenement = msgCfg} );
                        }
                        break;
                    }
                case TypeEvenement.Tour:
                    {
                        if (this.TourEventHandler != null)
                        {
                           TourEventHandler(this, new ChronstickInputEventArgs() { Evenement = msgCfg });
                        }
                        break;
                    }
                case TypeEvenement.Terminer:
                    {
                        if (this.enTerminaison)
                        {
                            return;
                        }                        
                            // On a fermer le programme d'input, on le repart!
                        System.Threading.Thread.Sleep(2000);
                        string prg = System.Configuration.ConfigurationManager.AppSettings["ChronistickInputExe"];
                        System.IO.FileInfo fi = new FileInfo(prg);
                        if (!fi.Exists)
                        {
                            MessageBox.Show(String.Format("Erreur de configuration, {0} n'existe pas!", prg), "ERREUR", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        Process[] pname = Process.GetProcessesByName(fi.Name.Substring(0, fi.Name.IndexOf('.')));
                        if (pname.Length == 0)
                        {
                            ProcessStartInfo psi = new ProcessStartInfo(prg);
                            Process.Start(psi);
                            System.Threading.Thread.Sleep(1000);
                            this.ConfigurerNbPatineur(this.nbPat);                            
                        }
                        break;
                    }
                case TypeEvenement.Ping:
                    {
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        public void ConfigurerNbPatineur(int nbPat)
        {
            this.nbPat = nbPat;
            MessageConfig message = new MessageConfig();
            message.TypeMessage = TypeMessageConfig.NombrePatineur;
            message.ValeurMessage = nbPat;
            MemoryStream stream1 = new MemoryStream();
            ser.WriteObject(stream1, message);
            byte[] b = stream1.GetBuffer();
            try
            {
                sock.SendTo(b, endPoint);
            }
            catch (Exception e)
            {
                // Rien faire si chronistick-input n'est pas à l'écoute
            }
        }

        public void ConfigurerDelaisPing(int delais)
        {
            MessageConfig message = new MessageConfig();
            message.TypeMessage = TypeMessageConfig.DelaisPing;
            message.ValeurMessage = delais;
            MemoryStream stream1 = new MemoryStream();
            ser.WriteObject(stream1, message);
            byte[] b = stream1.GetBuffer();
            sock.SendTo(b, endPoint);
        }

        public void ConfigurerDelaisTour(int delais)
        {           
            MessageConfig message = new MessageConfig();
            message.TypeMessage = TypeMessageConfig.DelaisTourPatineur;
            message.ValeurMessage = delais;
            MemoryStream stream1 = new MemoryStream();
            ser.WriteObject(stream1, message);
            byte[] b = stream1.GetBuffer();
            sock.SendTo(b, endPoint);
        }

        public void ConfigurerDelaisDepart(int delais)
        {            
            MessageConfig message = new MessageConfig();
            message.TypeMessage = TypeMessageConfig.DelaisDepart;
            message.ValeurMessage = delais;
            MemoryStream stream1 = new MemoryStream();
            ser.WriteObject(stream1, message);
            byte[] b = stream1.GetBuffer();
            sock.SendTo(b, endPoint);
        }

        public void Terminer(int delais)
        {
            this.enTerminaison = true;
            MessageConfig message = new MessageConfig();
            message.TypeMessage = TypeMessageConfig.Terminer;
            message.ValeurMessage = delais;
            MemoryStream stream1 = new MemoryStream();
            ser.WriteObject(stream1, message);
            byte[] b = stream1.GetBuffer();
            try
            {
                sock.SendTo(b, endPoint);
            }
            catch (Exception e)
            {
                // Ne rien faire si chronostick-input n'est pas à l'écoute
            }
        }
    }
}
