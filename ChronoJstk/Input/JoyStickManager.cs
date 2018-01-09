namespace ChronoStick_Input.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Timers;
    using System.Windows.Controls;
    using System.Windows.Threading;
    using SharpDX.DirectInput;

    public class JoyStickManager
    {
        public delegate void BoutonDelegue(object sender, BoutonEventArgs e);
        public delegate void ErreurDelegue(object sender, Exception e);
        public event BoutonDelegue BoutonEvent;
        public event ErreurDelegue JoyStickError;
        private Joystick joystick;
        private readonly Dictionary<int, bool> etatBouton;

        public JoyStickManager()
        {
            this.etatBouton = new Dictionary<int, bool>();
            for (int i = 0; i < 16; i++)
            {
                this.etatBouton.Add(i, false);
            }
        }

        public void CaptureJoyStick()
        {
            // Initialize DirectInput
            var directInput = new DirectInput();

            // Find a Joystick Guid
            var joystickGuid = Guid.Empty;

            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
                joystickGuid = deviceInstance.InstanceGuid;

            // If Gamepad not found, look for a Joystick

            if (joystickGuid == Guid.Empty)
                foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
                    joystickGuid = deviceInstance.InstanceGuid;

            // If Joystick not found, throws an error
            if (joystickGuid == Guid.Empty)
            {
                Console.WriteLine("No joystick/Gamepad found.");
                this.JoyStickError(this, new Exception("No joystick/Gamepad found."));
                return;
            }

            this.joystick = new Joystick(directInput, joystickGuid);
            JoystickState stato = new JoystickState();

            // specifico se relativo o assoluto
            joystick.Properties.AxisMode = DeviceAxisMode.Absolute;

            // effettuo un collegamento con il joystick
            joystick.Acquire();

            // qui faccio una acquisizione dello stato che memorizzo
            joystick.Poll();

            // effettuo una lettura dello stato
            joystick.GetCurrentState(ref stato);

            DispatcherTimer aTimer = new DispatcherTimer();
            aTimer.Tick += aTimer_Tick;
            aTimer.Interval = new TimeSpan(10);
            aTimer.IsEnabled = true;
        }

        void aTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                JoystickState stato = new JoystickState();
                // effettuo una lettura dello stato
                this.joystick.GetCurrentState(ref stato);

                //StringBuilder sb = new StringBuilder();
                for (int i = 0; i < 16; i++)
                {
                    if (this.etatBouton[i] != stato.Buttons[i])
                    {
                        this.etatBouton[i] = stato.Buttons[i];
                        if (this.BoutonEvent != null)
                        {
                            this.BoutonEvent.Invoke(this, new BoutonEventArgs(i, stato.Buttons[i]));
                        }
                    }
                }
            }
            catch
            (Exception exeception)
            {
                if (this.JoyStickError != null)
                {
                    this.JoyStickError.Invoke(this, exeception);
                }
            }
        }
    }
}


