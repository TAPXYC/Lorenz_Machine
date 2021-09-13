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
    /// Логика взаимодействия для History.xaml
    /// </summary>
    public partial class History : Page
    {
        List<string> Pictures = new List<string>()
        {
            { "pack://application:,,,/Lorenz_Mac.jpg"},
            { "pack://application:,,,/Lorenz_Mac1.jpg"},
            { "pack://application:,,,/Lorenz_Mac2.jpg"},
            {"pack://application:,,,/Able.jpg" },
            { "pack://application:,,,/Collosus.jpg" },
            { "pack://application:,,,/Collosus1.jpg" },
            { "pack://application:,,,/Collosus2.jpg" },
            { "pack://application:,,,/Collosus3.jpg" },
        };

        List<string> Name = new List<string>()
        {
            { "Машина 'Лоренц'"},
            { "Машина 'Лоренц'"},
            { "Машина 'Лоренц'"},
            {"Ablesetafel - параметры для настройки машины" },
            {"Суперкомпьютер Colossus"  },
            {"Суперкомпьютер Colossus"  },
            {"Суперкомпьютер Colossus"  },
            {"Суперкомпьютер Colossus"  },
        };


        int NumOfPicture = 0;
        DoubleAnimationUsingKeyFrames da;

        public History()
        {
            InitializeComponent();

            da = new DoubleAnimationUsingKeyFrames();
            da.Completed += Da_Completed;
            da.Duration = TimeSpan.FromSeconds(3);
            da.DecelerationRatio = 0.6;
            da.KeyFrames.Add(new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5))));
            da.AutoReverse = true;

            StartShow();
        }

        private void StartShow()
        {
            if(NumOfPicture == Pictures.Count)
            {
                NumOfPicture = 0;
            }

            Summary.Text = Name[NumOfPicture];

            Image Picture = new Image() { Margin = new Thickness(5), Opacity = 0};
            Picture.Source = new BitmapImage(new Uri(Pictures[NumOfPicture]));
            Collage.Children.Add(Picture);

            Picture.BeginAnimation(OpacityProperty, da);
        }

        private void Da_Completed(object sender, EventArgs e)
        {
            NumOfPicture++;
            Collage.Children.Clear();

            StartShow();
        }
    }
}
