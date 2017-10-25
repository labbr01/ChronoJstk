//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading.Tasks;

//namespace TestChrono1
//{
   
//    class Class1
//    {
//        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

//        IPEndPoint endPoint;
//        byte[] buffer;
//        int Bouton = 0, size = 8;
//        Byte[] rep = new Byte[32767];
//        string stringData;

//        public Class1()
//        {
//            endPoint = new IPEndPoint(IPAddress.Parse("192.168.0.154"), 2020);
//        }

//        private void button1_Click(object sender, EventArgs e)
//        {
//            string text = textBox1.Text;
//            buffer = Encoding.ASCII.GetBytes(text);
//            socket.SendTo(buffer, endPoint);
//        }


//        private void button5_Click(object sender, EventArgs e)
//        {
//            if (Bouton == 0)
//            {
//                button5.Text = "Déconnexion";
//                Bouton = 1;
//                socket.BeginConnect(endPoint, new AsyncCallback(BeginConnect), socket);
//            }
//            else
//            {
//                button5.Text = "Connexion";
//                Bouton = 0;
//                socket.Close();
//            }

//        }
//        public void BeginConnect(IAsyncResult ar)
//        {
//            socket = (Socket)ar.AsyncState;
//            try
//            {

//                socket.BeginReceive(rep, 0, size, SocketFlags.None, new AsyncCallback(CallbackMethod), socket);
//            }
//            catch
//            {
//                //erreur
//            }
//        }
//        private void CallbackMethod(IAsyncResult ar)
//        {
//            Socket remote = (Socket)ar.AsyncState;
//            int recv = remote.EndReceive(ar);
//            socket.BeginReceive(rep, 0, size, SocketFlags.None, new AsyncCallback(CallbackMethod), socket);
//            stringData = Encoding.ASCII.GetString(rep, 0, recv);
//            this.Invoke(new EventHandler(DisplayText));
//        }
//        private void DisplayText(object sender, EventArgs e)
//        {
//            textBox2.Text = stringData;
//        }
//        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
//        {
//            socket.Close();
//        }



//    }
//}
