using GrpcWpfClient.Models;
using GrpcWpfClient.ViewModels;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GrpcWpfClient.Views
{
    /// <summary>
    /// Логика взаимодействия для StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new ApplicationViewModel();
            await ((ApplicationViewModel)this.DataContext).GetWorkerStream();
        }
    }
}
