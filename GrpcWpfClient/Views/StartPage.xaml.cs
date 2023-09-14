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
        private BackgroundWorker backgroundWorker;
        public StartPage()
        {
            InitializeComponent();
            DataContext = new ApplicationViewModel();
            backgroundWorker = ((BackgroundWorker)this.FindResource("backgroundWorker"));
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await ((ApplicationViewModel)this.DataContext).GetWorkerStream();
        }
    }
}
