using ChronoJstk.ChronistickInput;
using ChronoStick_Input.Model;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ChronoJstk.Input
{
    public class DirectChronostickInputMgr : IChronistickInputMgr
    {
        public JoyStickManager jsm;
        public int tentative = 0;

        public event DelDepartEventHandler DepartEventHandler;
        public event DelTourEventHandler TourEventHandler;
        public event EventHandler ErreurConnexion;

        private int nbPatineur = 5;
        private int delaisTour = 5;
        private int delaisPing = 30;
        private int delaisDepart = 5;

        private Dictionary<int, DateTime> listeSignaux;

        public DirectChronostickInputMgr()
        {
            this.listeSignaux = new Dictionary<int, DateTime>();
            this.jsm = new JoyStickManager();
            this.jsm.BoutonEvent += Jsm_BoutonEvent;
            this.jsm.JoyStickError += Jsm_JoyStickError;
            this.jsm.CaptureJoyStick();
            
        }

        private void Jsm_JoyStickError(object sender, Exception e)
        {
            this.tentative += 1;
            if (this.jsm != null)
            {
                this.jsm.BoutonEvent -= this.Jsm_BoutonEvent;
                this.jsm.JoyStickError -= this.Jsm_JoyStickError;
                this.jsm = null;
            }
            if (this.tentative < 10)
            {
                this.jsm = new JoyStickManager();
                this.jsm.BoutonEvent += Jsm_BoutonEvent;
                this.jsm.JoyStickError += Jsm_JoyStickError;
                this.jsm.CaptureJoyStick();
            }
            else
            {
                if (MessageBox.Show("Reconnecter au joystick?", "Erreur de joystick", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    this.jsm = new JoyStickManager();
                    this.jsm.BoutonEvent += Jsm_BoutonEvent;
                    this.jsm.JoyStickError += Jsm_JoyStickError;
                    this.jsm.CaptureJoyStick();
                }
                else {
                    if (this.ErreurConnexion != null) { 
                    this.ErreurConnexion(this, new EventArgs());
                    }
                }
            }
        }

        private void Jsm_BoutonEvent(object sender, BoutonEventArgs e)
        {
            bool doublon = false;
            if (this.tentative != 0)
            {
                this.tentative = 0;
            }

            if (!e.Enfonce)
            {
                return;
            }
            if (!this.listeSignaux.ContainsKey(e.NoBouton))
            {
                this.listeSignaux.Add(e.NoBouton, e.Heure);
            }
            else
            {
                TimeSpan ts = e.Heure - this.listeSignaux[e.NoBouton];
                if (e.NoBouton == 0)
                {
                    if (ts.TotalSeconds > this.delaisDepart)
                    {
                        this.listeSignaux[e.NoBouton] = e.Heure;
                    }
                    else
                    {
                        doublon = true;
                    }
                }
                else
                {
                    if (ts.TotalSeconds > this.delaisTour)
                    {
                        this.listeSignaux[e.NoBouton] = e.Heure;
                    }
                    else {
                        doublon = true;
                    }
                }

            }
            switch (e.NoBouton)
            {
                case 0:
                    {
                        for (int i = 1;  i<= this.nbPatineur; i++) { 
                         DepartEventHandler?.Invoke(this, new ChronstickInputEventArgs() { Evenement = new ChronoStick_Affaires.Evenement() { Action = ChronoStick_Affaires.TypeEvenement.Depart, Doublon = doublon, Heure = e.Heure, NoPatineur = i, Origine = ChronoStick_Affaires.OrigineEvenement.USB } });
                        }
                        break;
                    }
                   default : 
                    {
                        TourEventHandler?.Invoke(this, new ChronstickInputEventArgs() { Evenement = new ChronoStick_Affaires.Evenement() { Action = ChronoStick_Affaires.TypeEvenement.Tour, Doublon = doublon, Heure = e.Heure, NoPatineur = e.NoBouton, Origine = ChronoStick_Affaires.OrigineEvenement.USB } });
                        break;
                    }
            }
        }

        public void ConfigurerDelaisDepart(int delais)
        {
            this.delaisDepart = delais;
        }

        public void ConfigurerDelaisPing(int delais)
        {
            throw new NotImplementedException();
        }

        public void ConfigurerDelaisTour(int delais)
        {
            this.delaisTour = delais;
        }

        public void ConfigurerNbPatineur(int nbPat)
        {
            this.nbPatineur = nbPat;
        }

        public void Terminer(int delais)
        {
            this.jsm.BoutonEvent -= this.Jsm_BoutonEvent;
            this.jsm.JoyStickError -= this.Jsm_JoyStickError;
            this.jsm = null;
        }

        public void Reconnecter()
        {
            if (this.jsm != null)
            {
                this.jsm.BoutonEvent -= this.Jsm_BoutonEvent;
                this.jsm.JoyStickError -= this.Jsm_JoyStickError;
                this.jsm = null;
            }
                this.jsm = new JoyStickManager();
                this.jsm.BoutonEvent += Jsm_BoutonEvent;
                this.jsm.JoyStickError += Jsm_JoyStickError;
                this.jsm.CaptureJoyStick();
        }
    }
}
