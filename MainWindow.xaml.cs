using System.Windows;
using WpfDimensionSample.ViewModels;

namespace WpfDimensionSample
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
