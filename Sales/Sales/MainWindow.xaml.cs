using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using Newtonsoft.Json;

namespace Sales
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        List<Sale> Sales = new List<Sale>();
        public class Sale
        {
            public DateTime DateSale { get; set; }
            public Client Client { get; set; }
            public List<Telephone> Telephones { get; set; }
        }
        public class Client
        {
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string Patronymic { get; set; }
        }
        public class Telephone
        {
            public int Articul { get; set; }
            public string NameTelephone { get; set; }
            public string Category { get; set; }
            public decimal Cost { get; set; }
            public int Count { get; set; }
            public string Manufacturer { get; set; }
        }
        public MainWindow()
        {
            InitializeComponent();

            WebClient getClient = new WebClient();
            getClient.Encoding = Encoding.UTF8;
            string data = getClient.DownloadString("https://localhost:7100/api/Sale/Get?datestart=10%2E02%2E2020&dateend=11%2E02%2E2020");
            Sales = JsonConvert.DeserializeObject<List<Sale>>(data);
            DgParent.ItemsSource = Sales;
        }
    
    }
}
