using System.Windows.Controls;

namespace Satur8.UI
{
    /// <summary>
    /// Логика взаимодействия для SaturatorView.xaml
    /// </summary>
    public partial class SaturatorView : UserControl
    {
        public SaturatorView()
        {
            InitializeComponent();
            DataContext = new SaturatorViewModel();
        }

    }
}