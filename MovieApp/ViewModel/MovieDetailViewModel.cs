using MovieApp.Models;
using MovieApp.ModelView;
using MovieApp.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace MovieApp.ViewModel
{
    internal class MovieDetailViewModel : BaseViewModel
    {
        private Movie _MovieInfo;
        public Movie MovieInfo { get { return _MovieInfo; } set { _MovieInfo = value; OnPropertyChanged(); } }
        public ICommand LoadCommand { get; set; }

        public MovieDetailViewModel() 
        {
            
            LoadCommand = new RelayCommand<object>((p) => { return true; }, (p) => { OnLoad(); }) ;         
        }

        private void OnLoad()
        {
            MovieFront movieFront = new MovieFront();
            var movieFrontData = movieFront.DataContext as MovieFrontViewModel;

            MovieInfo = movieFrontData.SelectedMovie;
        }
    }
}
