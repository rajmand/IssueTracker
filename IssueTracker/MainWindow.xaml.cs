using FirstFloor.ModernUI.Windows.Controls;
using System.Windows;

namespace IssueTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Topmost = ((App)Application.Current).AlwaysOnTop;
        }
    }
}
