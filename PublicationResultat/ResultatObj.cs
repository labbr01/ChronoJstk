using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultatPourWeb
{
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
