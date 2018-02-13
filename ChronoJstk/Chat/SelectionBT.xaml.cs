using GalaSoft.MvvmLight.Messaging;
using InTheHand.Net;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChronoJstk.Chat
{
    /// <summary>
    /// Logique d'interaction pour SelectionBT.xaml
    /// </summary>
    public partial class SelectionBT : Window
    {
       
        SenderBluetoothService _senderBluetoothService;
        private string _data;

        public SelectionBT()
        {
            InitializeComponent();
            _senderBluetoothService = new SenderBluetoothService();
            Devices = new ObservableCollection<DeviceBt>
            {
                new DeviceBt(null) { DeviceName = "Searching..." }
            };
            Messenger.Default.Register<messagesM>(messagesM.ShowDevices, this.ShowDevice);
            Messenger.Default.Register<InfoMessage>
(
     this,
     (action) => AjouterMessage(action)
);


            Messenger.Default.Send(messagesM.ShowDevices);

            this.DataContext = this;
        }

        /// <summary>
        /// Gets or sets the devices.
        /// </summary>
        /// <value>
        /// The devices.
        /// </value>
        public ObservableCollection<DeviceBt> Devices
        {
            get; set;
        }


            /// <summary>
            /// Shows the device.
            /// </summary>
            /// <param name="deviceMessage">The device message.</param>
            private async void ShowDevice(messagesM deviceMessage)
        {
            if (deviceMessage == messagesM.ShowDevices)
            {
                var items = await _senderBluetoothService.GetDevices();
                bool premier = true;
                items.ToList().ForEach(z =>
                {
                    InfoMessage im = new InfoMessage();
                    im.valeurMessage = z;
                    if (premier)
                    {
                        im.typeMessage = messagesM.AjouterPremier;
                    }
                    else
                    {
                        im.typeMessage = messagesM.AjouterAutre;
                    }
                    Messenger.Default.Send(im);

                    premier = false;
                });
            }
        }

        private void AjouterMessage(InfoMessage m)
        {
            if (m.typeMessage == messagesM.AjouterPremier)
            {
                this.Liste.Dispatcher.BeginInvoke(new Action(toto));
            }
            this.Liste.Dispatcher.BeginInvoke(new Action(() => toto1(m.valeurMessage)));
        }

        void toto()
        {
            Devices.Clear();
        }
        void toto1(DeviceBt bd)
        {
            Devices.Add(bd);
        }



        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (this.Liste.SelectedItem == null)
            {
                MessageBox.Show("Vous devez choisir un périphérique avant de quitter", "Périphérique BlueTooth", MessageBoxButton.OK);
                return;
            }

            this.DialogResult = true;
        }



        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }

    public class InfoMessage
    {
        public messagesM typeMessage { get; set; }
        public DeviceBt valeurMessage { get; set; }
    }

    public enum messagesM
    {
        none = 0,
        ShowDevices = 1,
        AjouterPremier = 2,
        AjouterAutre = 3
    }


}
