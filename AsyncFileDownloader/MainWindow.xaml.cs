using AsyncFileDownloader.ViewModel;
using System.Windows;

namespace AsyncFileDownloader
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            DataContext = new MainWindowViewModel();
        }
    }
}
