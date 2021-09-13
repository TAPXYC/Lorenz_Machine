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
using System.Windows.Shapes;

namespace CW___SZ4042
{
    /// <summary>
    /// Логика взаимодействия для NavigationWindow.xaml
    /// </summary>
    public partial class NavigationWindow : Window
    {
        public NavigationWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationPage NP = new NavigationPage();
            NavigationFrame.Navigate(NP);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if(NavigationFrame.CanGoBack)
            {
                NavigationFrame.GoBack();
            }
        }

        private void btnForward_Click_1(object sender, RoutedEventArgs e)
        {
            if (NavigationFrame.CanGoForward)
            {
                NavigationFrame.GoForward();
            }
        }
    }
}
