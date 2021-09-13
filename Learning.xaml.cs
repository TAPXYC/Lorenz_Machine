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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CW___SZ4042
{
    /// <summary>
    /// Логика взаимодействия для Learning_part1.xaml
    /// </summary>
    public partial class Learning : Page
    {
        CryptoPanel CrPanel = (Application.Current.MainWindow as MainWindow).NavigationFrame.Content as CryptoPanel;

        public Learning()
        {
            InitializeComponent();

            VisualBrush visualBrush = new VisualBrush();
            visualBrush.Visual = CrPanel;
            Example.Fill = visualBrush;
        }
        
    }
}
