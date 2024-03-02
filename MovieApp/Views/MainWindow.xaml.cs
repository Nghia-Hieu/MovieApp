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
using System.Windows.Threading;

namespace MovieApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class CarouselItem
    {
        public string ImagePath { get; set; }
    }

    public partial class MainWindow : Window
    {
        private int currentProfile = 0;
        private DispatcherTimer timer;


        public MainWindow()
        {
            InitializeComponent();

            var uriSource = new Uri(@"/MovieApp;component/Images/01.jpg", UriKind.Relative);
            var uriSource1 = new Uri(@"/MovieApp;component/Images/02.jpg", UriKind.Relative);
            var uriSource2 = new Uri(@"/MovieApp;component/Images/06.jpg", UriKind.Relative);

            ImageGrid.Source = new BitmapImage(uriSource1);
            ImageGrid2.Source = new BitmapImage(uriSource);
            ImageGrid1.Source = new BitmapImage(uriSource2);

            DataContext = this;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += Timer_Tick;

            timer.Start();
        }


        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int index = (int)slider.Value;

            switch (index)
            {
                case 0:
                    grid0.Visibility = Visibility.Visible;
                    grid1.Visibility = Visibility.Collapsed;
                    grid2.Visibility = Visibility.Collapsed;
                    grid3.Visibility = Visibility.Collapsed;
                    grid4.Visibility = Visibility.Collapsed;
                    removeEllipse();
                    Ellipse ect1 = (Ellipse)ec1.Template.FindName("ecb1", ec1);
                    if (ect1 != null)
                    {
                        ect1.Opacity = 1;
                    }
                    currentProfile = 0;
                    break;
                case 1:
                    grid0.Visibility = Visibility.Collapsed;
                    grid1.Visibility = Visibility.Visible;
                    grid2.Visibility = Visibility.Collapsed;
                    grid3.Visibility = Visibility.Collapsed;
                    grid4.Visibility = Visibility.Collapsed;
                    removeEllipse();
                    Ellipse ect2 = (Ellipse)ec2.Template.FindName("ecb2", ec2);
                    if (ect2 != null)
                    {
                        ect2.Opacity = 1;
                    }
                    currentProfile = 1;
                    break;
                case 2:
                    grid0.Visibility = Visibility.Collapsed;
                    grid1.Visibility = Visibility.Collapsed;
                    grid2.Visibility = Visibility.Visible;
                    grid3.Visibility = Visibility.Collapsed;
                    grid4.Visibility = Visibility.Collapsed; 
                    removeEllipse();
                    Ellipse ect3 = (Ellipse)ec3.Template.FindName("ecb3", ec3);
                    if (ect3 != null)
                    {
                        ect3.Opacity = 1;
                    }
                    currentProfile = 2;
                    break;
                case 3:
                    grid0.Visibility = Visibility.Collapsed;
                    grid1.Visibility = Visibility.Collapsed;
                    grid2.Visibility = Visibility.Collapsed;
                    grid3.Visibility = Visibility.Visible;
                    grid4.Visibility = Visibility.Collapsed;
                    removeEllipse();
                    Ellipse ect4 = (Ellipse)ec4.Template.FindName("ecb4", ec4);
                    if (ect4 != null)
                    {
                        ect4.Opacity = 1;
                    }
                    currentProfile = 3;
                    break;
                case 4:
                    grid0.Visibility = Visibility.Collapsed;
                    grid1.Visibility = Visibility.Collapsed;
                    grid2.Visibility = Visibility.Collapsed;
                    grid3.Visibility = Visibility.Collapsed;
                    grid4.Visibility = Visibility.Visible;
                    removeEllipse();
                    Ellipse ect5 = (Ellipse)ec5.Template.FindName("ecb5", ec5);
                    if (ect5 != null)
                    {
                        ect5.Opacity = 1;
                    }
                    currentProfile = 4;
                    break;
                default:
                    break;
            }
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            currentProfile--;
            if (currentProfile < 0)
            {
                currentProfile = 4;
            }
            slider.Value = currentProfile;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            currentProfile++;
            if (currentProfile > 4)
            {
                currentProfile = 0;
            }
            slider.Value = currentProfile;
        }

        private void removeEllipse()
        {
            Ellipse e1 = (Ellipse)ec1.Template.FindName("ecb1", ec1);
            Ellipse e2 = (Ellipse)ec2.Template.FindName("ecb2", ec2);
            Ellipse e3 = (Ellipse)ec3.Template.FindName("ecb3", ec3);
            Ellipse e4 = (Ellipse)ec4.Template.FindName("ecb4", ec4);
            Ellipse e5 = (Ellipse)ec5.Template.FindName("ecb5", ec5);

            e1.Opacity = 0.25;
            e2.Opacity = 0.25;
            e3.Opacity = 0.25;
            e4.Opacity = 0.25;
            e5.Opacity = 0.25;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            currentProfile++;
            if (currentProfile > 4)
            {
                currentProfile = 0;
            }
            slider.Value = currentProfile;
        }

        private void ec4_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            slider.Value = 3;
            timer.Start();
        }

        private void ec5_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            slider.Value = 4;
            timer.Start();

        }

        private void ec3_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            slider.Value = 2;
            timer.Start();

        }

        private void ec2_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            slider.Value = 1;
            timer.Start();

        }

        private void ec1_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            slider.Value = 0;
            timer.Start();

        }
    }
}
