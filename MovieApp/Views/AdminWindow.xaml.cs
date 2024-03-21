using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MovieApp.Views
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            //videoa.Source = new Uri(System.IO.Directory.GetCurrentDirectory().ToString() + @"/Images/demovideo.mp4");
            FilmFrame.NavigationService.Navigate(new MovieAdmin());
            AttendantFrame.NavigationService.Navigate(new MovieAttendant());
            ShowFrame.NavigationService.Navigate(new MovieShowTime());
            VoucherFrame.NavigationService.Navigate(new MovieVoucher());
        }
    }
}
