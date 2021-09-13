using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    class CycleScroll
    {
        private List<bool> LB;
        private Border border = new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(2) };
        private StackPanel SP = new StackPanel() { Background = Brushes.Transparent, Margin = new Thickness(0,-65,0,0) };

        public CycleScroll(List<bool> lb, Grid container, int Coll) //анимация вращения дисков
        {
            LB = lb;

            Canvas.SetZIndex(SP, 0);
            SP.MouseWheel += SP_MouseWheel;
            Grid.SetColumn(border, Coll);
            Grid.SetRow(border, 1);

            for (int i = 0; i < 9; i++)
            {
                CheckBox ChkBtn = new CheckBox()
                {
                    //всё, что нам нужно - это контент
                    Content = i,
                    Width = 100,
                    Height = 80,
                    IsChecked = LB[i]
                };

                ChkBtn.Click += TurnSwitch;

                InitializeCheckBoxTransform(ChkBtn);

                SP.Children.Add(ChkBtn);
            }

            border.Child = SP;
            container.Children.Add(border);
        }

        public void UpdateBoolList(List<bool> listBool)/*привязка графического изображения переключателя к соответствующему 
            значению в списке*/
        {
            LB = listBool;
            foreach (CheckBox cb in SP.Children)
            {
                cb.IsChecked = LB[(int)cb.Content];
            }
        }

        private int UpdateUpperNumber(int CurrentNum) //обновление верхнего номера при прокрутке
        {
            return CurrentNum == 0 ? LB.Count - 1 : CurrentNum - 1;
        }

        private int UpdateDownNumber(int CurrentNum) //обновление нижнего номера при прокрутке
        {
            return CurrentNum == LB.Count - 1 ? 0 : CurrentNum + 1;
        }


        private void InitializeCheckBoxTransform(CheckBox cb)
        {
            TranslateTransform translate = new TranslateTransform();
            translate.Y = 0;

            TransformGroup TransformGroup = new TransformGroup();
            TransformGroup.Children.Add(translate);

            cb.RenderTransform = TransformGroup;
        }

        private void TranslateToDown(CheckBox cb)
        {
            DoubleAnimation DA = new DoubleAnimation();
            DA.Completed += DA_ToDown_Completed;
            DA.From = 0;
            DA.To = 80;
            DA.Duration = TimeSpan.FromSeconds(0.2 /*0.3*/);
            DA.FillBehavior = FillBehavior.Stop;
           (cb.RenderTransform as TransformGroup).Children[0].BeginAnimation(TranslateTransform.YProperty, DA);
        }

        private void TranslateToUp(CheckBox cb)
        {
            DoubleAnimation DA = new DoubleAnimation();
            DA.Completed += DA_ToUp_Completed;
            DA.From = 0;
            DA.To = -80;
            DA.Duration = TimeSpan.FromSeconds(0.2 /*0.3*/);
            DA.FillBehavior = FillBehavior.Stop;
            (cb.RenderTransform as TransformGroup).Children[0].BeginAnimation(TranslateTransform.YProperty, DA);
        }
        private int Iteration = 0;
        private void DA_ToUp_Completed(object sender, EventArgs e)
        {
            if (Iteration == 8)
            {
                foreach (CheckBox cb in SP.Children)
                {
                    int newNum = UpdateDownNumber((int)cb.Content);
                    cb.Content = newNum;
                    cb.IsChecked = LB[newNum];
                }
                Iteration = 0;
            }
            else
                Iteration++;
        }

        private void DA_ToDown_Completed(object sender, EventArgs e)
        {
            if (Iteration == 8)
            {
                foreach (CheckBox cb in SP.Children)
                {
                    int newNum = UpdateUpperNumber((int)cb.Content);
                    cb.Content = newNum;
                    cb.IsChecked = LB[newNum];
                }
                Iteration = 0;
            }
            else
                Iteration++;
        }

        private void SP_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                foreach (CheckBox cb in SP.Children)
                {
                    TranslateToDown(cb);
                }
            }
            else
            {
                foreach (CheckBox cb in SP.Children)
                {
                    TranslateToUp(cb);
                }
            }
        }

        private void TurnSwitch(object sender, RoutedEventArgs e) //обработка нажатия на переключатель
        {
            int Index = (int)(sender as CheckBox).Content;

            LB[Index] = LB[Index] ? false : true;
        }
    }

    /// <summary>
    /// Логика взаимодействия для Ablesatafel.xaml
    /// </summary>
    public partial class Ablesatafel : Page
    {
        List<CycleScroll> AllCycleScroll = new List<CycleScroll>();
        List<MainWindow.Wheel> AllWheels = (Application.Current.MainWindow as MainWindow).AllWheels;

        public Ablesatafel()
        {
            InitializeComponent();
        }

        AblesatafelInstructions HelpWindow;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 12; i++)
            {
                AllCycleScroll.Add(new CycleScroll(AllWheels[i].Bytes, SettingPanel, i));
           
                // Генерируем тестовые поля для индексов
                TextBox WheelIndex = new TextBox();
                Canvas.SetZIndex(WheelIndex, 12);
                //Максимальное значение для данного тб
                WheelIndex.Tag = AllWheels[i].BytesCount;
                WheelIndex.TextChanged += IndexChanged;
                WheelIndex.Style = (Style)TryFindResource("IndexStyle");
                Grid.SetRow(WheelIndex, 2);
                Grid.SetColumn(WheelIndex, i);

                Binding bd = new Binding();
                bd.Source = AllWheels[i];
                bd.Path = new PropertyPath(MainWindow.Wheel.CurrentRotorPositionPROP);
                bd.Mode = BindingMode.TwoWay;

                WheelIndex.SetBinding(TextBox.TextProperty, bd);

                SettingPanel.Children.Add(WheelIndex);
            }
        }

        private void IndexChanged(object sender, TextChangedEventArgs e) //ввод индекса диска
        {
            TextBox WheelIndex = sender as TextBox;
            int currentValue;
            int MaxValue = int.Parse(WheelIndex.Tag.ToString());

            if (int.TryParse(WheelIndex.Text, out currentValue)) //защита от некорректного ввода
            {
                if(currentValue >= MaxValue || currentValue < 0)
                {
                    WheelIndex.Text = "0";
                }
            }
            else
            {
                WheelIndex.Text = "0";
            }
        }

        private void UpdateCheckBox()
        {
           for(int i = 0; i < 12; i++)
            {
                AllCycleScroll[i].UpdateBoolList(AllWheels[i].Bytes);
            }
        }

        private void ComeBackToCP(object sender, RoutedEventArgs e) //возвращение на криптографическую панель
        {
            (Application.Current.MainWindow as MainWindow).Closing -= Application_Closing;
            if (HelpWindow != null)
            {
                HelpWindow.Close();
                HelpWindow = null;
            }

            CryptoPanel CP = new CryptoPanel();
           (Application.Current.MainWindow as MainWindow).NavigationFrame.Navigate(CP);
            
        }     
        
        private void SaveSettings(object sender, RoutedEventArgs e) //сохранение настроек
        {
            SaveFileDialog SFD = new SaveFileDialog();

            SFD.Filter = "Setting files(*.szs)|*.szs";

            if (SFD.ShowDialog() == true)
            {
                using (StreamWriter sw = new StreamWriter(SFD.OpenFile(), Encoding.GetEncoding(1251)))
                {
                    foreach (MainWindow.Wheel Rotor in AllWheels)
                    {
                        string CurWheelsSettings = "";
                        for (int i = 0; i < Rotor.BytesCount; i++)
                        {
                            if (Rotor.Bytes[i])
                            {
                                CurWheelsSettings += '1';
                            }
                            else
                            {
                                CurWheelsSettings += '0';
                            }
                        }
                        sw.WriteLine(CurWheelsSettings);                        
                    }
                    foreach (MainWindow.Wheel Rotor in AllWheels)
                    {
                        sw.WriteLine(Rotor.CurrentRotorPosition);
                    }
                    sw.Close();
                }
            }
        }

        private void RecieveSettings(object sender, RoutedEventArgs e) //получение настроек
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Key files(*.szs)|*.szs";
            List<string> SettingsList = new List<string>();

            if (dlg.ShowDialog() == true)
            {
                StreamReader sr = new StreamReader(dlg.FileName);
                while (!sr.EndOfStream)
                {
                    SettingsList.Add(sr.ReadLine());
                }
                sr.Close();
            }
            if (AreSettingsCorrect(SettingsList))
            {
                for (int i = 0; i < AllWheels.Count; i++)
                {
                    for (int j = 0; j < SettingsList[i].Length; j++)
                    {
                        if (SettingsList[i][j] == '1')
                        {
                            AllWheels[i].Bytes[j] = true;                           
                        }
                        else
                        {
                            AllWheels[i].Bytes[j] = false;                           
                        }                        
                    }
                }
                for (int i = AllWheels.Count; i < AllWheels.Count*2; i++)
                {
                    AllWheels[i - AllWheels.Count].CurrentRotorPosition = Convert.ToInt32(SettingsList[i]);
                }

                UpdateCheckBox();
            }
        }

        private string ConvertWarning(List<int> ErrorStrings) /*метод создания корректного предупреждения в зависимости
            от количества ошибочных дисков*/
        {
            string WarningPart = "";
            for (int i = 0; i < ErrorStrings.Count - 1; i++)
            {
                WarningPart += $"{ErrorStrings[i] + 1}, ";
            }
            WarningPart += $"и {ErrorStrings[ErrorStrings.Count - 1] + 1}";

            if (ErrorStrings.Count > 1)
            {
                WarningPart += " дисков";
            }
            else
            {
                WarningPart += " диска";
            }

            return WarningPart;
        }

        private bool AreSettingsCorrect(List<string> SettingsList)//проверки на число и корректность необходимых параметров
        {
            int[] CheckMas = { 43, 47, 51, 53, 59, 37, 61, 41, 31, 29, 26, 23 };
            bool CorrectFlag = true;
            List<int> ErrorStrings = new List<int>();
            if (SettingsList.Count != AllWheels.Count*2)
            {
                CorrectFlag = false;
                MessageBox.Show($"Число строк({SettingsList.Count}) не соответствует необходимому числу" +
                    $" параметров ({AllWheels.Count*2})");
            }
            else
            {
                for (int i = 0; i < SettingsList.Count/2; i++)
                {
                    if (SettingsList[i].Length != CheckMas[i])
                    {
                        CorrectFlag = false;
                        ErrorStrings.Add(i);
                    }
                }
                if (!CorrectFlag)
                {
                    MessageBox.Show($"Некорректно число переключателей {ConvertWarning(ErrorStrings)}");
                    ErrorStrings.Clear();
                }

                for (int i = 0; i < AllWheels.Count; i++)
                {
                    for (int j = 0; j < SettingsList[i].Length; j++)
                    {
                        if (SettingsList[i][j] != '1' && SettingsList[i][j] != '0')
                        {
                            CorrectFlag = false;
                            if (!ErrorStrings.Contains(i))
                                ErrorStrings.Add(i);
                        }
                    }
                }
                if (!CorrectFlag)
                {
                    MessageBox.Show($"Настройки {ConvertWarning(ErrorStrings)} содержат" +
                        $" недопустимые символы");
                    ErrorStrings.Clear();

                }
                else
                {
                    for (int i = AllWheels.Count; i < AllWheels.Count * 2; i++)
                    {
                        if (Convert.ToInt32(SettingsList[i]) >= CheckMas[i - AllWheels.Count] || Convert.ToInt32(SettingsList[i]) < 0)
                        {
                            CorrectFlag = false;
                            ErrorStrings.Add(i - AllWheels.Count);
                        }
                    }
                    if (!CorrectFlag)
                    {
                        MessageBox.Show($"Текущие индексы {ConvertWarning(ErrorStrings)} не входят в допустимый диапазон");
                        ErrorStrings.Clear();
                    }
                }
            }
            return CorrectFlag;

        }


        private void HelpButtonClick(object sender, RoutedEventArgs e) //кнопка вызова инструкций для настройки
        {
            if (HelpWindow == null)
            {
                HelpWindow = new AblesatafelInstructions();
                HelpWindow.Closing += HelpWindowClosing;
                HelpWindow.Show();

                (Application.Current.MainWindow as MainWindow).Closing += Application_Closing;
            }
        }

        private void Application_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HelpWindow.Close();
        }

        private void HelpWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).Closing -= Application_Closing;
            HelpWindow = null;
        }
    }
}
