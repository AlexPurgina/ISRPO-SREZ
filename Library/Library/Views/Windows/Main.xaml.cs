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
using Library.Views.Peges;

namespace Library.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class Main : Window
    {
       
            public Main()
            {
                InitializeComponent();
            }
            string token { get; set; }
            public Main(string token)
            {
                InitializeComponent();
                this.token = token;
                MainFrame.Content = new ReadersList(token);
            }
        
    }
}
