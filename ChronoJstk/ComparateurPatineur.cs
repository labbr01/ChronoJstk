using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChronoJstk
{
    class ComparateurPatineur : IEqualityComparer<PatineurCourse>
    {
        bool IEqualityComparer<PatineurCourse>.Equals(PatineurCourse x, PatineurCourse y)
        {
            if ((x == null && y != null) || (x != null && y == null)) {
                return false;
            }
            if (x.Bloc == y.Bloc && x.Vague == y.Vague && x.Serie == y.Serie)
            {
                return true;
            }
            return false;
        }

        int IEqualityComparer<PatineurCourse>.GetHashCode(PatineurCourse obj)
        {
            if (obj == null)
            {
                return 0;
            }

            string zz = string.Format("{0},{1},{2}",obj.Bloc, obj.Serie, obj.Vague);
            return zz.GetHashCode();            
        }
    }
}
