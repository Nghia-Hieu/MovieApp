using MovieApp.ModelView;
using MovieApp.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MovieApp.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public bool isLoaded = false;
        public double currentProfile = 0;
        private DispatcherTimer timer;

        private double _SliderValue;
        public double SliderValue { get { return _SliderValue; } set { _SliderValue = value; OnPropertyChanged(); } }

        private Visibility _Image1Visibility;
        public Visibility Image1Visibility { get { return _Image1Visibility; } set { _Image1Visibility = value; OnPropertyChanged(); } }

        private Visibility _Image2Visibility;
        public Visibility Image2Visibility { get { return _Image2Visibility; } set { _Image2Visibility = value; OnPropertyChanged(); } }

        private Visibility _Image3Visibility;
        public Visibility Image3Visibility { get { return _Image3Visibility; } set { _Image3Visibility = value; OnPropertyChanged(); } }

        private Visibility _Image4Visibility;
        public Visibility Image4Visibility { get { return _Image4Visibility; } set { _Image4Visibility = value; OnPropertyChanged(); } }

        private Visibility _Image5Visibility;
        public Visibility Image5Visibility { get { return _Image5Visibility; } set { _Image5Visibility = value; OnPropertyChanged(); } }

        private double _OPC1;
        public double OPC1 { get { return _OPC1; } set { _OPC1 = value; OnPropertyChanged(); } }

        private double _OPC2;
        public double OPC2 { get { return _OPC2; } set { _OPC2 = value; OnPropertyChanged(); } }

        private double _OPC3;
        public double OPC3 { get { return _OPC3; } set { _OPC3 = value; OnPropertyChanged(); } }

        private double _OPC4;
        public double OPC4 { get { return _OPC4; } set { _OPC4 = value; OnPropertyChanged(); } }

        private double _OPC5;
        public double OPC5 { get { return _OPC5; } set { _OPC5 = value; OnPropertyChanged(); } }


        public ICommand SliderChangedCommand { get; set; }

        public ICommand PreviousImageCommand { get; set; }

        public ICommand NextImageCommand { get; set; }

        public ICommand ToLoginCommand { get; set; }
        public ICommand GetImage1Command { get; set; }
        public ICommand GetImage2Command { get; set; }
        public ICommand GetImage3Command { get; set; }
        public ICommand GetImage4Command { get; set; }
        public ICommand GetImage5Command { get; set; }

        public MainViewModel()
        {
            Image1Visibility = Visibility.Visible;
            Image2Visibility = Visibility.Collapsed;
            Image3Visibility = Visibility.Collapsed;
            Image4Visibility = Visibility.Collapsed;
            Image5Visibility = Visibility.Collapsed;
            SliderValue = 0;
            OPC1 = 1;
            OPC2 = 0.25;
            OPC3 = 0.25;
            OPC4 = 0.25;
            OPC5 = 0.25;
            if (isLoaded)
            {
                isLoaded = true;
                MainWindow mainWindow = new MainWindow();
                mainWindow.ShowDialog();
            }

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += Timer_Tick;

            timer.Start();

            ToLoginCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { LoginWindow loginWindow = new LoginWindow(); loginWindow.ShowDialog(); });
            SliderChangedCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { ExecuteSliderChange(); });
            PreviousImageCommand = new RelayCommand<System.Windows.Controls.Slider>((p) => { return true; }, (p) => { PreviousImage(); });
            NextImageCommand = new RelayCommand<Slider>((p) => { return true; }, (p) => { NextImage(); });
            GetImage1Command = new RelayCommand<object>((p) => { return true; }, (p) => { timer.Stop(); SliderValue = 0; timer.Start(); });
            GetImage2Command = new RelayCommand<object>((p) => { return true; }, (p) => { timer.Stop(); SliderValue = 1; timer.Start(); });
            GetImage3Command = new RelayCommand<object>((p) => { return true; }, (p) => { timer.Stop(); SliderValue = 2; timer.Start(); });
            GetImage4Command = new RelayCommand<object>((p) => { return true; }, (p) => { timer.Stop(); SliderValue = 3; timer.Start(); });
            GetImage5Command = new RelayCommand<object>((p) => { return true; }, (p) => { timer.Stop(); SliderValue = 4; timer.Start(); });
        }

        public void NextImage()
        {
            currentProfile++;
            if (currentProfile > 4)
                currentProfile = 0;
            SliderValue = currentProfile;

        }

        public void PreviousImage()
        {
            currentProfile--;
            if (currentProfile < 0)
                currentProfile = 4;
            SliderValue = currentProfile;
        }

        public void removeEllipse()
        {
            OPC1 = 0.25;
            OPC2 = 0.25;
            OPC3 = 0.25;
            OPC4 = 0.25;
            OPC5 = 0.25;
        }
        public void ExecuteSliderChange()
        {
            Debug.WriteLine("Here");

            int index = (int)SliderValue;
            switch (index)
            {
                case 0:
                    Debug.WriteLine("0");

                    Image1Visibility = Visibility.Visible;
                    Image2Visibility = Visibility.Collapsed;
                    Image3Visibility = Visibility.Collapsed;
                    Image4Visibility = Visibility.Collapsed;
                    Image5Visibility = Visibility.Collapsed;
                    removeEllipse(); OPC1 = 1;
                    //removeEllipse();
                    //Ellipse ect1 = (Ellipse)ec1.Template.FindName("ecb1", ec1);
                    //if (ect1 != null)
                    //{
                    //    ect1.Opacity = 1;
                    //}
                    currentProfile = 0;
                    break;
                case 1:
                    Debug.WriteLine("1");

                    Image1Visibility = Visibility.Collapsed;
                    Image2Visibility = Visibility.Visible;
                    Image3Visibility = Visibility.Collapsed;
                    Image4Visibility = Visibility.Collapsed;
                    Image5Visibility = Visibility.Collapsed;
                    removeEllipse(); OPC2 = 1;

                    //removeEllipse();
                    //Ellipse ect2 = (Ellipse)ec2.Template.FindName("ecb2", ec2);
                    //if (ect2 != null)
                    //{
                    //    ect2.Opacity = 1;
                    //}
                    currentProfile = 1;
                    break;
                case 2:
                    Debug.WriteLine("2");

                    Image1Visibility = Visibility.Collapsed;
                    Image2Visibility = Visibility.Collapsed;
                    Image3Visibility = Visibility.Visible;
                    Image4Visibility = Visibility.Collapsed;
                    Image5Visibility = Visibility.Collapsed;
                    removeEllipse(); OPC3 = 1;

                    //removeEllipse();
                    //Ellipse ect3 = (Ellipse)ec3.Template.FindName("ecb3", ec3);
                    //if (ect3 != null)
                    //{
                    //    ect3.Opacity = 1;
                    //}
                    currentProfile = 2;
                    break;
                case 3:
                    Debug.WriteLine("3");

                    Image1Visibility = Visibility.Collapsed;
                    Image2Visibility = Visibility.Collapsed;
                    Image3Visibility = Visibility.Collapsed;
                    Image4Visibility = Visibility.Visible;
                    Image5Visibility = Visibility.Collapsed;
                    removeEllipse();  OPC4 = 1;

                    //removeEllipse();
                    //Ellipse ect4 = (Ellipse)ec4.Template.FindName("ecb4", ec4);
                    //if (ect4 != null)
                    //{
                    //    ect4.Opacity = 1;
                    //}
                    currentProfile = 3;
                    break;
                case 4:
                    Debug.WriteLine("4");

                    Image1Visibility = Visibility.Collapsed;
                    Image2Visibility = Visibility.Collapsed;
                    Image3Visibility = Visibility.Collapsed;
                    Image4Visibility = Visibility.Collapsed;
                    Image5Visibility = Visibility.Visible;
                    removeEllipse();
                    OPC5 = 1;

                    //removeEllipse();
                    //Ellipse ect5 = (Ellipse)ec5.Template.FindName("ecb5", ec5);
                    //if (ect5 != null)
                    //{
                    //    ect5.Opacity = 1;
                    //}
                    currentProfile = 4;
                    break;
                default:
                    Debug.WriteLine("bre");

                    break;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            currentProfile++;
            if (currentProfile > 4)
            {
                currentProfile = 0;
            }
            SliderValue = currentProfile;
        }
    }
}
