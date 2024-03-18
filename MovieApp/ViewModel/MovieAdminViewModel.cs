using MovieApp.Models;
using MovieApp.ModelView;
using MovieApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MovieApp.ViewModel
{
    internal class MovieAdminViewModel:BaseViewModel
    {
        private Movie _SelectedMovie;
        public Movie SelectedMovie { get => _SelectedMovie; set { _SelectedMovie = value; OnPropertyChanged(); } }

        private ObservableCollection<Movie> _ListMovie { get; set; }
        public ObservableCollection<Movie> ListMovie { get => _ListMovie; set { _ListMovie = value; OnPropertyChanged(); } }


        public ICommand AddMovieCommand { get; set; }
        public ICommand EditMovieCommand { get; set; }
        public ICommand DeleteMovieCommand { get; set; }

        public ICommand LoadMovieCommand { get; set; }


        public MovieAdminViewModel()
        {
            LoadMovieCommand = new RelayCommand<Movie>((p) => { return true; }, (p) => { LoadMovieList(); });

            AddMovieCommand = new RelayCommand<Window>((p) => { return true; }, (p) => {  });
            EditMovieCommand = new RelayCommand<Movie>((p) => { return true; }, (p) => { SelectedMovie = p; MovieInfoWindow movieInfo = new MovieInfoWindow(); movieInfo.ShowDialog(); });
            DeleteMovieCommand = new RelayCommand<Movie>((p) => { return true; }, (p) => { Debug.WriteLine(p.name); });

        }

        private void LoadMovieList()
        {
            var listMovie = DataProvider.Ins.DB.Movies.ToList();
            ListMovie = new ObservableCollection<Movie>(listMovie);
        }

    }
}
