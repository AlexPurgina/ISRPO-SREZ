using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
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
using Library.Views.Windows;

namespace Library
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        static string sha256(string inputString)
        {
            var crypt = new SHA256Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(inputString));
            foreach (byte theByte in crypto)
            {
                hash += theByte.ToString("x2");
            }
            return hash;
        }
        public async void btnEnter_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                using (HttpClient httpClient = new HttpClient { BaseAddress = new Uri(Properties.Settings.Default.BaseAddress) })
                {
                    var content = new StringContent("", Encoding.UTF8, "applocation/json");
                    HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"/Login?login={tbLogin.Text}&password={sha256(pbPassword.Password)}", content);
                    string token = httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();


                    if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        Main main = new Main(token);
                        main.Show();
                        this.Close();
                    }
                    else MessageBox.Show("Логин или пароль не верный");


                }
            }
            catch (Exception)
            {

                MessageBox.Show("Пользователь не найден");
            }
        }

            private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                TbPassword.Text = pbPassword.Password;
                TbPassword.Visibility = Visibility.Visible;
                pbPassword.Visibility = Visibility.Collapsed;
            }

            private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            {
                TbPassword.Visibility = Visibility.Collapsed;
                pbPassword.Visibility = Visibility.Visible;
            }
        
        
    }
}
