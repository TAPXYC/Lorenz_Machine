using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace CW___SZ4042
{
    /// <summary>
    /// Логика взаимодействия для CryptoPanel.xaml
    /// </summary>
    public partial class CryptoPanel : Page
    {
        NavigationWindow NW;
        DispatcherTimer T;
        char Letter;
        string FileText = "";
        int counter = 0;
        class AnimationWheel
        {
            int[] PositionOfWheels = new int[] { 0, 1, 2, 3, 4, 6, 7, 9, 10, 11, 12, 13 };
            TextBlock prev;
            TextBlock curr;
            TextBlock next;
            TextBlock NextNext;

            MainWindow.Wheel Wheel;

            public AnimationWheel(int NumberOfWheel, Grid WheelContainer, MainWindow.Wheel wheel, Grid ContainerIndexOfRotors)
            {
                Wheel = wheel;

                Wheel.RotateThisWheel += RotateCurWheel;

                Rectangle border = new Rectangle() { Stroke = Brushes.Black, StrokeThickness = 2 };
                Grid.SetRow(border, 1);
                Grid.SetColumn(border, PositionOfWheels[NumberOfWheel]);

                int CurrentRotorPosition = wheel.CurrentRotorPosition;

                prev = new TextBlock();
                InitializeTextBlock(prev, 80);
                Grid.SetColumn(prev, PositionOfWheels[NumberOfWheel]);

                curr = new TextBlock();
                InitializeTextBlock(curr, 0);
                Grid.SetColumn(curr, PositionOfWheels[NumberOfWheel]);

                next = new TextBlock();
                InitializeTextBlock(next, -80);
                Grid.SetColumn(next, PositionOfWheels[NumberOfWheel]);

                NextNext = new TextBlock();
                Grid.SetColumn(NextNext, PositionOfWheels[NumberOfWheel]);
                InitializeTextBlock(NextNext, -140);


                // Генерируем тестовые поле для индекса этого колеса
                TextBlock tb = new TextBlock();

                Binding bd = new Binding();
                bd.Source = Wheel;
                bd.Path = new PropertyPath(MainWindow.Wheel.CurrentRotorPositionPROP);
                bd.Mode = BindingMode.TwoWay;

                tb.SetBinding(TextBlock.TextProperty, bd);

                Border brd = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(2),
                    Child = tb
                };

                Grid.SetColumn(brd, PositionOfWheels[NumberOfWheel]);

                ContainerIndexOfRotors.Children.Add(brd);

                UpdateNumber();

                WheelContainer.Children.Add(prev);
                WheelContainer.Children.Add(curr);
                WheelContainer.Children.Add(next);
                WheelContainer.Children.Add(NextNext);
                WheelContainer.Children.Add(border);
            }

            void UpdateNumber()
            {
                int curPos = Wheel.CurrentRotorPosition;

                //Для предыдущего
                int boolIndex = curPos == Wheel.BytesCount - 1 ? 0 : curPos + 1;

                prev.Text = (Wheel.Bytes[boolIndex] ? 1:0).ToString();

                //Для текущего
                boolIndex = curPos;
                curr.Text = (Wheel.Bytes[boolIndex] ?1:0).ToString();

                //Для следующего
                boolIndex = curPos == 0 ?
                    Wheel.BytesCount - 1
                    :
                    curPos - 1;

                next.Text = (Wheel.Bytes[boolIndex] ?1:0).ToString();

                //Для следующего-следующего
                boolIndex = curPos - 2 < 0 ?
                    Wheel.BytesCount + curPos - 2
                    :
                    curPos - 2;
                NextNext.Text =(Wheel.Bytes[boolIndex]?1:0).ToString();
            }

            void InitializeTextBlock(TextBlock tb, int Y)
            {
                TranslateTransform translate = new TranslateTransform();
                translate.Y = Y;

                TransformGroup TransformGroup = new TransformGroup();
                TransformGroup.Children.Add(translate);

                tb.RenderTransform = TransformGroup;
            }

            void TranslateNumber(TextBlock tb, int To, int From)
            {
                DoubleAnimation DA = new DoubleAnimation();
                DA.Completed += DA_Completed;
                DA.From = From;
                DA.To = To;
                DA.Duration = TimeSpan.FromSeconds(0.2);
                DA.FillBehavior = FillBehavior.Stop;

                (tb.RenderTransform as TransformGroup).Children[0].BeginAnimation(TranslateTransform.YProperty, DA);
            }

            private void DA_Completed(object sender, EventArgs e)
            {
                UpdateNumber();
            }

            public void RotateCurWheel()
            {
                TranslateNumber(prev, 140, 80);
                TranslateNumber(curr, 80,0);
                TranslateNumber(next,0, -80);
                TranslateNumber(NextNext,-80,-140);
            }
        }

        List<MainWindow.Wheel> AllWheels = (Application.Current.MainWindow as MainWindow).AllWheels;
        int LetterBytesCount = 5; //число бит в одной букве
        List< AnimationWheel> AnimationWheels = new List<AnimationWheel>();

        //ЗАГРУЗКА
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            bool IsStrongOption = true;

            for(int i = 0; i < 12; i++)
            {
                //если всё еще сильная настройка - то чекаем дальше
                if(IsStrongOption)
                {
                    int NumActiv = 0;
                    foreach (bool check in AllWheels[i].Bytes)
                    {
                        if(check)
                        {
                            NumActiv++;
                        }
                    }

                    if(!(NumActiv > 5 && AllWheels[i].BytesCount - NumActiv > 5))
                    {
                        IsStrongOption = false;
                    }
                }
                AnimationWheels.Add(new AnimationWheel(i, WheelField, AllWheels[i], IndexOfRotors));
            }

            if(IsStrongOption)
            {
                PasswordLevelStrong.Foreground = Brushes.Cyan;
                PasswordLevelStrong.Text = "Сильная конфигурация дисков";
            }
            else
            {
                PasswordLevelStrong.Foreground = Brushes.Red;
                PasswordLevelStrong.Text = "Внимание!! Слабая конфигурация дисков";
            }
        }


        public CryptoPanel()
        {
            InitializeComponent();
            //на экран выводится то, что хранится в резерве - так при переходе со страницы настроек данные не теряются
            PlainText.Focus();
            PlainText.Text = MainWindow.PlainTextReserve;
            ITA2.Text = MainWindow.ITA2TextReserve;
            CryptoText.Text = MainWindow.CryptoTextReserve;
            ShowCipherKeys();
        }
        KeyConverter KC = new KeyConverter();
        //создание ссылок
        MainWindow.Wheel Psi43 = (Application.Current.MainWindow as MainWindow).Psi43;
        MainWindow.Wheel Psi47 = (Application.Current.MainWindow as MainWindow).Psi47;
        MainWindow.Wheel Psi51 = (Application.Current.MainWindow as MainWindow).Psi51;
        MainWindow.Wheel Psi53 = (Application.Current.MainWindow as MainWindow).Psi53;
        MainWindow.Wheel Psi59 = (Application.Current.MainWindow as MainWindow).Psi59;
        MainWindow.Wheel Mu37 = (Application.Current.MainWindow as MainWindow).Mu37;
        MainWindow.Wheel Mu61 = (Application.Current.MainWindow as MainWindow).Mu61;
        MainWindow.Wheel X41 = (Application.Current.MainWindow as MainWindow).X41;
        MainWindow.Wheel X31 = (Application.Current.MainWindow as MainWindow).X31;
        MainWindow.Wheel X29 = (Application.Current.MainWindow as MainWindow).X29;
        MainWindow.Wheel X26 = (Application.Current.MainWindow as MainWindow).X26;
        MainWindow.Wheel X23 = (Application.Current.MainWindow as MainWindow).X23;
        List<MainWindow.Wheel> PsiWheels = (Application.Current.MainWindow as MainWindow).PsiWheels;
        List<MainWindow.Wheel> XWheels = (Application.Current.MainWindow as MainWindow).XWheels;

        private void GoToAblesetafel(object sender, RoutedEventArgs e) //перейти на панель настройки
        {
            //Ablesatafel Asf = new Ablesatafel();
            Ablesatafel a = new Ablesatafel();
            (Application.Current.MainWindow as MainWindow).NavigationFrame.Navigate(a);
        }
        //Событие нажатия на клавишу
        public void InputLetter(object sender, KeyEventArgs e)
        {        
            string EKey = e.Key.ToString();
            char ch;
            if (DecryptMode.IsChecked == false) //если в режиме шифрования
            {
                if (e.Key == Key.Space && MainWindow.LetterMode)  //пробел считаем как букву, поэтому кодируем
                                                                 //его в девятку и просто шифруем
                {
                    MainWindow.PlainTextReserve = PlainText.Text += " ";
                    PlainLetter.Content = "_";
                    ShowPlainLetterBytes('9');
                    MainWindow.ITA2TextReserve = ITA2.Text += "9";
                    MainWindow.CryptoTextReserve = CryptoText.Text += CryptCurrentLetter('9');
                    ShowCryptedLetterBytes(CryptCurrentLetter('9'));
                    RotateWheels();
                    ShowCipherKeys();
                }
                else if (e.Key == Key.Space && !MainWindow.LetterMode) //если до этого пользователем вводились знаки пунктуации
                                                                      //или цифры, то выходим из символьного режима
                {
                    PlainLetter.Content = "_";
                    //ставим 8 - контрольное значение, обозначающее конец символьного режима
                    MainWindow.ITA2TextReserve = ITA2.Text += "8";
                    MainWindow.LetterMode = true;
                    MainWindow.CryptoTextReserve = CryptoText.Text += CryptCurrentLetter('8');
                    ShowCryptedLetterBytes(CryptCurrentLetter('8'));
                    RotateWheels();
                    ShowCipherKeys();
                    //шифруем "9", т.к. это код пробела в данном шифре
                    MainWindow.PlainTextReserve = PlainText.Text += " ";
                    ShowPlainLetterBytes('9');
                    MainWindow.ITA2TextReserve = ITA2.Text += "9";
                    MainWindow.CryptoTextReserve = CryptoText.Text += CryptCurrentLetter('9');
                    ShowCryptedLetterBytes(CryptCurrentLetter('9'));
                    RotateWheels();
                    ShowCipherKeys();
                }             


                if (char.TryParse(EKey, out ch)) //если это конкретный символ, а не, скажем, Enter или что-то в этом роде 
                {
                    PlainLetter.Content = ch;
                    ShowPlainLetterBytes(ch);
                    if (char.IsLetter(ch) && MainWindow.LetterMode)
                    {
                        MainWindow.PlainTextReserve = PlainText.Text += ch.ToString().ToUpper();
                        MainWindow.ITA2TextReserve = ITA2.Text += ch.ToString().ToUpper();
                        MainWindow.CryptoTextReserve = CryptoText.Text += CryptCurrentLetter(ch);
                        ShowCryptedLetterBytes(CryptCurrentLetter(ch));
                        RotateWheels();
                        ShowCipherKeys();
                    }
                    else if (char.IsLetter(ch) && !MainWindow.LetterMode) /*окончание символьного режима обозначается контрольным
                    символом 8 */
                    {
                        MainWindow.ITA2TextReserve = ITA2.Text += "8";
                        MainWindow.LetterMode = true;
                        MainWindow.CryptoTextReserve = CryptoText.Text += CryptCurrentLetter('8');
                        ShowCryptedLetterBytes(CryptCurrentLetter('8'));
                        RotateWheels();
                        ShowCipherKeys();
                        MainWindow.PlainTextReserve = PlainText.Text += ch.ToString().ToUpper();
                        MainWindow.ITA2TextReserve = ITA2.Text += ch.ToString().ToUpper();
                        MainWindow.CryptoTextReserve = CryptoText.Text += CryptCurrentLetter(ch);
                        ShowCryptedLetterBytes(CryptCurrentLetter(ch));
                        RotateWheels();
                        ShowCipherKeys();
                    }

                }
                else if (MainWindow.OemToChar.ContainsKey(EKey)) //если введён по-дурацки обозначенный символ, например
                                                                //Oem5 - это "/". Это цифры и допустимые знаки пунктуации
                {
                    ch = MainWindow.OemToChar[EKey];  //преобразуем их в нормальный вид
                    PlainLetter.Content = MainWindow.FromNumToLetter[ch];
                    ShowPlainLetterBytes(MainWindow.FromNumToLetter[ch]);
                    if (MainWindow.LetterMode)
                    {
                        //ставим "5" - код начала символьного режима, после которого все цифры и знаки будут заменены
                        //соответствующими буквами
                        MainWindow.ITA2TextReserve = ITA2.Text += "5";
                        MainWindow.LetterMode = false;
                        MainWindow.CryptoTextReserve = CryptoText.Text += CryptCurrentLetter('5');
                        ShowCryptedLetterBytes(CryptCurrentLetter('5'));
                        RotateWheels();
                        ShowCipherKeys();
                        //шифруем код введённого символа
                        MainWindow.PlainTextReserve = PlainText.Text += ch.ToString();
                        ch = MainWindow.FromNumToLetter[ch];
                        MainWindow.ITA2TextReserve = ITA2.Text += ch.ToString();
                        MainWindow.CryptoTextReserve = CryptoText.Text += CryptCurrentLetter(ch);
                        ShowCryptedLetterBytes(CryptCurrentLetter(ch));
                        RotateWheels();
                        ShowCipherKeys();
                    }
                    else
                    {
                        //если в своём режиме - просто шифруем код символа
                        MainWindow.PlainTextReserve = PlainText.Text += ch.ToString();
                        ch = MainWindow.FromNumToLetter[ch];
                        MainWindow.ITA2TextReserve = ITA2.Text += ch.ToString();
                        MainWindow.CryptoTextReserve = CryptoText.Text += CryptCurrentLetter(ch);
                        ShowCryptedLetterBytes(CryptCurrentLetter(ch));
                        RotateWheels();
                        ShowCipherKeys();
                    }
                }
            }
            else //режим дешифрования
            {
                if (char.TryParse(EKey, out ch))
                {
                    PlainLetter.Content = ch;
                    ShowPlainLetterBytes(ch);
                    Decrypt(ch);
                    ShowCryptedLetterBytes(CryptCurrentLetter(ch));
                    RotateWheels();
                    ShowCipherKeys();

                }
                else if (e.Key == Key.D3 || e.Key == Key.D4 || e.Key == Key.D5|| e.Key == Key.D8 || e.Key == Key.D9 ||e.Key==Key.Oem5 )
                {
                    ch = MainWindow.OemToChar[EKey];
                    PlainLetter.Content = ch;
                    ShowPlainLetterBytes(ch);                    
                    PlainLetter.Content = MainWindow.FromNumToLetter[ch];
                    ShowPlainLetterBytes(MainWindow.FromNumToLetter[ch]);
                    Decrypt(ch);
                    ShowCryptedLetterBytes(CryptCurrentLetter(ch));
                    RotateWheels();
                    ShowCipherKeys();
                }


            }
        }
        public void Decrypt(char ch)
        {
            //то же, что и шифрование, только необходимо перевести промежуточный код в человеческий вид
            MainWindow.PlainTextReserve = PlainText.Text += ch.ToString().ToUpper();
            char CryptedLetter = CryptCurrentLetter(ch);
            MainWindow.ITA2TextReserve = ITA2.Text += CryptedLetter;
            if (CryptedLetter == '9')
            {
                MainWindow.CryptoTextReserve = CryptoText.Text += ' ';
            }
            else if (CryptedLetter == '5')
            {
                MainWindow.LetterMode = false;
            }
            else if (CryptedLetter == '8')
            {
                MainWindow.LetterMode = true;
            }
            else if (!MainWindow.LetterMode) /*при неправильном дешифровании, машина может случайно перейти в другой режим 
                и всё сломать, так что препятствуем этому*/
            try
            {
                MainWindow.CryptoTextReserve = CryptoText.Text += MainWindow.FromLetterToNum[CryptedLetter];
            }
                catch
                {
                    MessageBox.Show("Похоже, вы неправильно дешифруете. Убедитесь, что настройки и вводимый текст верны");
                }
            else
            {
                MainWindow.CryptoTextReserve = CryptoText.Text += CryptedLetter;
            }
        }

          public void RotateWheels()
        {
            //если переключатель левого 37-битового Мю-ротора поднят, то двигаются все Пси-роторы
            if (Mu37.Bytes[Mu37.CurrentRotorPosition])
            {
                Psi59.RotateThisWheel();
                Psi53.RotateThisWheel();
                Psi51.RotateThisWheel();
                Psi47.RotateThisWheel();
                Psi43.RotateThisWheel();
            }
            //Левый 37-битовый Мю-ротор двигается только, если переключатель в текущей итерации поднят (true)
            if (Mu61.Bytes[Mu61.CurrentRotorPosition])
                {
                    Mu37.RotateThisWheel();
                }
            //Хи-роторы, а также правый (61 битовый) Мю-ротор двигаются всегда
            Mu61.RotateThisWheel();            
            X41.RotateThisWheel();
            X31.RotateThisWheel();
            X29.RotateThisWheel();
            X26.RotateThisWheel();
            X23.RotateThisWheel();       
        }
        public string GetStringCode(List<bool> BinaryCode) //получаем удобную для поиска в словарях и сохранения в файл запись
        {
            string StringCode = "";
            for (int i = 0; i < LetterBytesCount; i++)
            {
                if (BinaryCode[i])
                {
                    StringCode += '1';
                }
                else
                {
                    StringCode += '0';
                }
            }
            return StringCode;
        }
        public void ShowCipherKeys() //выводим символы, генерируемый первыми и последними пятью дисками
        {
            List<bool> CKey1List = new List<bool>();
            List<bool> CKey2List = new List<bool>();
            for (int i = 0; i < LetterBytesCount; i++)
            {
                CKey1List.Add(PsiWheels[i].Bytes[PsiWheels[i].CurrentRotorPosition]);
                CKey2List.Add(XWheels[i].Bytes[XWheels[i].CurrentRotorPosition]);
            }
            CipherKey1.Content = MainWindow.ReverseCodeBook[GetStringCode(CKey1List)];
            CipherKey2.Content = MainWindow.ReverseCodeBook[GetStringCode(CKey2List)];
        }
        public void ShowPlainLetterBytes(char letter) //показывает биты введённой буквы
        {
            PlainByte0.Content = BoolToInt(MainWindow.CodeBook[letter][0]);
            PlainByte1.Content = BoolToInt(MainWindow.CodeBook[letter][1]);
            PlainByte2.Content = BoolToInt(MainWindow.CodeBook[letter][2]);
            PlainByte3.Content = BoolToInt(MainWindow.CodeBook[letter][3]);
            PlainByte4.Content = BoolToInt(MainWindow.CodeBook[letter][4]);
        }
        public int BoolToInt(bool bit) 
        {
            if (bit)
                return 1;
            else
                return 0;
        }
        public void ShowCryptedLetterBytes(char letter) //показывает биты полученной буквы
        {
            CipherLetter.Content = letter;
            CipherByte0.Content = BoolToInt(MainWindow.CodeBook[letter][0]);
            CipherByte1.Content = BoolToInt(MainWindow.CodeBook[letter][1]);
            CipherByte2.Content = BoolToInt(MainWindow.CodeBook[letter][2]);
            CipherByte3.Content = BoolToInt(MainWindow.CodeBook[letter][3]);
            CipherByte4.Content = BoolToInt(MainWindow.CodeBook[letter][4]);
        }
        public char GetLetterFromWheels(List<MainWindow.Wheel> xWheels) //получаем букву из текущих значений битов на роторах
        {
            List<bool> CurrentGeneratingLetter = new List<bool>();
            for (int i = 0; i < xWheels.Count; i++)
            {
                CurrentGeneratingLetter.Add(xWheels[i].Bytes[xWheels[i].CurrentRotorPosition]);
            }
            return MainWindow.ReverseCodeBook[GetStringCode(CurrentGeneratingLetter)];
        }

        public char CryptCurrentLetter(char CurLetter) // Шифр Вернама с двумя ключами
        {
            List<bool> LetterCode = new List<bool>();
            for (int i = 0; i < LetterBytesCount; i++) //5 битов у одной буквы
            {
                LetterCode.Add(MainWindow.CodeBook[CurLetter][i] ^ PsiWheels[i].Bytes[PsiWheels[i].CurrentRotorPosition] ^
                    XWheels[i].Bytes[XWheels[i].CurrentRotorPosition]); //Plain XOR Psi XOR Chi
                
            }
            return MainWindow.ReverseCodeBook[GetStringCode(LetterCode)]; //вытаскиваем из словаря символ по полученному битовому коду
        }
        private void ClickDeleteButton(object sender, RoutedEventArgs e) //очищение текстовых полей
        {
            PlainText.Text = "";
            MainWindow.PlainTextReserve = "";
            ITA2.Text = "";
            MainWindow.ITA2TextReserve = "";
            CryptoText.Text = "";
            MainWindow.CryptoTextReserve = "";
            PlainText.Focus();
        }

        private void RecieveInstructions(object sender, RoutedEventArgs e) //кнопка получения инструкций
        {
            if (NW == null)
            {
                NW = new NavigationWindow();
                NW.Owner = Application.Current.MainWindow as MainWindow;
                NW.Closing += NWClosing;
                NW.Show();

                (Application.Current.MainWindow as MainWindow).Closing += ApplicationClosing;
            }
        }

        private void ApplicationClosing(object sender, System.ComponentModel.CancelEventArgs e) /*при закрытии основного окна,
            закроется и окно инструкций */
        {
            NW.Close();
        }

        private void NWClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).Closing -= ApplicationClosing;
            NW = null;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).Closing -= ApplicationClosing;
            if (NW != null)
            {
                NW.Close();
                NW = null;
            }
        }

        private void ChangeMode(object sender, RoutedEventArgs e) /*при смене режима шифрования/дешифрования очищаем текст,
            чтобы не создавать проблем */
        {
            MainWindow.LetterMode = true;
            PlainText.Text = "";
            MainWindow.PlainTextReserve = "";
            ITA2.Text = "";
            MainWindow.ITA2TextReserve = "";
            CryptoText.Text = "";
            MainWindow.CryptoTextReserve = "";
            PlainText.Focus();
        }

        private void SaveCryptoText(object sender, RoutedEventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog();

            SFD.Filter = "Text files(*.txt)|*.txt";

            if (SFD.ShowDialog() == true)
            {
                using (StreamWriter sw = new StreamWriter(SFD.OpenFile(), Encoding.GetEncoding(1251)))
                {
                    foreach (char letter in CryptoText.Text)
                    {
                        sw.Write(letter);
                    }
                }
            }
        }
        void CryptDownloadedText(string FileText)
        {
            T = new DispatcherTimer();
            T.Tick += CryptLetter;
            T.Interval = new TimeSpan(0, 0, 0, 0, 150);
            T.Start();
            
        }
        
        private void CryptLetter(object sender, EventArgs e)
        {
            Letter = FileText[counter];
           if (DecryptMode.IsChecked == false)
            {
                if (Letter == ' ' && MainWindow.LetterMode)
                {
                    MainWindow.PlainTextReserve = PlainText.Text += " ";
                    PlainLetter.Content = "_";
                    ShowPlainLetterBytes('9');
                    MainWindow.ITA2TextReserve = ITA2.Text += "9";
                    MainWindow.CryptoTextReserve = CryptoText.Text += CryptCurrentLetter('9');
                    ShowCryptedLetterBytes(CryptCurrentLetter('9'));
                    RotateWheels();
                    ShowCipherKeys();
                }
                else if (Letter == ' ' && !MainWindow.LetterMode)
                {
                    PlainLetter.Content = "_";
                    MainWindow.ITA2TextReserve = ITA2.Text += "8";
                    MainWindow.LetterMode = true;
                    MainWindow.CryptoTextReserve = CryptoText.Text += CryptCurrentLetter('8');
                    ShowCryptedLetterBytes(CryptCurrentLetter('8'));
                    RotateWheels();
                    ShowCipherKeys();
                    MainWindow.PlainTextReserve = PlainText.Text += " ";
                    ShowPlainLetterBytes('9');
                    MainWindow.ITA2TextReserve = ITA2.Text += "9";
                    MainWindow.CryptoTextReserve = CryptoText.Text += CryptCurrentLetter('9');
                    ShowCryptedLetterBytes(CryptCurrentLetter('9'));
                    RotateWheels();
                    ShowCipherKeys();
                }
                else if (char.IsLetter(Letter) && MainWindow.LetterMode)
                {
                    PlainLetter.Content = Letter;
                    ShowPlainLetterBytes(Letter);
                    MainWindow.PlainTextReserve = PlainText.Text += Letter.ToString().ToUpper();
                    MainWindow.ITA2TextReserve = ITA2.Text += Letter.ToString().ToUpper();
                    MainWindow.CryptoTextReserve = CryptoText.Text += CryptCurrentLetter(Letter);
                    ShowCryptedLetterBytes(CryptCurrentLetter(Letter));
                    RotateWheels();
                    ShowCipherKeys();
                }
                else if (char.IsLetter(Letter) && !MainWindow.LetterMode) /*окончание символьного режима обозначается контрольным
                    символом 8 */
                {
                    MainWindow.ITA2TextReserve = ITA2.Text += "8";
                    MainWindow.LetterMode = true;
                    MainWindow.CryptoTextReserve = CryptoText.Text += CryptCurrentLetter('8');
                    ShowCryptedLetterBytes(CryptCurrentLetter('8'));
                    RotateWheels();
                    ShowCipherKeys();                    
                    MainWindow.PlainTextReserve = PlainText.Text += Letter.ToString().ToUpper();
                    MainWindow.ITA2TextReserve = ITA2.Text += Letter.ToString().ToUpper();
                    MainWindow.CryptoTextReserve = CryptoText.Text += CryptCurrentLetter(Letter);
                    ShowCryptedLetterBytes(CryptCurrentLetter(Letter));
                    RotateWheels();
                    ShowCipherKeys();
                }
                else if (MainWindow.FromNumToLetter.ContainsKey(Letter))
                {
                    PlainLetter.Content = MainWindow.FromNumToLetter[Letter];
                    ShowPlainLetterBytes(MainWindow.FromNumToLetter[Letter]);
                    if (MainWindow.LetterMode)
                    {
                        MainWindow.ITA2TextReserve = ITA2.Text += "5";
                        MainWindow.LetterMode = false;
                        MainWindow.CryptoTextReserve = CryptoText.Text += CryptCurrentLetter('5');
                        ShowCryptedLetterBytes(CryptCurrentLetter('5'));
                        RotateWheels();
                        ShowCipherKeys();                        
                        MainWindow.PlainTextReserve = PlainText.Text += Letter.ToString();
                        Letter = MainWindow.FromNumToLetter[Letter];
                        MainWindow.ITA2TextReserve = ITA2.Text += Letter.ToString();
                        MainWindow.CryptoTextReserve = CryptoText.Text += CryptCurrentLetter(Letter);
                        ShowCryptedLetterBytes(CryptCurrentLetter(Letter));
                        RotateWheels();
                        ShowCipherKeys();
                    }
                    else if (MainWindow.FromNumToLetter.ContainsKey(Letter))
                    {
                        MainWindow.PlainTextReserve = PlainText.Text += Letter.ToString();
                        Letter = MainWindow.FromNumToLetter[Letter];
                        MainWindow.ITA2TextReserve = ITA2.Text += Letter.ToString();
                        MainWindow.CryptoTextReserve = CryptoText.Text += CryptCurrentLetter(Letter);
                        ShowCryptedLetterBytes(CryptCurrentLetter(Letter));
                        RotateWheels();
                        ShowCipherKeys();
                    }                   
                }                
            }   
            
            else //режим дешифрования
            {
                if (char.IsLetter(Letter))
                {
                    PlainLetter.Content = Letter;
                    ShowPlainLetterBytes(Letter);
                    Decrypt(Letter);
                    ShowCryptedLetterBytes(CryptCurrentLetter(Letter));
                    RotateWheels();
                    ShowCipherKeys();

                }
                else if (MainWindow.FromNumToLetter.ContainsKey(Letter))
                {
                    PlainLetter.Content = Letter;
                    ShowPlainLetterBytes(Letter);
                    PlainLetter.Content = MainWindow.FromNumToLetter[Letter];
                    ShowPlainLetterBytes(MainWindow.FromNumToLetter[Letter]);
                    Decrypt(Letter);
                    ShowCryptedLetterBytes(CryptCurrentLetter(Letter));
                    RotateWheels();
                    ShowCipherKeys();
                }                
            }
            counter++;
            if (counter == FileText.Length)
            {
                (sender as DispatcherTimer).Stop();
                sender = null;
            }
        }


        private void DownloadText(object sender, RoutedEventArgs e) //загрузить текст для шифрования/дешифрования
        {            
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Txt files(*.txt)|*.txt";
            List<string> TextList = new List<string>();
            if (dlg.ShowDialog() == true)
            {
                StreamReader sr = new StreamReader(dlg.FileName);
                while (!sr.EndOfStream)
                {
                    TextList.Add(sr.ReadLine());
                }
                sr.Close();
                foreach (string String in TextList)
                {
                    for (int i=0; i<String.Length; i++)
                    {
                        if (char.IsLetter(String[i]))
                        {
                            FileText += String[i].ToString().ToUpper();
                        }
                        else if (MainWindow.FromNumToLetter.ContainsKey(String[i]) || String[i] == ' ')
                        {
                            FileText += String[i];
                        }
                        else
                        {
                            MessageBox.Show("Данный текст содержит недопустимые символы. Допускаются лишь латинские буквы, цифры," +
                                " а так же знаки из следующего набора\n .,?-=/'[]");
                            return;
                        }
                    }
                    if (DecryptMode.IsChecked == false) //разделителем строк в тексте будет двойной слэш
                    FileText += "//";
                }
                CryptDownloadedText(FileText);
            }
        }
    }
}
