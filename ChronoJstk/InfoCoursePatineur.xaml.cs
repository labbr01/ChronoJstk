using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChronoJstk
{
    /// <summary>
    /// Logique d'interaction pour InfoCoursePatineur.xaml
    /// </summary>
    public partial class InfoCoursePatineur : UserControl
    {
        public InfoCoursePatineur()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PatineurCourse pc = DataContext as PatineurCourse;
            if (pc != null)
            {                
                pc.Tour();
            }            
        }

        private void btnFinZero_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Confirmer le non départ ou le non fin du patineur", "Avertissement", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                PatineurCourse pc = DataContext as PatineurCourse;
                if (pc != null)
                {
                    pc.NonFinDepart();
                }
            }
        }
    }
}
