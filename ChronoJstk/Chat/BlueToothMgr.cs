using InTheHand.Net;
using InTheHand.Net.Sockets;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Configuration;

namespace ChronoJstk.Chat
{
    class BlueToothMgr
    {
        bool terminate = false;
        private ManualResetEvent mre;
        private System.Collections.Concurrent.ConcurrentQueue<ElementFTP> elementsFTP;
        private System.Collections.Concurrent.ConcurrentQueue<ElementDiffusion> elementsDiffusion;
        private static BlueToothMgr instance = null;
        private System.Threading.Thread BlueToothTheread;
        //private object boquant = new object();
        //private bool? pret = null;
        private readonly Guid _serviceClassId;
        private DeviceBt dev = null;
        //private bool connexionEnCours = false;
        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ElementDiffusion));
        DataContractJsonSerializer ser1 = new DataContractJsonSerializer(typeof(ElementFTP));

        //private Stack<ElementDiffusion> pile = new Stack<ElementDiffusion>();

        private BlueToothMgr()
        {
            this.mre = new ManualResetEvent(false);
            this.elementsDiffusion = new System.Collections.Concurrent.ConcurrentQueue<ElementDiffusion>();
            this.elementsFTP = new System.Collections.Concurrent.ConcurrentQueue<ElementFTP>();
            //_serviceClassId = new Guid("9bde4762-89a6-418e-bacf-fcd82f1e0677");
            _serviceClassId = new Guid("49D8DEA8-F14F-4367-81E6-64169B7813DF");
            //Messenger.Default.Register<fonctionInterne>(fonctionInterne.ViderCache, this.ViderCache);
            //Messenger.Default.Register<ElementDiffusion>
            //    (
            //    this,
            //    (action) => DiffuserMessage(action)
            //    );

            //Messenger.Default.Register<ElementFTP>
            //    (
            //    this,
            //    (action) => DiffuserFTP(action)
            //    );
        }
           
        public static BlueToothMgr Instance { get
            {
                if (instance == null)
                {
                    instance = new BlueToothMgr();
                }
                return instance;
            }
        }

