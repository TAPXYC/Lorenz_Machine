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

namespace CW___SZ4042
{
    /// <summary>
    /// Логика взаимодействия для NavigationPage.xaml
    /// </summary>
    public partial class NavigationPage : Page
    {
        Frame NavigateFrame;

        public NavigationPage()
        {
            InitializeComponent();

            NavigateFrame = ((Application.Current.MainWindow as MainWindow).OwnedWindows[0] as NavigationWindow).NavigationFrame;
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            History history = new History();
            NavigateFrame.Navigate(history);
        }

        private void btnPrincipleOperation_Click(object sender, RoutedEventArgs e)
        {
            PrincipleOperation principleOperation = new PrincipleOperation();
            NavigateFrame.Navigate(principleOperation);
        }

        private void btnLearnPart1_Click(object sender, RoutedEventArgs e)
        {
            Learning L1 = new Learning();
            NavigateFrame.Navigate(L1);
        }
    }
}
