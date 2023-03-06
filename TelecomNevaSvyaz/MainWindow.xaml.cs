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
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace TelecomNevaSvyaz
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string code;
        int countTime; // Время до окончания действия кода
        DispatcherTimer disTimer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            Base.baseDate = new BaseDate();
            pbPassword.IsEnabled = false;
            tbCode.IsEnabled = false;
            btnLogin.IsEnabled = false;
            disTimer.Interval = new TimeSpan(0, 0, 1);
            disTimer.Tick += new EventHandler(DisTimer_Tick);
        }

        private void btnCancellation_Click(object sender, RoutedEventArgs e)
        {
            tbNomer.Text = "";
            pbPassword.Password = "";
            tbCode.Text = "";
            disTimer.Stop();
            code = "";
            tbRemainingTime.Text = "";
            pbPassword.IsEnabled = false;
            tbCode.IsEnabled = false;
            btnLogin.IsEnabled = false;
        }

        private void tbNomer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Employees employee = Base.baseDate.Employees.FirstOrDefault(x => x.nomer == tbNomer.Text);
                if (employee != null)
                {
                    pbPassword.IsEnabled = true;
                    pbPassword.Focus();
                }
                else
                {
                    pbPassword.IsEnabled = false;
                    pbPassword.Password = "";
                    MessageBox.Show("Произошла ошибка! Сотрудник  с таким номером не найден!");
                }
            } 
        }

        private void pbPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                GetNewCode();
            }
        }
        private void DisTimer_Tick(object sender, EventArgs e)
        {
            if (countTime == 0) // Если 10 секунд закончились
            {
                disTimer.Stop();
                code = "";
                tbRemainingTime.Text = "Код не действителен. Запросите повторную отправку кода";

            }
            else
            {
                tbRemainingTime.Text = "Код перестанет быть действительным через " + countTime;
            }
            countTime--;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void tbCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login();
            }
        }

        private void Login()
        {
            if (code != "")
            {
                if(tbCode.Text == code)
                {
                    disTimer.Stop();
                    tbRemainingTime.Text = "";
                    code = "";
                    Employees employee = Base.baseDate.Employees.FirstOrDefault(x => x.nomer == tbNomer.Text && x.password == pbPassword.Password);
                    if (employee != null)
                    {
                        MessageBox.Show("Вы успешно авторизовались с ролью " + employee.Roles.role);
                    }
                    else
                    {
                        MessageBox.Show("Сотрудник с таким номером и паролем не найден!");
                    }
                }
                else
                {
                    MessageBox.Show("Код введён не верно!");
                }
            }
            else
            {
                MessageBox.Show("Код утратил свою действительность!");
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GetNewCode();
        }
        private void GetNewCode()
        {
            Employees employee = Base.baseDate.Employees.FirstOrDefault(x => x.nomer == tbNomer.Text && x.password == pbPassword.Password);
            if (employee != null)
            {
                Random rand = new Random();
                code = "";
                for (int i = 0; i < 8; i++)
                {
                    int j = rand.Next(3); // Выбор 0 - число; 1 - буква; 2 - спецсимвол
                    if (j == 0)
                    {
                        code = code + rand.Next(9).ToString();
                    }
                    else if (j == 1)
                    {
                        int l = rand.Next(2); // Выбор 0 - заглавная; 1 - маленькая буква
                        if (l == 0)
                        {
                            code = code + (char)rand.Next('A', 'Z' + 1);
                        }
                        else
                        {
                            code = code + (char)rand.Next('a', 'z' + 1);
                        }
                    }
                    else
                    {
                        int l = rand.Next(4); // Выбор диапозона
                        if (l == 0)
                        {
                            code = code + (char)rand.Next(33, 48);
                        }
                        else if (l == 1)
                        {
                            code = code + (char)rand.Next(58, 65);
                        }
                        else if (l == 2)
                        {
                            code = code + (char)rand.Next(91, 97);
                        }
                        else if (l == 3)
                        {
                            code = code + (char)rand.Next(123, 127);
                        }
                    }
                }
                MessageBox.Show("Код для доступа " + code + "\nУ вас будет дано 10 секунд, чтобы ввести код");
                tbCode.IsEnabled = true;
                tbCode.Text = "";
                btnLogin.IsEnabled = true;
                tbCode.Focus();
                countTime = 10;
                disTimer.Start();
            }
            else
            {
                MessageBox.Show("Сотрудник с таким номером и паролем не найден!");
                disTimer.Stop();
                code = "";
                tbRemainingTime.Text = "";
                tbCode.IsEnabled = false;
                tbCode.Text = "";
            }
        }
    }
}