// empile un message afin de l'envoyer vers FTP via bluetooth
        public void SendMessageBluetoothVersFTP(string pathFTP, string contenuFTP)
        {
            ElementFTP e = new ElementFTP();
            e.CheminFTP = pathFTP;
            e.Message = contenuFTP;
            e.SiteFTP = ParamCommuns.Instance.SiteFTP;
            e.UsagerFTP = ParamCommuns.Instance.UsagerFTP;
            e.MotPasseFTP = ParamCommuns.Instance.MotPasse;
            this.elementsFTP.Enqueue(e);
            this.mre.Set();
            //Messenger.Default.Send<ElementFTP>(e);
        }

        /// <summary>
        /// Empile un message afin de le difuser avec signalr
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public void DiffuserMessageBluetooth(Chat.ChronoSignalR.TypeMessage type, string message)
        {
            ElementDiffusion ed = new ElementDiffusion() { TypeMessage = type, Message = message };
            ed.SignalRHub = ParamCommuns.Instance.SignalRHub;
            ed.SignalRServer = ParamCommuns.Instance.SignalRServer;
            this.elementsDiffusion.Enqueue(ed);
            this.mre.Set();
            //if (this.pret.HasValue && this.pret.Value)
            //{
            //    ElementDiffusion ed = new ElementDiffusion() { TypeMessage = type, Message = message };
            //    ed.SignalRHub = ParamCommuns.Instance.SignalRHub;
            //    ed.SignalRServer = ParamCommuns.Instance.SignalRServer;
            //    Messenger.Default.Send(ed);
            //    return;
            //} 
            //else if (this.pret.HasValue && ! this.pret.Value)
            //{
            //    lock (this.boquant)
            //    {
            //        if (this.pret.HasValue && this.pret.Value)
            //        {
            //            ElementDiffusion ed = new ElementDiffusion() { TypeMessage = type, Message = message };
            //            ed.SignalRHub = ParamCommuns.Instance.SignalRHub;
            //            ed.SignalRServer = ParamCommuns.Instance.SignalRServer;
            //            Messenger.Default.Send(ed);
            //        }
            //        else { 
            //           this.pile.Push(new ElementDiffusion() { TypeMessage = type, Message = message });
            //        }
            //    }

            //    return;
            //}

            //this.pile.Push(new ElementDiffusion() { TypeMessage = type, Message = message });

        }

        /// <summary>
        /// Processus background qui traite les messages à l'infini
        /// </summary>
        private void BackgroundThradProcess()
        {
            while (true)
            {
                if (this.terminate) {
                    break;
                }
                this.mre.Reset();
                if (this.elementsDiffusion.Count >= 0)
                {
                    ElementDiffusion ed = null;
                    if (this.elementsDiffusion.TryDequeue(out ed))
                    { 
                        this.DiffuserMessage(ed);
                    }
                }
                else if (this.elementsFTP.Count >= 0)
                {
                    ElementFTP element = null;
                    if (this.elementsFTP.TryDequeue(out element))
                    { 
                        this.DiffuserFTP(element);
                    }
                }
                else {
                    this.mre.WaitOne(300000);

                }
            }
        }

        /// <summary>
        /// Shows the device.
        /// </summary>
        /// <param name="deviceMessage">The device message.</param>
        private void Connect(DeviceBt device)
        {
            this.dev = device;
            ElementDiffusion ed;
            while (this.elementsDiffusion.TryDequeue(out ed)) {
                if (ed == null)
                {

                    break;
                }
            }

            ElementFTP eftp;
            while (this.elementsFTP.TryDequeue(out eftp))
            {
                if (eftp == null)
                {
                    break;
                }
            }
            this.mre.Reset();
            this.terminate = false;
            //this.ViderCache(fonctionInterne.ViderCache);
            //Messenger.Default.Send(fonctionInterne.ViderCache);
        }

        //private BluetoothEndPoint bep = null;

        private void DiffuserFTP(ElementFTP ed)
        {
            //lock (this.boquant)
            //{
            try
            {
                //this.globalClient.Client.Close();
                using (BluetoothClient bluetoothClient = new BluetoothClient())
                {
                    var ep = new BluetoothEndPoint(this.dev.DeviceInfo.DeviceAddress, _serviceClassId);

                    bluetoothClient.Connect(ep);

                    using (var bluetoothStream = bluetoothClient.GetStream())
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            this.ser1.WriteObject(ms, ed);
                            int longueur = (int)ms.Length;
                            byte[] bx = BitConverter.GetBytes(longueur);
                            int longueurTest = bx.Length;
                            bluetoothStream.Write(bx, 0, 4);
                            byte[] sender = ms.GetBuffer();
                            int depart = 0;
                            while (longueur > 0)
                            {
                                if (longueur >= 1024)
                                {
                                    bluetoothStream.Write(sender, depart, 1024);
                                    bluetoothStream.Flush();
                                    depart += 1024;
                                    longueur -= 1024;
                                }
                                else
                                {
                                    bluetoothStream.Write(sender, depart, longueur);
                                    bluetoothStream.Flush();
                                    longueur = 0;
                                }
                            }
                        }
                    }
                }
                ////////////this.globalClient.Dispose();
                ////////////this.globalClient = null;
                ////////////if (this.globalClient == null)
                ////////////{
                ////////////    this.globalClient = new BluetoothClient();
                ////////////    var ep = new BluetoothEndPoint(this.dev.DeviceInfo.DeviceAddress, _serviceClassId);
                ////////////    this.globalClient.Connect(ep);
                ////////////    this.globalStream = this.globalClient.GetStream();
                ////////////}
                //////////////this.globalStream.Close();
                //////////////this.globalStream = this.globalClient.GetStream();
                ////////////using (MemoryStream ms = new MemoryStream())
                ////////////{
                ////////////    this.ser1.WriteObject(ms, ed);
                ////////////    int longueur = (int)ms.Length;
                ////////////    byte[] bx = BitConverter.GetBytes(longueur);
                ////////////    int longueurTest = bx.Length;
                ////////////    globalStream.Write(bx, 0, 4);
                ////////////    byte[] sender = ms.GetBuffer();
                ////////////    int depart = 0;
                ////////////    while (longueur > 0)
                ////////////    {
                ////////////        if (longueur >= 1024)
                ////////////        {
                ////////////            globalStream.Write(sender, depart, 1024);
                ////////////            globalStream.Flush();
                ////////////            depart += 1024;
                ////////////            longueur -= 1024;
                ////////////        }
                ////////////        else
                ////////////        {
                ////////////            globalStream.Write(sender, depart, longueur);
                ////////////            globalStream.Flush();
                ////////////            longueur = 0;
                ////////////        }
                ////////////    }


                ////////////}


                //using (BluetoothClient bluetoothClient = new BluetoothClient())
                //{

                //    var bep = new BluetoothEndPoint(this.dev.DeviceInfo.DeviceAddress, _serviceClassId);


                //    // connecting
                //    bluetoothClient.Connect(bep);

                //    using (var bluetoothStream = bluetoothClient.GetStream())
                //    {
                //        using (MemoryStream ms = new MemoryStream())
                //        {
                //            this.ser1.WriteObject(ms, ed);
                //            var buffer = ms.GetBuffer();
                //            bluetoothStream.Write(buffer, 0, buffer.Length);
                //            bluetoothStream.Flush();
                //            bluetoothStream.Close();
                //        }
                //    }
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur de messagerie" + e.ToString());
            }
            //}
        }

        private StreamWriter ecrivain;
        private int noLigne = 0;
        private bool log = true;
        private void Ecrivain(string s) {
            if (this.log) { 
                if (this.ecrivain == null) {
                    string path = ConfigurationManager.AppSettings["PathLogEcrivain"];
                    if (!System.IO.Directory.Exists(path))
                    {
                        this.log = false;
                        return;
                    }
                    string fichier = System.IO.Path.Combine(path, "Emetteur_" + System.Guid.NewGuid().ToString() + ".txt");

                    this.ecrivain = new StreamWriter(fichier, true);
                    this.ecrivain.WriteLine("DEBUT DU LOG---------------------------------------------");
                    this.ecrivain.Flush();
                }

            this.ecrivain.WriteLine(string.Format("No {0} : ", noLigne++) + s);
            this.ecrivain.Flush();

            }

        } 
        //BluetoothClient globalClient = null;
        //Stream globalStream = null;

        private void DiffuserMessage(ElementDiffusion ed)
        {
            try
            {

                using (BluetoothClient bluetoothClient = new BluetoothClient())
                {
                    var ep = new BluetoothEndPoint(this.dev.DeviceInfo.DeviceAddress, _serviceClassId);

                    bluetoothClient.Connect(ep);

                    using (var bluetoothStream = bluetoothClient.GetStream())
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            this.ser.WriteObject(ms, ed);
                            string s = System.Text.UTF8Encoding.UTF8.GetString(ms.GetBuffer());
                            ms.Seek(0, SeekOrigin.Begin);
                            this.Ecrivain(s);
                            int longueur = (int)ms.Length;
                            byte[] bx = BitConverter.GetBytes(longueur);
                            int longueurTest = bx.Length;
                            bluetoothStream.Write(bx, 0, 4);
                            byte[] sender = ms.GetBuffer();
                            int depart = 0;
                            while (longueur > 0)
                            {
                                if (longueur >= 1024)
                                {
                                    bluetoothStream.Write(sender, depart, 1024);
                                    bluetoothStream.Flush();
                                    depart += 1024;
                                    longueur -= 1024;
                                }
                                else
                                {
                                    bluetoothStream.Write(sender, depart, longueur);
                                    bluetoothStream.Flush();
                                    longueur = 0;
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                this.Ecrivain("ERREUR --------------------------");
                this.Ecrivain(e.ToString());
            }
            ////////lock (this.boquant)
            ////////{
            ////////    try
            ////////    {
            ////////        if (this.globalClient == null)
            ////////        {
            ////////            this.globalClient = new BluetoothClient();
            ////////            var ep = new BluetoothEndPoint(this.dev.DeviceInfo.DeviceAddress, _serviceClassId);
            ////////            this.globalClient.Connect(ep);
            ////////            this.globalStream = this.globalClient.GetStream();

            ////////        }

            ////////        using (MemoryStream ms = new MemoryStream())
            ////////        {                        
            ////////            this.ser.WriteObject(ms, ed);
            ////////            int longueur = (int)ms.Length;
            ////////            byte[] bx = BitConverter.GetBytes(longueur);
            ////////            int longueurTest = bx.Length;
            ////////            globalStream.Write(bx, 0, 4);
            ////////            byte[] sender = ms.GetBuffer();
            ////////            int depart = 0;
            ////////            while (longueur > 0)
            ////////            {
            ////////                if (longueur >= 1024)
            ////////                {
            ////////                    globalStream.Write(sender, depart, 1024);
            ////////                    globalStream.Flush();
            ////////                    depart += 1024;
            ////////                    longueur -= 1024;
            ////////                }
            ////////                else
            ////////                {
            ////////                    globalStream.Write(sender, depart, longueur);
            ////////                    globalStream.Flush();
            ////////                    longueur = 0;
            ////////                }
            ////////            }
            ////////            //globalStream.Write(ms.GetBuffer(), 0, longueur);
            ////////            //globalStream.Flush();
            ////////        }


            ////////        //using (BluetoothClient bluetoothClient = new BluetoothClient())
            ////////        //{

            ////////        //    var bep = new BluetoothEndPoint(this.dev.DeviceInfo.DeviceAddress, _serviceClassId);


            ////////        //    // connecting
            ////////        //    bluetoothClient.Connect(bep);



            ////////        //    using (var bluetoothStream = bluetoothClient.GetStream())
            ////////        //    {
            ////////        //        using (MemoryStream ms = new MemoryStream())
            ////////        //        {
            ////////        //            this.ser.WriteObject(ms, ed);

            ////////        //            var buffer = ms.GetBuffer();
            ////////        //            bluetoothStream.Write(buffer, 0, buffer.Length);
            ////////        //            bluetoothStream.Flush();
            ////////        //            bluetoothStream.Close();
            ////////        //        }
            ////////        //    }
            ////////        //}
            ////////    }
            ////////    catch (Exception e)
            ////////    {
            ////////        Console.WriteLine("Erreur de messagerie");
            ////////    }
            ////////}
        }

        //private async void ViderCache(fonctionInterne m)
        //{
        //    if (m == fonctionInterne.ViderCache)
        //    {
        //        while (true)
        //        {
        //            ElementDiffusion ed = null;
        //            lock (this.boquant)
        //            {
        //                if (this.pile.Count == 0)
        //                {
        //                    this.pret = true;
        //                    return;
        //                }
        //                ed = this.pile.Pop();
        //            }
        //            Messenger.Default.Send(ed);
        //        }
        //    }            
        //}

        public void Terminer()
        {
             if (this.BlueToothTheread != null)
            {
                this.terminate = true;
                this.BlueToothTheread.Abort();
                this.BlueToothTheread = null;
            }
        }

        public void Configurer()
        {
            //if (this.dev == null && !this.connexionEnCours)
            //{
            //    this.connexionEnCours = true;
                SelectionBT w = new SelectionBT();
                bool? r = w.ShowDialog();

                if (r.HasValue && r.Value)
                {
                    this.dev = w.Liste.SelectedItem as DeviceBt;
                    this.Connect(this.dev);
                ThreadStart ts = new ThreadStart(this.BackgroundThradProcess);
                this.BlueToothTheread = new Thread(ts);
                this.BlueToothTheread.Start();
                }
            //}
        }

        //public enum fonctionInterne
        //{
        //    ViderCache = 4,
        //    AfficherMessage = 5,
        //    CopieFTP = 6
        //}            
        
        [DataContract]
        public class ElementDiffusion
        {
            [DataMember]
            public Chat.ChronoSignalR.TypeMessage TypeMessage { get; set; }
            [DataMember]
            public string Message { get; set; }
            [DataMember]
            public string SignalRHub { get; set; }
            [DataMember]
            public string SignalRServer { get; set; }

        }

        [DataContract]
        public class ElementFTP 
        {
            [DataMember]
            public string SiteFTP { get; set; }
            [DataMember]
            public string UsagerFTP { get; set; }
            [DataMember]
            public string MotPasseFTP { get; set; }
            [DataMember]
            public string CheminFTP { get; set; }
            [DataMember]
            public string Message { get; set; }
        }
    }
}
