﻿using PVModele;
using PVModele.Tables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace ChronoJstk
{
    class ProgrammeCourseMgr
    {
        public void Obtenir(string nomFichier, out int noCompe, out string NomCompe)
        {
            noCompe = -1;
            NomCompe = string.Empty;
            int noCompeInterne = -1;
            using (DBPatinVitesse db = new DBPatinVitesse(nomFichier))
            {
                ChoisirCompetition dialog = new ChoisirCompetition();             
                //dialog.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                dialog.ListCompe = new System.Collections.ObjectModel.ObservableCollection<string>(db.Competition.Select(z => z.Lieu).ToList());
                    dialog.ShowDialog();
                if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
                {
                    string nom;
                    if (dialog.CompeSele == null)
                    {
                        nom = dialog.ListCompe.First();
                    }
                    else { 
                        nom = dialog.CompeSele;
                    }
                    NomCompe = nom;
                    noCompe = db.Competition.SingleOrDefault(z => z.Lieu == nom).NoCompetition;
                    noCompeInterne = noCompe;
                }
                else {
                    ParamCommuns.Instance.Programmes = null;
                    ParamCommuns.Instance.DescVagues = null;
                    return;
                }

                DetailCompe(db, noCompeInterne);
            }
        }

        public static void RafraichirCompe()
        {
            string nomFichier =  ParamCommuns.Instance.NomFichierPat;
            string nomCompe = ParamCommuns.Instance.NomCompetition;
            int noCompt = ParamCommuns.Instance.NoCompetition;
            string connectString = string.Format(@"Provider = Microsoft.Jet.OLEDB.4.0; Data Source = {0}; User Id = admin; Password =; ", nomFichier);
            try { 
            using (DBPatinVitesse db = new DBPatinVitesse(connectString))
            {
                DetailCompe(db, noCompt);
            }
            }
            catch (Exception e)
            {
                MessageBox.Show("Erreur de rafraichissement");
            }
        }
        private static System.Timers.Timer aTimer;
        
        public static void MiseAJourCompe()
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(5000000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        public static void InterrompreMajCompe()
        {
            if (aTimer == null)
            {
                return;
            }
            aTimer.Stop();
        }

        public static void ReprendreMajCompe()
        {
            if (aTimer == null)
            {
                return;
            }
            aTimer.Start();
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                              e.SignalTime);
            RafraichirCompe();
        }



        public static void DetailCompe(DBPatinVitesse db, int noCompeInterne)
    {
        var programmes = new ObservableCollection<ProgrammeCourse>();
        var descVagues = new Dictionary<string, List<PatineurVague>>();

        // Sélection de tous les temps des patineurs de la compétition
        var laTotal = from patvag in db.PatVagues
                      join patcmp in db.PatineurCompe on patvag.NoPatCompe equals patcmp.NoPatCompe
                      join patineur in db.Patineur on patcmp.NoPatineur equals patineur.NoPatineur
                      join club in db.Club on patineur.NoClub equals club.NoClub
                      join vag in db.Vagues on patvag.CleTVagues equals vag.CleTVagues
                      join progcrs in db.ProgCourses on vag.CleDistancesCompe equals progcrs.CleDistancesCompe
                      join diststd in db.DistanceStandards on progcrs.NoDistance equals diststd.NoDistance
                      join cmnt in db.Commentaire on patvag.Juge equals cmnt.Code
                      where patvag.NoPatCompe == patcmp.NoPatCompe
                      && patcmp.NoPatineur == patineur.NoPatineur
                      && patineur.NoClub == club.NoClub
                      && patvag.CleTVagues == vag.CleTVagues
                      && vag.CleDistancesCompe == progcrs.CleDistancesCompe
                      && progcrs.NoDistance == diststd.NoDistance
                      && patcmp.NoCompetition == noCompeInterne
                      && progcrs.NoCompetition == noCompeInterne
                      select new ResultatObj()
                      {
                          NoPatineur = patineur.NoPatineur,
                          Nom = patineur.Nom + "," + patineur.Prenom,
                          Club = club.NomClub,
                          NoCasque = patvag.NoCasque,
                          Temps = patvag.Temps,
                          Point = patvag.Point,
                          Rang = patvag.Rang,
                          Code = cmnt.CodeAction.Replace("NIL", string.Empty),
                          NoVague = vag.NoVague,
                          Epreuve = vag.Qual_ou_Fin,
                          Groupe = patcmp.Groupe,
                          LongueurEpreuve = diststd.LongueurEpreuve,
                          NoBloc = progcrs.NoBloc,
                          Sexe = patineur.Sexe
                      };

        var nbp = laTotal.Count();

        bool m = bool.Parse(ConfigurationManager.AppSettings["Mixte"]);
        TrieResultat comparer = new TrieResultat(m);
        var laTotale1 = laTotal.ToList().OrderBy(z => z, comparer).ToList();
        nbp = laTotale1.Count();
        var laTotale2 = laTotale1.ToList();
        nbp = laTotale2.Count();

        var b = laTotale2.Select(z => new { z.Epreuve, z.Groupe, z.LongueurEpreuve, z.NoBloc, z.ChiffreVague }).Distinct().ToList();
        foreach (var z in b.OrderBy(z => z.NoBloc).ThenBy(z => z.ChiffreVague))
        {
            ProgrammeCourse pc = new ProgrammeCourse();
            //pc.Type == z.Epreuve;
            pc.Bloc = z.NoBloc;
            pc.Epreuve = z.LongueurEpreuve.ToString();
            pc.Trace = 100;
            pc.NbTour = z.LongueurEpreuve / pc.Trace;
            var x = laTotale2.Where(k => k.NoBloc == z.NoBloc && k.Epreuve == z.Epreuve && k.Groupe == z.Groupe && k.LongueurEpreuve == z.LongueurEpreuve);
            int nb = x.Count();
            string min = x.Min(k => k.LettreVague);
            string max = x.Max(k => k.LettreVague);
            ResultatObj r = x.First();
            string chiffreSerie = r.NoVague.Replace(r.LettreVague, string.Empty);
            int serie = System.Convert.ToInt32(chiffreSerie);
            pc.Serie = serie;
            pc.LVagues = x.Select(pz => pz.LettreVague).Distinct().ToList();
            pc.De = min;
            pc.A = max;
            pc.TypeCourse = z.Epreuve;
            programmes.Add(pc);

            foreach (var v in x)
            {
                List<PatineurVague> patVags = null;
                if (descVagues.ContainsKey(v.ChiffreVague))
                {
                    patVags = descVagues[v.ChiffreVague];
                }
                else
                {
                    patVags = new List<PatineurVague>();
                    descVagues.Add(v.ChiffreVague, patVags);
                }
                PatineurVague pv = new PatineurVague();
                pv.Epreuve = v.LongueurEpreuve;
                pv.Groupe = v.Groupe;
                pv.Casque = v.NoCasque;
                pv.Patineurs = v.Nom;
                pv.Club = v.Club;
                pv.Rang = v.Rang;
                pv.Temps = v.Temps.ToString();
                pv.Points = v.Point;
                pv.Commentaire = string.Empty;
                pv.Vague = v.LettreVague;
                //pv.Date;                                                                             
                patVags.Add(pv);
            }           
        }

            //pc.Serie = z.ChiffreVague;
            if (ParamCommuns.Instance.Programmes == null)
            {
                ParamCommuns.Instance.Programmes = programmes;
                ParamCommuns.Instance.DescVagues = descVagues;
                MiseAJourCompe();
            }
            else
            {
                // La compétition est déjà définie, on ajoute seulement les courses qui sont différentes
                List<ProgrammeCourse> actuel = ParamCommuns.Instance.Programmes.ToList();
                List<ProgrammeCourse> nouveaux = programmes.ToList();
                foreach (ProgrammeCourse pc in actuel)
                {
                    ProgrammeCourse nouveau = nouveaux.SingleOrDefault(z => z.Bloc == pc.Bloc && z.Serie == pc.Serie);
                    if (nouveau != null &&
                        nouveau.Epreuve == pc.Epreuve &&
                        nouveau.NbTour == pc.NbTour &&
                        nouveau.Trace == pc.Trace &&
                        nouveau.TypeCourse == pc.TypeCourse &&
                        string.Join(",", nouveau.LVagues) == string.Join(",", pc.LVagues))
                    {
                        // pareil en tout point, pas un nouveau!
                        nouveaux.Remove(nouveau);
                    }
                    else
                    {
                        if (nouveau == null)
                        {
                            // On a un ancien qui n'existe plus (on l'elève du programme)                          
                            ParamCommuns.Instance.Programmes.Remove(pc);
                        }
                        else
                        {
                            // on a un ancien qui est différent d'un nouveau
                            ParamCommuns.Instance.Programmes.Add(nouveau);
                            ParamCommuns.Instance.Programmes.Remove(pc);
                        }
                    }
               }

                    // On ajoute les vraix nouveaux
                    nouveaux.ForEach(z => ParamCommuns.Instance.Programmes.Add(z));

                    Dictionary<string, List<PatineurVague>> descVaguesActuel = new Dictionary<string, List<PatineurVague>>();
                    foreach (KeyValuePair<string, List<PatineurVague>> kvp in ParamCommuns.Instance.DescVagues)
                    {
                        descVaguesActuel.Add(kvp.Key, kvp.Value);
                    }
                    Dictionary<string, List<PatineurVague>> descVaguesNouveau = new Dictionary<string, List<PatineurVague>>();
                    foreach (KeyValuePair<string, List<PatineurVague>> kvp in descVagues)
                    {
                        descVaguesNouveau.Add(kvp.Key, kvp.Value);
                    }
                    foreach (string cle in descVaguesActuel.Keys)
                    {
                        List<PatineurVague> pvactuels = descVaguesActuel[cle];

                        List<PatineurVague> pvnouveaux = null;
                        if (descVaguesNouveau.ContainsKey(cle))
                        {
                            pvnouveaux = descVaguesNouveau[cle];
                            descVaguesNouveau.Remove(cle);
                        }

                        bool tousPareil = true;
                        if (pvactuels.Count() == pvnouveaux.Count())
                        {
                            int i = 0;
                            foreach (PatineurVague pvactuel in pvactuels)
                            {
                                if (i < pvnouveaux.Count())
                                {
                                    PatineurVague pvNouveau = pvnouveaux[i];
                                    if (pvactuel.Patineurs == pvNouveau.Patineurs && pvactuel.Club == pvactuel.Club)
                                    {
                                        pvactuel.Casque = pvNouveau.Casque;
                                        pvactuel.Commentaire = pvNouveau.Commentaire;
                                        pvactuel.Date = pvNouveau.Date;
                                        pvactuel.Epreuve = pvNouveau.Epreuve;
                                        pvactuel.Groupe = pvNouveau.Groupe;
                                        pvactuel.Points = pvNouveau.Points;
                                        pvactuel.Rang = pvNouveau.Rang;
                                        pvactuel.Temps = pvNouveau.Temps;
                                        pvactuel.Vague = pvNouveau.Vague;
                                    }
                                    else
                                    {
                                        tousPareil = false;
                                        break;
                                    }
                                }


                                i += 1;
                            }
                        }
                        else
                        {
                            tousPareil = false;
                        }

                        if (!tousPareil)
                        {
                            // Il y a une différence sur les patineurs
                            // il faut mettre à jour
                            ParamCommuns.Instance.DescVagues[cle].Clear();
                            pvnouveaux.ForEach(z => ParamCommuns.Instance.DescVagues[cle].Add(z));
                        }
                    }

                    foreach (string cle in descVaguesNouveau.Keys)
                    {
                        ParamCommuns.Instance.DescVagues.Add(cle, descVaguesNouveau[cle]);
                    }
                }
            
        }

    }
    class TrieResultat : IComparer<ResultatObj>
    {
        private bool mixte = false;
        public TrieResultat(bool Mixte)
        {
            mixte = Mixte;
        }

        public int Compare(ResultatObj xo, ResultatObj yo)
        {
            if (xo == null && yo == null) { return 0; }
            if (xo != null && yo == null) { return 1; }
            if (xo == null && yo != null) { return -1; }


            string xc = string.Empty;
            string yc = string.Empty;
            if (mixte)
            {
                xc = xo.Groupe.PadLeft(120, '0') + xo.NoVague.PadLeft(6, '0') + xo.NoBloc.ToString().PadLeft(3, '0') + xo.Rang.ToString().PadLeft(4, '0') + xo.Point.ToString().PadLeft(8, '0') + xo.NoPatineur.ToString().PadLeft(3, '0');
                yc = yo.Groupe.PadLeft(120, '0') + yo.NoVague.PadLeft(6, '0') + yo.NoBloc.ToString().PadLeft(3, '0') + yo.Rang.ToString().PadLeft(4, '0') + yo.Point.ToString().PadLeft(8, '0') + yo.NoPatineur.ToString().PadLeft(3, '0');
            }
            else
            {
                xc = xo.Groupe.PadLeft(120, '0') + xo.Sexe + xo.NoVague.PadLeft(6, '0') + xo.NoBloc.ToString().PadLeft(3, '0') + xo.Rang.ToString().PadLeft(4, '0') + xo.Point.ToString().PadLeft(8, '0') + xo.NoPatineur.ToString().PadLeft(3, '0');
                yc = yo.Groupe.PadLeft(120, '0') + yo.Sexe + yo.NoVague.PadLeft(6, '0') + yo.NoBloc.ToString().PadLeft(3, '0') + yo.Rang.ToString().PadLeft(4, '0') + yo.Point.ToString().PadLeft(8, '0') + yo.NoPatineur.ToString().PadLeft(3, '0');
            }

            return string.Compare(xc, yc);
        }
    }

    public class ResultatObj
    {
        public int NoPatineur { get; set; }
        public string Nom { get; set; }
        public string Club { get; set; }
        public int NoCasque { get; set; }
        public double Temps { get; set; }
        public int Point { get; set; }
        public int Rang { get; set; }
        public string Code { get; set; }
        public string NoVague { get; set; }
        public string ChiffreVague
        {
            get
            {
                string testeur = this.NoVague.Substring(0, this.NoVague.Length - 1);
                return testeur.PadLeft(4, '0');
            }
        }
        public string LettreVague
        {
            get
            {
                string testeur = this.NoVague.Substring(this.NoVague.Length - 1);
                return testeur;

            }
        }
        public string Epreuve { get; set; }
        public string Groupe { get; set; }
        public int LongueurEpreuve { get; set; }
        public int NoBloc { get; set; }
        public string Sexe { get; set; }

    }
}
