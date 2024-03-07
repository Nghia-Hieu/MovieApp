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
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;

namespace MovieApp.ViewModel
{
    
    internal class MovieDetailViewModel : BaseViewModel
    {
        private Movie _MovieInfo;
        public Movie MovieInfo { get { return _MovieInfo; } set { _MovieInfo = value; OnPropertyChanged(); } }

        private Director _MovieDirector;
        public Director MovieDirector { get { return _MovieDirector; } set { _MovieDirector = value; OnPropertyChanged(); } }

        private ObservableCollection<Actor> _MovieActors;
        public ObservableCollection<Actor> MovieActors { get { return _MovieActors; } set { _MovieActors = value; OnPropertyChanged(); } }

        private ObservableCollection<MovieGenre> _CurrentMovieGenres;
        public ObservableCollection<MovieGenre> CurrentMovieGenres { get { return _CurrentMovieGenres; } set { _CurrentMovieGenres = value; OnPropertyChanged(); } }
        public ICommand LoadMovieDetailCommand { get; set; }

        public MovieDetailViewModel() 
        {

            LoadMovieDetailCommand = new RelayCommand<object>((p) => { return true; }, (p) => { LoadingMovie(); }) ;         
        }

        private void LoadingMovie()
        {
            MovieFront movieFront = new MovieFront();
            var movieFrontData = movieFront.DataContext as MovieFrontViewModel;

            MovieInfo = movieFrontData.SelectedMovie;
            var director = DataProvider.Ins.DB.Directors.Where(c => c.movie_id == MovieInfo.id);

            if(director != null )
            {
                MovieDirector = new Director();
                MovieDirector = director.First();
            }

            MovieActors = new ObservableCollection<Actor>();
            var actors = DataProvider.Ins.DB.Actors.Where(c=>c.movie_id==MovieInfo.id);
            foreach(var i in actors)
            {
                Actor actor = new Actor();
                actor.id = i.id;
                actor.name = i.name;
                actor.description = i.description;
                actor.movie_id = i.movie_id;
                Debug.WriteLine(actor.name);
                MovieActors.Add(actor);
            }

            CurrentMovieGenres = new ObservableCollection<MovieGenre>();
            var genres = DataProvider.Ins.DB.Movies.Join(DataProvider.Ins.DB.MovieGenres, t1 => t1.id, t2 => t2.movie_id, (t1, t2)=> new {Id = t1.id, Genre_name = t2.genre_name}).Where(x => x.Id == MovieInfo.id);
            foreach (var i in genres)
            {
                MovieGenre genre = new MovieGenre();
                genre.genre_name = i.Genre_name;
                genre.movie_id = i.Id;

                CurrentMovieGenres.Add(genre);
            }
        }
    }
}
