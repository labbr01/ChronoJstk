//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace ChronoJstk.Chat
//{
//    public class ChronoWeb
//    {
//        private AutoResetEvent are;
//        public ChronoWeb()
//        {
//            // Initialisation du chrono web
//            this.are = new AutoResetEvent(false);

//            Thread thread = new Thread(new ThreadStart(WorkThreadFunction));
//            this.are.Reset();
//            thread.Start();
//            this.Message = new Queue<string>();
//        }

//        private System.Collections.Generic.Queue<string> Message { get; set; }

//        public void MessageChrono(string message)
//        {
//            this.Message.Enqueue(message);
//            this.are.Set();
//        }

//        private void WorkThreadFunction()
//        {
//            Task t = Chat.WebSocketClient.Init();
//            t.Wait();
//            while (true)
//            {
//                this.are.WaitOne();
//                this.are.Reset();        
//                while (this.Message.Count > 0)
//                {
//                    string msg = this.Message.Dequeue();
//                    if (msg == "TERMINER")
//                    {
//                        break;
//                    }

//                    t = Chat.WebSocketClient.Talk(msg);
//                    t.Wait();
//                    System.Diagnostics.Debug.WriteLine("Message = " + this.Message);

//                }
//                //if (this.Message == "TERMINER")
//                //{
//                //    break;
//                //}

//                //t = Chat.WebSocketClient.Talk(this.Message);
//                //System.Diagnostics.Debug.WriteLine("Message = " + this.Message);
//            }

//        }
//    }
//}
