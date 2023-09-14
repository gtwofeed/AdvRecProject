using GrpcWpfClient.ViewModels;
using System.ComponentModel;
using System.Threading.Tasks;
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
            DataContext = new ApplicationViewModel();
            
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                await ((ApplicationViewModel)this.DataContext).GetWorkerStream();
            });
        }
    }
}
