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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string PlainTextReserve = "";
        public static string ITA2TextReserve = "";
        public static string CryptoTextReserve = "";
        public static bool LetterMode = true;       

        public class Wheel : DependencyObject   //наследую, чтобы получать все методы свойств зависимостей
        {
            //Создаю свойство зависимостей и регистрирую его
            public static DependencyProperty CurrentRotorPositionPROP = DependencyProperty.Register("CurrentRotorPosition", typeof(int), typeof(Wheel));

            //Делегат вращения колёс
            public delegate void WheelsRotate();
            //ВОТ ЕГО ИМЯ ДЛЯ ВЫЗОВА. ТЕПЕРЬ КОЛЕСА ВРАЩАЮТСЯ ТАК
            public WheelsRotate RotateThisWheel;

            public int BytesCount; //число переключателей, характеризующее конкретный диск 

            public Wheel(int bytesCount)
            {
                RotateThisWheel += RotateWheel;

                CurrentRotorPosition = 0;
                BytesCount = bytesCount;
                //Заполняем битовый массив "нулями"
                for (int i = 0; i < BytesCount; i++)
                {
                    Bytes.Add(false);
                }
            }
            public List<bool> Bytes = new List<bool>();  //массив значений переключателей для шифрования

            //Поле текущего индекса диска в свойство для создания привязки
            public int CurrentRotorPosition
            {
                get
                {
                    return (int)GetValue(CurrentRotorPositionPROP);
                }
                set
                {
                    SetValue(CurrentRotorPositionPROP, value);
                }
            }
                  
            //ИНКАПСУЛИРОВАЛ
            private void RotateWheel()
            {
                CurrentRotorPosition--;
                if (CurrentRotorPosition < 0)
                {
                    CurrentRotorPosition = BytesCount - 1;
                }                
            }
        }
        #region Словари
        //Код Бодо
        public static Dictionary<char, List<bool>> CodeBook = new Dictionary<char, List<bool>>
        {
                { 'A', new List<bool>{true, true, false, false, false} }, //11000
                { 'B', new List<bool>{true, false, false, true, true}  }, //10011
                { 'C', new List<bool>{false, true, true, true, false}  }, //01110
                { 'D', new List<bool>{true, false, false, true, false} }, //10010
                { 'E', new List<bool>{true, false, false, false, false}}, //10000
                { 'F', new List<bool>{true, false, true, true, false}  }, //10110
                { 'G', new List<bool>{false, true, false, true, true}  }, //01011
                { 'H', new List<bool>{false, false, true, false, true} }, //00101
                { 'I', new List<bool>{false, true, true, false, false}},  //01100
                { 'J', new List<bool>{true, true, false, true, false}  }, //11010
                { 'K', new List<bool>{true, true, true, true, false}   }, //11110
                { 'L', new List<bool>{false, true, false, false, true} }, //01001
                { 'M', new List<bool>{false, false, true, true, true}  }, //00111
                { 'N', new List<bool>{false, false, true, true, false} }, //00110
                { 'O', new List<bool>{false, false, false, true, true} }, //00011
                { 'P', new List<bool>{false, true, true, false, true}  }, //01101
                { 'Q', new List<bool>{true, true, true, false, true}   }, //11101
                { 'R', new List<bool>{false, true, false, true, false} }, //01010
                { 'S', new List<bool>{true, false, true, false, false} }, //10100
                { 'T', new List<bool>{false, false, false, false, true}}, //00001
                { 'U', new List<bool>{true, true, true, false, false}  }, //11100
                { 'V', new List<bool>{false, true, true, true, true}   }, //01111
                { 'W', new List<bool>{true, true, false, false, true}  }, //11001
                { 'X', new List<bool>{true, false, true, true, true}   }, //10111
                { 'Y', new List<bool>{true, false, true, false, true}  }, //10101
                { 'Z', new List<bool>{true, false, false, false, true} }, //10001
                { '/', new List<bool>{false, false, false, false, false}},//00000
                { '3', new List<bool>{false, false, false, true, false }},//00010
                { '4', new List<bool>{false, true, false, false, false }},//01000
                { '5', new List<bool>{true, true, false, true, true } },  //11011
                { '8', new List<bool>{true, true, true, true, true } },   //11111
                { '9', new List<bool>{false, false, true, false, false}}  //00100
        };
        //Обратный код Бодо
        public static Dictionary<string, char> ReverseCodeBook = new Dictionary<string, char>()
        {
                { "11000", 'A' }, //11000
                { "10011", 'B' }, //10011
                { "01110", 'C' }, //01110
                { "10010", 'D' }, //10010
                { "10000", 'E' }, //10000
                { "10110", 'F' }, //10110
                { "01011", 'G' }, //01011
                { "00101", 'H' }, //00101
                { "01100", 'I' },  //01100
                { "11010", 'J' }, //11010
                { "11110", 'K' }, //11110
                { "01001", 'L' }, //01001
                { "00111", 'M' }, //00111
                { "00110", 'N' }, //00110
                { "00011", 'O' }, //00011
                { "01101", 'P' }, //01101
                { "11101", 'Q' }, //11101
                { "01010", 'R' }, //01010
                { "10100", 'S' }, //10100
                { "00001", 'T' }, //00001
                { "11100", 'U' }, //11100
                { "01111", 'V' }, //01111
                { "11001", 'W' }, //11001
                { "10111", 'X' }, //10111
                { "10101", 'Y' }, //10101
                { "10001", 'Z' }, //10001
                { "00000", '/' }, //00000
                { "00010", '3' }, //00010
                { "01000", '4' }, //01000
                { "11011", '5' }, //11011
                { "11111", '8' }, //11111
                { "00100", '9' }  //00100

        };
        //Словарь кодирования цифр и специальных символов
        public static Dictionary<char, char> FromNumToLetter = new Dictionary<char, char>()
        {
            {'1', 'Q' },
            {'2', 'W' },
            {'3', 'E' },
            {'4', 'R' },
            {'5', 'T' },
            {'6', 'Y' },
            {'7', 'U' },
            {'8', 'I' },
            {'9', 'O' },
            {'0', 'P' },
            {'?', 'B' },
            {',', 'N' },
            {'.', 'M' },
            {'-', 'A' },
            {'/', 'X' },
            {'\'', 'S'},
            {'[', 'K' },
            {']', 'L' },
            {'=', 'V' }
        };
        //Словарь, обратный предыдущему
        public static Dictionary<char, char> FromLetterToNum = new Dictionary<char, char>()
        {
            {'Q', '1' },
            {'W', '2' },
            {'E', '3' },
            {'R', '4' },
            {'T', '5' },
            {'Y', '6' },
            {'U', '7' },
            {'I', '8' },
            {'O', '9' },
            {'P', '0' },
            {'B', '?' },
            {'N', ',' },
            {'M', '.' },
            {'A', '-' },
            {'X', '/' },
            {'S', '\''},
            {'K', '[' },
            {'L', ']' },
            {'V', '=' }
        };
        //Словарь перевода из сигналов от клавиш в нормальные названия
        public static Dictionary<string, char> OemToChar = new Dictionary<string, char>()
        {
            {"OemQuestion", '?' },
            {"OemComma", ',' },
            {"OemPeriod", '.' },            
            {"OemMinus", '-' },
            {"Divide", '/' },
            {"OemQuotes", '\'' },
            {"OemOpenBrackets", '[' },
            {"Oem6", ']' },
            {"Oem5", '/' },
            {"OemPlus", '=' },
            {"D1", '1' },
            {"D2", '2' },
            {"D3", '3' },
            {"D4", '4' },
            {"D5", '5' },
            {"D6", '6' },
            {"D7", '7' },
            {"D8", '8' },
            {"D9", '9' },
            {"D0", '0' }
        };
        #endregion
        #region Роторы
        //Psi - роторы
        public Wheel Psi43 = new Wheel(43);
        public Wheel Psi47 = new Wheel(47);
        public Wheel Psi51 = new Wheel(51);
        public Wheel Psi53 = new Wheel(53);
        public Wheel Psi59 = new Wheel(59);
        //Mu - роторы
        public Wheel Mu37 = new Wheel(37);
        public Wheel Mu61 = new Wheel(61);
        //X - роторы
        public Wheel X41 = new Wheel(41);
        public Wheel X31 = new Wheel(31);
        public Wheel X29 = new Wheel(29);
        public Wheel X26 = new Wheel(26);
        public Wheel X23 = new Wheel(23);
        public List<Wheel> PsiWheels = new List<Wheel>();
        public List<Wheel> XWheels = new List<Wheel>();
        public List<Wheel> AllWheels = new List<Wheel>();
        #endregion
        public MainWindow()  //инициализация дисковых списков
        {
            InitializeComponent();
            PsiWheels.Add(Psi43);
            PsiWheels.Add(Psi47);
            PsiWheels.Add(Psi51);
            PsiWheels.Add(Psi53);
            PsiWheels.Add(Psi59);
            XWheels.Add(X41);
            XWheels.Add(X31);
            XWheels.Add(X29);
            XWheels.Add(X26);
            XWheels.Add(X23);

            AllWheels.Add(Psi43);
            AllWheels.Add(Psi47);
            AllWheels.Add(Psi51);
            AllWheels.Add(Psi53);
            AllWheels.Add(Psi59);
            AllWheels.Add(Mu37);
            AllWheels.Add(Mu61);
            AllWheels.Add(X41);
            AllWheels.Add(X31);
            AllWheels.Add(X29);
            AllWheels.Add(X26);
            AllWheels.Add(X23);
        }      
       
    }
}
