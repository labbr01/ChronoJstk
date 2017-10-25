using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ChronoStick_Input
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");

            // On démarre le view model même si la fenêtre n'est pas affichée
            Model.ChronostickInputViewModel.Instance.SetTaskBar(notifyIcon);
        }
       
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Erreur non traitée :" + e.ToString());
            e.Handled = true;

        }

        public void Exit()
        {           
            Dispatcher.Invoke((Action)delegate ()
            {
                try
                {
                    this.Shutdown();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Model.ChronostickInputViewModel.Instance.Terminer();
            notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
        }

    }
}
