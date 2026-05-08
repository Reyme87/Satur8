using Satur8.Persistence;
using System.Windows;

namespace Satur8.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SaturatorDbContext _dbContext;
        public MainWindow(SaturatorDbContext dbContext)
        {
            _dbContext = dbContext;
            InitializeComponent();
        }
    }
}