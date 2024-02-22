using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MovieApp.Views;
namespace MovieApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            NavigateToBaseUI();
        }

        private void NavigateToBaseUI()
        {
            ContentArea.Content = new BaseWindow(this);
        }

        public void NavigateToLoginPage()
        {
            ContentArea.Content = new Login(this);
        }

        public void LoginSuccessful()
        {
            // Handle login successful event here
            // For example, navigate back to the base UI
            NavigateToBaseUI();
        }
    }
}