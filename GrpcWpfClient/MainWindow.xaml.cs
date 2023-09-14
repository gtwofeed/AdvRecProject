using System.Windows;
using GrpcWpfClient.ViewModels;
using GrpcWpfClient.Views;

namespace GrpcWpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();            
            mainFrame.Navigate(new StartPage());
        }
    }
}
