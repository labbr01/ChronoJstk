using Newtonsoft.Json.Linq;
using PVModele;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultatPourWeb
{
    public class PublicationResultat
    {
        private string extFich;
        private int noCompeti;
        private bool mixte;
        private string pathTravail;
        private string ftpUser;
        private string ftpPassword;
        private string ftpSite;
        // ftp ftpCopie = new ftp("ftp://ftp.mmetara.com", "cpvqadmin", "10cpvq01");
        public PublicationResultat(int noCompeti, bool mixte, string extFich, string pathTravail, string ftpSite, string ftpUser, string ftpPassword)
        {
            this.noCompeti = noCompeti;
            this.mixte = mixte;
            this.extFich = extFich;
            if (string.IsNullOrEmpty(this.extFich))
            {
                this.extFich = "js";
            }
            this.pathTravail = pathTravail;
            this.ftpSite = ftpSite;
            this.ftpUser = ftpUser;
            this.ftpPassword = ftpPassword;
        }

        public JObject InfoCompeVagues(DBPatinVitesse db)
        {            
                var queryCompe =
                    from compe in db.Competition
                    where compe.NoCompetition == noCompeti
                    select compe;
                ;

                JObject programme = new JObject();
                programme.Add("Nom", queryCompe.First().Commentaire);
                JArray ja = new JArray();
                programme.Add("Groupes", ja);

                var pc = from progcourse in db.ProgCourses
                         where progcourse.NoCompetition == noCompeti
                         select progcourse.Groupe;

                var grppreced = string.Empty;
                var vgpreced = string.Empty;
                JArray vaguesGroupes = new JArray();
                JArray patineurVague = new JArray();
                JObject vagueJson = null;

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
                              && patcmp.NoCompetition == noCompeti
                              && progcrs.NoCompetition == noCompeti
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

            //var laTotale1 = laTotal.OrderBy(z => z, comparer ).ThenBy(z => z.NoBloc).ThenBy(z => z.Rang).ThenBy(z => z.Point).ThenBy(z => z.NoCasque);
            //var laTotale1 = laTotal.OrderBy(z => z, comparer);
            //var laTotale1 = laTotal.OrderBy(z => z.Groupe).ThenBy(z => z.Sexe).ThenBy(z => z.ChiffreVague).ThenBy(z => z.LettreVague).ThenBy(z => z.NoBloc).ThenBy(z => z.Rang).ThenBy(z => z.Point).ThenBy(z => z.NoCasque);
            //var laTotale1 = laTotal.OrderBy(z => z.Groupe + z.Sexe + z.NoVague.PadLeft(6,'0') + z.NoBloc.ToString().PadLeft(3,'0') + z.Rang.ToString().PadLeft(4,'0')).ThenBy(z => z.Point).ThenBy(z => z.NoCasque);
            bool m = this.mixte;
                TrieResultat comparer = new TrieResultat(m);
                var laTotale1 = laTotal.ToList().OrderBy(z => z, comparer).ToList();
                nbp = laTotale1.Count();
                var laTotale2 = laTotale1.ToList();
                nbp = laTotale2.Count();
                //foreach (var ab in laTotale2)
                //{
                //    Console.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", ab.Groupe, ab.NoVague, ab.Nom, ab.Rang, ab.Temps, ab.Point, ab.Club, ab.LongueurEpreuve, ab.Nom, ab.Epreuve, ab.Code));
                //}

                var noGroupe = 0;
                Dictionary<int, ResultatObj> dictRes = new Dictionary<int, ResultatObj>();
                foreach (var ab in laTotale2)
                {
                    string nomgrp = ab.Groupe + " " + ab.Sexe.ToUpper();
                    if (m)
                    { nomgrp = ab.Groupe; }
                    if (grppreced != nomgrp)
                    {
                        // Si le groupe change, la vague change
                        if (vagueJson != null)
                        {
                            vaguesGroupes.Add(vagueJson);
                        }
                        vagueChange(out vgpreced, out patineurVague, out vagueJson, ab);
                        if (!string.IsNullOrEmpty(grppreced))
                        {
                            noGroupe += 1;
                            //string groupeData = vaguesGroupes.ToString();
                            CreerFichierResultatGroupe(noGroupe, vaguesGroupes, grppreced, (JArray)programme["Groupes"], dictRes);
                            dictRes = new Dictionary<int, ResultatObj>();
                        }
                        vaguesGroupes = new JArray();
                        grppreced = nomgrp;
                        //vgpreced = ab.NoVague;
                    }
                    else if (vgpreced != ab.NoVague)
                    {
                        if (vagueJson != null)
                        {
                            vaguesGroupes.Add(vagueJson);
                        }
                        vagueChange(out vgpreced, out patineurVague, out vagueJson, ab);
                    }
                    if (dictRes.ContainsKey(ab.NoPatineur))
                    {
                        dictRes[ab.NoPatineur].Point += ab.Point;
                    }
                    else
                    {
                        dictRes.Add(ab.NoPatineur, new ResultatObj()
                        {
                            NoPatineur = ab.NoPatineur,
                            Nom = ab.Nom,
                            Club = ab.Club,
                            Point = ab.Point,
                            Code = ""
                        });
                    }

                    JObject patineurJson = new JObject();
                    patineurJson.Add("Casque", ab.NoCasque.ToString());
                    patineurJson.Add("Patineurs", ab.Nom);
                    patineurJson.Add("Club", ab.Club);
                    patineurJson.Add("Rang", ab.Rang);
                    patineurJson.Add("Temps", ab.Temps);
                    patineurJson.Add("Commentaire", ab.Code);
                    patineurJson.Add("Point", ab.Point);
                    patineurJson.Add("Date", "");
                    patineurVague.Add(patineurJson);
                    //Console.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", ab.Groupe, ab.NoVague, ab.Nom, ab.Rang, ab.Temps, ab.Point, ab.Club, ab.LongueurEpreuve, ab.Nom, ab.Etape));
                }

                // On termine avec le dernier groupe
                noGroupe += 1;
                if (vagueJson != null)
                {
                    vaguesGroupes.Add(vagueJson);
                }
                if (!string.IsNullOrEmpty(grppreced))
                {
                    //string groupeData = vaguesGroupes.ToString();
                    CreerFichierResultatGroupe(noGroupe, vaguesGroupes, grppreced, (JArray)programme["Groupes"], dictRes);
                }

                // Créer le fichier de programme
                string programmeStr = programme.ToString();
                programmeStr = programmeStr.Replace(Environment.NewLine, string.Empty);
                //string extFich = "js";
                //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Fmt"]) && ConfigurationManager.AppSettings["Fmt"] == "JSON")
                //{
                //    extFich = "json";
                //}
                string nomficheprg = string.Format("programme.{0}", extFich);
              
                string newtext = programmeStr;
                if (extFich == "js")
                {
                    newtext = string.Format("var a = '{0}';", programmeStr);
                }

            string pathResultat = Path.Combine(this.pathTravail, "ResultatsTravail");
            if (!Directory.Exists(pathResultat)) {
                Directory.CreateDirectory(pathResultat);
            }
            pathResultat = Path.Combine(pathResultat, "programme.json");
            string ResultatsPrecedents = Path.Combine(this.pathTravail, "ResultatsPrecedents");
            if (!Directory.Exists(ResultatsPrecedents))
            {
                Directory.CreateDirectory(ResultatsPrecedents);
            }
            ResultatsPrecedents = Path.Combine(ResultatsPrecedents, "programme.json");
            string ResultatsFTP = Path.Combine(this.pathTravail, "ResultatsFTP");
            if (!Directory.Exists(ResultatsFTP))
            {
                Directory.CreateDirectory(ResultatsFTP);
            }
            ResultatsFTP = Path.Combine(ResultatsFTP, "programme.json");


            using (StreamWriter sr = new StreamWriter(pathResultat))
                {
                    sr.Write(newtext);
                }

                if (!System.IO.File.Exists(ResultatsPrecedents))
                {
                    // première copie du résultat
                    System.IO.File.Copy(pathResultat, ResultatsPrecedents);
                    System.IO.File.Copy(pathResultat, ResultatsFTP);
                }
                else
                {
                    string contenuGit = System.IO.File.ReadAllText(ResultatsPrecedents);
                    if (contenuGit != newtext)
                    {
                        System.IO.File.Delete(ResultatsPrecedents);
                        System.IO.File.Copy(pathResultat, ResultatsPrecedents);
                        System.IO.File.Copy(pathResultat, ResultatsFTP);
                    }
                }
            //List<string> ls = new List<string>();
            return programme;

        }

        private void CreerFichierResultatGroupe(int noGroupe, JArray groupeData, string nomGroupe, JArray groupes, Dictionary<int, ResultatObj> dictRes)
        {
            //string extFich = "js";
            //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Fmt"]) && ConfigurationManager.AppSettings["Fmt"] == "JSON")
            //{
            //    extFich = "json";
            //}

            JObject jogrp = new JObject();
            jogrp.Add("Groupe", nomGroupe);
            //string gitLink = ConfigurationManager.AppSettings["gitLink"];

            jogrp.Add("src", string.Format("data/GR{0}.{1}", noGroupe, extFich));

            groupes.Add(jogrp);
            // Ajouter la compilation des temps
            List<ResultatObj> ls = dictRes.Values.OrderByDescending(z => z.Point).ToList();
            var i = 0;
            ls.ForEach(z => z.Rang = ++i);

            JArray patineurCompile = new JArray();
            foreach (ResultatObj ab in ls)
            {
                JObject patineurJson = new JObject();
                patineurJson.Add("Casque", ab.NoCasque.ToString());
                patineurJson.Add("Patineurs", ab.Nom);
                patineurJson.Add("Club", ab.Club);
                patineurJson.Add("Rang", ab.Rang);
                patineurJson.Add("Temps", ab.Temps);
                patineurJson.Add("Commentaire", ab.Code);
                patineurJson.Add("Point", ab.Point);
                patineurJson.Add("Date", "");
                patineurCompile.Add(patineurJson);
            }

            JObject vagueJson = new JObject();
            vagueJson.Add("vague", "Sommaire");
            vagueJson.Add("distance", nomGroupe);
            vagueJson.Add("etape", "competition");
            vagueJson.Add("Patineurs", patineurCompile);
            groupeData.Add(vagueJson);

            string groupeDatastr = groupeData.ToString();
            groupeDatastr = groupeDatastr.Replace(Environment.NewLine, string.Empty);
            string nomfichx = string.Format("GR{0}.{1}", noGroupe, extFich);
            string pathResultat = Path.Combine(this.pathTravail, "ResultatsTravail", nomfichx);
            string ResultatsPrecedents = Path.Combine(this.pathTravail, "ResultatsPrecedents", nomfichx);
            string ResultatsFTP = Path.Combine(this.pathTravail, "ResultatsFTP", nomfichx);

            string newtxt = string.Empty;
            if (extFich == "json")
            {
                newtxt = groupeDatastr;
            }
            else
            {
                newtxt = string.Format("var a = '{0}';", groupeDatastr);
            }

            using (StreamWriter sr = new StreamWriter(pathResultat))
            {
                sr.Write(newtxt);
            }

            if (!System.IO.File.Exists(ResultatsPrecedents))
            {
                System.IO.File.Copy(pathResultat, ResultatsPrecedents);
                System.IO.File.Copy(pathResultat, ResultatsFTP);
            }
            else
            {
                string contenuGit = System.IO.File.ReadAllText(ResultatsPrecedents);
                if (contenuGit != newtxt)
                {
                    System.IO.File.Delete(ResultatsPrecedents);
                    System.IO.File.Copy(pathResultat, ResultatsPrecedents);
                    System.IO.File.Copy(pathResultat, ResultatsFTP, true);
                }
            }
        }

        private void vagueChange(out string vgpreced, out JArray patineurVague, out JObject vagueJson, ResultatObj ab)
        {
            patineurVague = new JArray();
            vagueJson = new JObject();
            vagueJson.Add("vague", ab.NoVague);
            vagueJson.Add("distance", ab.LongueurEpreuve.ToString() + "m");
            vagueJson.Add("etape", ab.Epreuve);
            vagueJson.Add("Patineurs", patineurVague);
            vgpreced = ab.NoVague;
        }

        public List<string> CopierSiteFTP(JObject programme)
        {
            //            serveur: ftp.mmetara.com
            //id: cpvqadmin
            //pwd: 10cpvq01
            List<string> resultat = new List<string>();

            ftp ftpCopie = new ftp(this.ftpSite, this.ftpUser, this.ftpPassword);
            string PathResultatsFTP = Path.Combine(this.pathTravail, "ResultatsFTP");
            foreach (string fichier in Directory.GetFiles(PathResultatsFTP))
            {
                FileInfo fi = new FileInfo(fichier);
                string nbcp = string.Format("data/{0}", fi.Name);

                foreach (JObject gr in (JArray)programme["Groupes"])
                {
                    if ((string)gr["src"] == nbcp)
                    {
                        resultat.Add((string)gr["Groupe"]);
                        break;
                    }
                }

                //Console.WriteLine("Téléversement du fichier {0} à {1}", fichier, nbcp);
                    ftpCopie.upload(nbcp, fichier);
                File.Delete(fichier);
            }
            return resultat;
        }

        /// <summary>
        /// Contains approximate string matching
        /// </summary>
        static class LevenshteinDistance
        {
            /// <summary>
            /// Compute the distance between two strings.
            /// </summary>
            public static int Compute(string s, string t)
            {
                int n = s.Length;
                int m = t.Length;
                int[,] d = new int[n + 1, m + 1];
                // Step 1
                if (n == 0)
                {
                    return m;
                }

                if (m == 0)
                {
                    return n;
                }

                // Step 2
                for (int i = 0; i <= n; d[i, 0] = i++)
                {
                }

                for (int j = 0; j <= m; d[0, j] = j++)
                {
                }

                // Step 3
                for (int i = 1; i <= n; i++)
                {
                    //Step 4
                    for (int j = 1; j <= m; j++)
                    {
                        // Step 5
                        int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                        // Step 6
                        d[i, j] = Math.Min(
                            Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                            d[i - 1, j - 1] + cost);
                    }
                }
                // Step 7
                return d[n, m];
            }
        }
    }
}
