using ChronoStick_Affaires;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace ChronoStick_Input.Model
{
    public class ChronostickInputViewModel: INotifyPropertyChanged
    {
        private Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
ProtocolType.Udp);

        private JoyStickManager joystickManager = null;
        private IPEndPoint endPoint = null;
        private IPAddress serverAddr = null;
        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Evenement));
        DataContractJsonSerializer serMsg = new DataContractJsonSerializer(typeof(MessageConfig));

        private static ChronostickInputViewModel instance = null;

        public event PropertyChangedEventHandler PropertyChanged;

        private Evenement DepartPrecedent = null;

        private Dictionary<int, Evenement> TourPrecedent = new Dictionary<int, Evenement>();

        private Timer aTimer;
        private Timer bTimer;
        private double intervale = double.MinValue;
        private ChronostickInputViewModel() 
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            this.TitreFenetre = string.Format("{1} {0}", version, ChronoStick_Input.Properties.Resources.NomLog);

            this.joystickManager = new JoyStickManager();
            this.joystickManager.BoutonEvent += JoystickManager_BoutonEvent;
            this.joystickManager.JoyStickError += JoystickManager_JoyStickError;
            this.TexteEtat = ChronoStick_Input.Properties.Resources.EtatMsgCon; // "Connecté";
            this.CouleurEtat = new SolidColorBrush(Colors.Green);
            this.ConnectVisible = Visibility.Collapsed;
            this.joystickManager.CaptureJoyStick();

            string nbSecPing = System.Configuration.ConfigurationManager.AppSettings["NbSecPing"];
            int delais = this.delais;
            int.TryParse(nbSecPing, out delais);

            string nbPatineur = System.Configuration.ConfigurationManager.AppSettings["NbPatineur"];
            int nbPatineurDep = this.nbPatineurDep;
            int.TryParse(nbPatineur, out nbPatineurDep);
            this.NbPatineurDep = nbPatineurDep;

            string noPortMessages = System.Configuration.ConfigurationManager.AppSettings["NoPortMessages"];
            int noPort = this.noPort;
            int.TryParse(noPortMessages, out noPort);
            this.NoPort = noPort;

            string hostsMessages = System.Configuration.ConfigurationManager.AppSettings["HostMessages"];
            this.HostMessages = hostsMessages;

            string noPortConfig = System.Configuration.ConfigurationManager.AppSettings["NoPortConfig"];
            noPort = this.noPortConfig;
            int.TryParse(noPortConfig, out noPort);
            this.NoPortConfig = noPort;

            string nbSecTourMinCfg = System.Configuration.ConfigurationManager.AppSettings["NbSecTourMin"];
            noPort = this.nbSecTourMin;
            int.TryParse(nbSecTourMinCfg, out noPort);
            this.NbSecTourMin = noPort;

            string nbSecDepMinCfg = System.Configuration.ConfigurationManager.AppSettings["NbSecDepMin"];
            noPort = this.nbSecDepMin;
            int.TryParse(nbSecDepMinCfg, out noPort);
            this.NbSecDepMin = noPort;

            if (delais > 0) 
            {
                this.intervale = this.delais * 1000;
                aTimer = new Timer();
                aTimer.Interval = this.intervale;
                aTimer.Elapsed += aTimer_Elapsed;
                aTimer.AutoReset = true;
                aTimer.Enabled = true;
            }

            bTimer = new Timer();
            bTimer.Interval = 3000;
            bTimer.Elapsed += BTimer_Elapsed;
            bTimer.AutoReset = false;
            bTimer.Enabled = false;

            //this.journal.CollectionChanged += journal_CollectionChanged;

            // Initialiser la réception de configuraiton
            UdpClient udpClient = new UdpClient();
            udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, this.NoPortConfig));

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

        private void BTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.taskBar.HideBalloonTip();
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
            MessageConfig msgCfg = (MessageConfig)this.serMsg.ReadObject(ms);
            switch(msgCfg.TypeMessage)
            {
                case TypeMessageConfig.Terminer:
                    {
                        (Application.Current as ChronoStick_Input.App).Exit();
                        break;
                    }
                case TypeMessageConfig.DelaisDepart:
                    {
                        this.NbSecDepMin = msgCfg.ValeurMessage;
                        break;
                    }
                case TypeMessageConfig.DelaisPing:
                    {
                        this.Delais = msgCfg.ValeurMessage;
                        this.intervale = this.Delais * 1000;
                        break;
                    }

                case TypeMessageConfig.DelaisTourPatineur:
                    {
                        this.NbSecTourMin = msgCfg.ValeurMessage;
                        break;
                    }
                case TypeMessageConfig.NombrePatineur:
                    {
                        this.NbPatineurDep = msgCfg.ValeurMessage;
                        break;
                    }
            }
            
            string title = ChronoStick_Input.Properties.Resources.MsgBaloon; // "Chronostick-Input Configuration";
            string text = string.Format("{0} = {1}", msgCfg.TypeMessage, msgCfg.ValeurMessage);

            this.taskBar.ShowBalloonTip(title, text, BalloonIcon.Info);
            bTimer.Enabled = true;
            bTimer.Start();

        }

        private void JoystickManager_JoyStickError(object sender, Exception e)
        {
            this.TexteEtat = ChronoStick_Input.Properties.Resources.EtatMsgErr; // "Erreur";
            this.CouleurEtat = new SolidColorBrush(Colors.Red);
            this.ConnectVisible = Visibility.Visible;
        }

        public void Reconnecter()
        {
            if (this.joystickManager != null)
            {
                this.joystickManager.BoutonEvent -= this.JoystickManager_BoutonEvent;
                this.joystickManager.JoyStickError -= this.JoystickManager_JoyStickError;
                this.joystickManager = null;
            }

            this.joystickManager = new JoyStickManager();
            this.joystickManager.BoutonEvent += this.JoystickManager_BoutonEvent;
            this.joystickManager.JoyStickError += this.JoystickManager_JoyStickError;
            this.TexteEtat = ChronoStick_Input.Properties.Resources.EtatMsgCon; // "Connecté";
            this.ConnectVisible = Visibility.Collapsed;
            this.CouleurEtat = new SolidColorBrush(Colors.Green);
            this.joystickManager.CaptureJoyStick();
        }

        private void JoystickManager_BoutonEvent(object sender, BoutonEventArgs e)
        {
            if (e != null)
            {
                if (e.NoBouton == 0 && e.Enfonce)
                {
                    this.Depart(OrigineEvenement.USB, e.Heure);
                }
                else if (e.Enfonce)
                {
                    this.Tour(e.NoBouton, OrigineEvenement.USB, e.Heure);
                }
            }
        }

        //void journal_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    if (this.journal.Count > 500) 
        //    {
        //        this.journal.CollectionChanged -= this.journal_CollectionChanged;
        //        Timer t = new Timer(2000);
        //        t.Elapsed += T_Elapsed;
        //        t.AutoReset = false;
        //        t.Start();
        //    }
        //}

        //private void T_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    while (this.journal.Count > 500)
        //    {
        //        this.journal.RemoveAt(0);
        //    }

        //    this.journal.CollectionChanged += this.journal_CollectionChanged;
        //    Timer t = sender as Timer;
        //    t.Stop();
        //    t.Dispose();
        //}

        void aTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.aTimer.Interval != this.intervale) 
            {
                this.aTimer.Interval = this.intervale;
            }

            Evenement ev = new Evenement() { NoPatineur = 0, Action = TypeEvenement.Ping, Heure = DateTime.Now, Origine = OrigineEvenement.Systeme, Doublon = false };
            this.AjouterJournal(ev);

            // Jamais plus que 500 lignes dans le journal.
            if (this.Dispatcher != null && this.Journal.Count > 50 )
            {
                Dispatcher.Invoke((Action)delegate ()
                {
                    while (this.journal.Count > 500)
                    {
                        this.journal.RemoveAt(0);
                    }
                });
            }
        }

        public static ChronostickInputViewModel Instance
        {
            get 
            {
                if (instance == null) 
                {
                    instance = new ChronostickInputViewModel();
                }

                return instance;
            }
        }

        private TaskbarIcon taskBar;

        public void SetTaskBar(TaskbarIcon taskBar)
        {
            this.taskBar = taskBar;
        }

        private Visibility connectVisible = Visibility.Visible;
        public Visibility ConnectVisible { get { return this.connectVisible; } set { this.connectVisible = value; this.NotifierChangementPropriete("ConnectVisible"); } }

        public SolidColorBrush couleurEtat;
        public SolidColorBrush CouleurEtat { get { return this.couleurEtat; } set { this.couleurEtat = value; this.NotifierChangementPropriete("CouleurEtat"); } }

        public string texteEtat;
        public string TexteEtat { get { return this.texteEtat; } set { this.texteEtat = value; this.NotifierChangementPropriete("TexteEtat"); } }

        // this.TexteEtat = "Erreur";

        //this.CouleurEtat

        private int noPort = 777;
        public int NoPort { get { return this.noPort; } set { this.noPort = value; this.NotifierChangementPropriete("NoPort"); } }

        private int noPortConfig = 777;
        public int NoPortConfig { get { return this.noPortConfig; } set { this.noPortConfig = value; this.NotifierChangementPropriete("NoPortConfig"); } }

        private int delais = 5;
        public int Delais { get { return this.delais; } set { this.delais = value; this.NotifierChangementPropriete("Delais"); } }

        private int nbPatineurDep = 5;
        public int NbPatineurDep { get { return this.nbPatineurDep; } set { this.nbPatineurDep = value; this.NotifierChangementPropriete("NbPatineurDep"); } }

        private int nbSecTourMin= 5;
        public int NbSecTourMin { get { return this.nbSecTourMin; } set { this.nbSecTourMin = value; this.NotifierChangementPropriete("NbSecTourMin"); } }

        private int nbSecDepMin = 5;
        public int NbSecDepMin { get { return this.nbSecDepMin; } set { this.nbSecDepMin = value; this.NotifierChangementPropriete("NbSecDepMin"); } }

        private string titreFenetre = string.Empty;
        public string TitreFenetre { get { return this.titreFenetre; } set { this.titreFenetre = value; this.NotifierChangementPropriete("TitreFenetre"); } }

        private string hostMessages = string.Empty;
        public string HostMessages { get { return this.hostMessages; } set { this.hostMessages = value; this.NotifierChangementPropriete("HostMessages"); } }

        private ObservableCollection<string> journal = new ObservableCollection<string>();
        public ObservableCollection<string> Journal { get { return this.journal; } }

        private void NotifierChangementPropriete(string nom)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(nom));
            }
        }

        public void Terminer()
        {
            DateTime heure = DateTime.Now;
            Evenement ev = new Evenement() { NoPatineur = 0, Action = TypeEvenement.Terminer, Heure = heure, Origine = OrigineEvenement.Systeme, Doublon = false };
            this.AjouterJournal(ev);
        }

        private void Depart(OrigineEvenement origine, DateTime heure)
        {
            bool doublon = false;
            if (this.DepartPrecedent != null)
            {
                TimeSpan ts = heure - this.DepartPrecedent.Heure;
                if (ts.TotalSeconds < this.NbSecTourMin)
                {
                    doublon = true;
                }
            }

            bool premier = true;
            for (int i = 1; i <= this.NbPatineurDep; i++)
            {
                Evenement ev = new Evenement() { NoPatineur = i, Action = TypeEvenement.Depart, Heure = heure, Origine = origine, Doublon = doublon };
                this.AjouterJournal(ev);

                if (!doublon && premier)
                {
                    this.DepartPrecedent = ev;
                    premier = false;
                }
            }
        }

        public void Depart(OrigineEvenement origine) 
        {
            this.Depart(origine, DateTime.Now);           
        }

        private void Tour(int noPat, OrigineEvenement origine, DateTime heure)
        {
            bool doublon = false;
            if (this.TourPrecedent.ContainsKey(noPat))
            {
                TimeSpan ts = heure - this.TourPrecedent[noPat].Heure;
                if (ts.TotalSeconds < this.NbSecTourMin)
                {
                    doublon = true;
                }
            }

            Evenement ev = new Evenement() { NoPatineur = noPat, Action = TypeEvenement.Tour, Heure = heure, Origine = origine, Doublon = doublon };
            this.AjouterJournal(ev);

            if (!doublon)
            {
                if (this.TourPrecedent.ContainsKey(noPat))
                {
                    this.TourPrecedent[noPat] = ev;
                }
                else
                {
                    this.TourPrecedent.Add(noPat, ev);
                }
            }
        }
      
        public void Tour(int noPat, OrigineEvenement origine) 
        {
            this.Tour(noPat, origine, DateTime.Now);
        }

        public void AjouterJournal(Evenement e) 
        {
            if (e == null)
            {
                return;
            }

            if (this.Dispatcher != null)
            {
                Dispatcher.Invoke((Action)delegate() { this.journal.Add(e.Affichage); });
            }
            else 
            {
                this.journal.Add(e.Affichage);
            }

            MemoryStream stream1 = new MemoryStream();            
            this.ser.WriteObject(stream1, e);
            byte[] b = stream1.GetBuffer();

            if (this.endPoint == null)
            {
                this.serverAddr = IPAddress.Parse(this.HostMessages);
                this.endPoint= new IPEndPoint(serverAddr, this.NoPort);
            }
            else
            {
                if (this.endPoint.Port != this.NoPort || this.serverAddr.ToString() != this.HostMessages)
                {
                    this.serverAddr = IPAddress.Parse(this.HostMessages);
                    this.endPoint = new IPEndPoint(serverAddr, this.NoPort);
                }
            }
           
            this.sock.SendTo(b, this.endPoint);
        }

        public Dispatcher Dispatcher { get; set; }    
    }
}
