﻿using MovieApp.Models;
using MovieApp.ModelView;
using MovieApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Migrations.Model;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;

namespace MovieApp.ViewModel
{

    internal class MovieFrontViewModel:BaseViewModel
    {
        public double currentProfile = 0;

        public Movie ChosenMovie { get; set; }

        private DispatcherTimer timer;

        private BaseViewModel _CurrentPage;
        public BaseViewModel CurrentPage { get { return _CurrentPage; } set { _CurrentPage = value; OnPropertyChanged(); } }

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

        private string _SearchKey;
        public string SearchKey { get { return _SearchKey; } set { _SearchKey = value; OnPropertyChanged(); } }

        private string _MovieImage;
        public string MovieImage { get { return _MovieImage; } set { _MovieImage = value; OnPropertyChanged(); } }

        private Movie _SelectedMovie;
        public Movie SelectedMovie { get { return _SelectedMovie; } set { _SelectedMovie = value; OnPropertyChanged(); } }

        private ObservableCollection<Movie> _MovieSet;
        public ObservableCollection<Movie> MovieSet
        {
            get { return _MovieSet; }
            set
            {
                _MovieSet = value;
                OnPropertyChanged();
            }
        }

        public ICommand SliderChangedCommand { get; set; }
        public ICommand PreviousImageCommand { get; set; }
        public ICommand NextImageCommand { get; set; }
        public ICommand ToLoginCommand { get; set; }
        public ICommand GetImage1Command { get; set; }
        public ICommand GetImage2Command { get; set; }
        public ICommand GetImage3Command { get; set; }
        public ICommand GetImage4Command { get; set; }
        public ICommand GetImage5Command { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand MovieClickCommand { get; set; }
        public ICommand GridMouseEnterCommand { get; set; }
        public ICommand GridMouseLeaveCommand { get; set; }
        public ICommand LoadMovieCommand { get; set; }


        public MovieFrontViewModel()
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
            MovieSet = new ObservableCollection<Movie>();

            DateTime limitDate = DateTime.Parse("03/01/2024");
            var countMovie = DataProvider.Ins.DB.Movies.Where(x => x.release_date < DateTime.Today && x.release_date >= limitDate && x.status == true);

            foreach (var i in countMovie)
            {
                Movie movie = new Movie();
                movie.id = i.id;
                movie.name = i.name;
                movie.duration = i.duration;
                movie.certification = i.certification;
                movie.rating = i.rating;
                movie.release_date = i.release_date;
                movie.status = i.status;
                movie.description = i.description;
                movie.image = $"{AppDomain.CurrentDomain.BaseDirectory}/Images/{i.id}.jpg";    
                MovieSet.Add(movie);
            }



            // Change image slider after 3 seconds
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += Timer_Tick;

            timer.Start();

            LoadMovieCommand = new RelayCommand<Window>((p) => { return true; }, (p) => {  LoadMainPage(); });


            ToLoginCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { LoginWindow loginWindow = new LoginWindow(); loginWindow.ShowDialog(); });
            SliderChangedCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { ExecuteSliderChange(); });
            PreviousImageCommand = new RelayCommand<System.Windows.Controls.Slider>((p) => { return true; }, (p) => { PreviousImage(); });
            NextImageCommand = new RelayCommand<Slider>((p) => { return true; }, (p) => { NextImage(); });
            GetImage1Command = new RelayCommand<object>((p) => { return true; }, (p) => { timer.Stop(); SliderValue = 0; timer.Start(); });
            GetImage2Command = new RelayCommand<object>((p) => { return true; }, (p) => { timer.Stop(); SliderValue = 1; timer.Start(); });
            GetImage3Command = new RelayCommand<object>((p) => { return true; }, (p) => { timer.Stop(); SliderValue = 2; timer.Start(); });
            GetImage4Command = new RelayCommand<object>((p) => { return true; }, (p) => { timer.Stop(); SliderValue = 3; timer.Start(); });
            GetImage5Command = new RelayCommand<object>((p) => { return true; }, (p) => { timer.Stop(); SliderValue = 4; timer.Start(); });
            SearchCommand = new RelayCommand<object>((p) => { return true; }, (p) => { Search(); });
            MovieClickCommand = new RelayCommand<object>((p) => { return true; }, (p) => { ToMovieDetail(); });
            GridMouseEnterCommand = new RelayCommand<object>((p) => { return true; }, (p) => { timer.Stop(); SliderValue = 3; });
            GridMouseLeaveCommand = new RelayCommand<object>((p) => { return true; }, (p) => { SliderValue = 1; timer.Start(); });
        }
        private void LoadMainPage()
        {
           
        }

        public void ToMovieDetail()
        {
            ChosenMovie = SelectedMovie;
            MainWindow mainWindow = new MainWindow();
            var data = mainWindow.DataContext as MainViewModel;
            data.CurrentPage = new MovieDetail();
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

        public void RemoveEllipse()
        {
            OPC1 = 0.25;
            OPC2 = 0.25;
            OPC3 = 0.25;
            OPC4 = 0.25;
            OPC5 = 0.25;
        }
        public void ExecuteSliderChange()
        {

            int index = (int)SliderValue;
            switch (index)
            {
                case 0:

                    Image1Visibility = Visibility.Visible;
                    Image2Visibility = Visibility.Collapsed;
                    Image3Visibility = Visibility.Collapsed;
                    Image4Visibility = Visibility.Collapsed;
                    Image5Visibility = Visibility.Collapsed;
                    RemoveEllipse();
                    OPC1 = 1;
                   
                    currentProfile = 0;
                    break;
                case 1:
                    Image1Visibility = Visibility.Collapsed;
                    Image2Visibility = Visibility.Visible;
                    Image3Visibility = Visibility.Collapsed;
                    Image4Visibility = Visibility.Collapsed;
                    Image5Visibility = Visibility.Collapsed;
                    RemoveEllipse(); OPC2 = 1;
                   
                    currentProfile = 1;
                    break;
                case 2:
                    Image1Visibility = Visibility.Collapsed;
                    Image2Visibility = Visibility.Collapsed;
                    Image3Visibility = Visibility.Visible;
                    Image4Visibility = Visibility.Collapsed;
                    Image5Visibility = Visibility.Collapsed;
                    RemoveEllipse(); OPC3 = 1;
                   
                    currentProfile = 2;
                    break;
                case 3:
                    Image1Visibility = Visibility.Collapsed;
                    Image2Visibility = Visibility.Collapsed;
                    Image3Visibility = Visibility.Collapsed;
                    Image4Visibility = Visibility.Visible;
                    Image5Visibility = Visibility.Collapsed;
                    RemoveEllipse(); OPC4 = 1;
                  
                    currentProfile = 3;
                    break;
                case 4:
                    Image1Visibility = Visibility.Collapsed;
                    Image2Visibility = Visibility.Collapsed;
                    Image3Visibility = Visibility.Collapsed;
                    Image4Visibility = Visibility.Collapsed;
                    Image5Visibility = Visibility.Visible;
                    RemoveEllipse();
                    OPC5 = 1;
                  
                    currentProfile = 4;
                    break;
                default:
                    break;
            }
        }

        public void Search()
        {
            Debug.WriteLine(SearchKey);
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
