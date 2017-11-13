using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultatPourWeb
{
    class TrieResultat : IComparer<ResultatObj>
    {
        private bool mixte = false;
        public TrieResultat(bool Mixte)
        {
            mixte = Mixte;
        }

        public int Compare(ResultatObj xo, ResultatObj yo)
        {
            //if (x == null && y == null) { return 0; }
            //if (x != null && y == null) { return 1; }
            //if (x == null && y != null) { return -1; }
            //ResultatObj xo = null;
            //ResultatObj yo = null;
            //if (x is ResultatObj) { xo = x as ResultatObj; }
            //if (y is ResultatObj) { yo = y as ResultatObj; }
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
            //string lettreVagueX = xo.NoVague.Substring(xo.NoVague.Length - 1, 1);
            //string lettreVagueY = yo.NoVague.Substring(yo.NoVague.Length - 1, 1);
            //int chiffreVagueX = System.Convert.ToInt32(xo.NoVague.Substring(0, xo.NoVague.Length - 1));
            //int chiffreVagueY = System.Convert.ToInt32(yo.NoVague.Substring(0, yo.NoVague.Length - 1));
            ////.Groupe + z.Sexe + z.NoVague
            //int r1 = string.Compare(xo.Groupe, yo.Groupe);
            //if (r1 != 0)
            //{
            //    return r1;
            //}
            //int r2 = string.Compare(xo.Sexe, yo.Sexe);
            //if (r2 != 0)
            //{
            //    return r2;
            //}
            //if (chiffreVagueX > chiffreVagueY)
            //{
            //    return -1;
            //}
            //else if (chiffreVagueX > chiffreVagueY)
            //{
            //    return 1;
            //}
            //int r3 = string.Compare(lettreVagueX, lettreVagueY);
            //if (r3 != 0)
            //{
            //    return r3;
            //}
            ////.ThenBy(z => z.NoBloc).ThenBy(z => z.Rang).ThenBy(z => z.Point).ThenBy(z => z.NoCasque);
            //if (xo.NoBloc > yo.NoBloc)
            //{
            //    return -1;
            //}
            //else if (xo.NoBloc > yo.NoBloc)
            //{
            //    return 1;
            //}
            //if (xo.Rang > yo.Rang)
            //{
            //    return -1;
            //}
            //else if (xo.Rang > yo.Rang)
            //{
            //    return 1;
            //}
            //if (xo.Point > yo.Point)
            //{
            //    return -1;
            //}
            //else if (xo.Point > yo.Point)
            //{
            //    return 1;
            //}
            //if (xo.NoCasque > yo.NoCasque)
            //{
            //    return -1;
            //}
            //else if (xo.NoCasque > yo.NoCasque)
            //{
            //    return 1;
            //}
            //return 0;
        }
    }
}
