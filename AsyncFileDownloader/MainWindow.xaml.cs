using AsyncFileDownloader.ViewModel;
using System.Windows;

namespace AsyncFileDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // 일단 코드 비하인드에 쓰는식으로 이렇게 뷰모델 연결
            // 오름차트에서는 어떻게 세팅한건지 아직 잘 모르겠음
            DataContext = new MainWindowViewModel();
        }
    }
}
