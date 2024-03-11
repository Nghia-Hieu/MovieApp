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

        private DateTime _Date1;
        public DateTime Date1 { get { return _Date1; } set { _Date1 = value; OnPropertyChanged(); } }

        private DateTime _Date2;
        public DateTime Date2 { get { return _Date2; } set { _Date2 = value; OnPropertyChanged(); } }

        private DateTime _Date3;
        public DateTime Date3 { get { return _Date3; } set { _Date3 = value; OnPropertyChanged(); } }

        private ObservableCollection<ShowTime> _ListShowTime1;
        public ObservableCollection<ShowTime> ListShowTime1 { get { return _ListShowTime1; } set { _ListShowTime1 = value; OnPropertyChanged(); } }

        private ObservableCollection<ShowTime> _ListShowTime2;
        public ObservableCollection<ShowTime> ListShowTime2 { get { return _ListShowTime2; } set { _ListShowTime2 = value; OnPropertyChanged(); } }

        private ObservableCollection<ShowTime> _ListShowTime3;
        public ObservableCollection<ShowTime> ListShowTime3 { get { return _ListShowTime3; } set { _ListShowTime3 = value; OnPropertyChanged(); } }

        private ShowTime _SelectedShowTime;
        public ShowTime SelectedShowTime { get { return _SelectedShowTime; } set { _SelectedShowTime = value; OnPropertyChanged(); } }

        private ShowTime _SelectedShowTime1;
        public ShowTime SelectedShowTime1 { get { return _SelectedShowTime1; } set { _SelectedShowTime1 = value; OnPropertyChanged(); } }

        private ShowTime _SelectedShowTime2;
        public ShowTime SelectedShowTime2 { get { return _SelectedShowTime2; } set { _SelectedShowTime2 = value; OnPropertyChanged(); } }

        public ICommand LoadMovieDetailCommand { get; set; }
        public ICommand ShowTimeClickCommand { get; set; }
       



        public MovieDetailViewModel()
        {
            //SelectedShowTime = new ShowTime();

            LoadMovieDetailCommand = new RelayCommand<object>((p) => { return true; }, (p) => { LoadingMovie(); }) ;
            ShowTimeClickCommand = new RelayCommand<ShowTime>((p) => { return true; }, (p) => { ShowUp(p); });
           

        }

        public void ShowUp(ShowTime show)
        {
            Debug.WriteLine(show.time + " " + show.date);
            SelectedShowTime = show;
            TicketWindow ticketWindow = new TicketWindow();
            
            ticketWindow.ShowDialog();   
        }
        

        private void LoadingMovie()
        {
            Debug.WriteLine("Load Detail");

            MovieFront movieFront = new MovieFront();

            var movieFrontData = movieFront.DataContext as MovieFrontViewModel;

            //Get Movie Actor
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
                MovieActors.Add(actor);
            }
            
            // Get Movie Genres
            CurrentMovieGenres = new ObservableCollection<MovieGenre>();
            var genres = DataProvider.Ins.DB.Movies.Join(DataProvider.Ins.DB.MovieGenres, t1 => t1.id, t2 => t2.movie_id, (t1, t2)=> new {Id = t1.id, Genre_name = t2.genre_name}).Where(x => x.Id == MovieInfo.id);
            foreach (var i in genres)
            {
                MovieGenre genre = new MovieGenre();
                genre.genre_name = i.Genre_name;
                genre.movie_id = i.Id;

                CurrentMovieGenres.Add(genre);
            }

            //Get movie ShowTime
           
            
            ListShowTime1 = new ObservableCollection<ShowTime>();
            ListShowTime2 = new ObservableCollection<ShowTime>();
            ListShowTime3 = new ObservableCollection<ShowTime>();
            var todayDate = DateTime.Today;
            Date1 = todayDate;
            Date2 = todayDate.AddDays(1);
            Date3 = todayDate.AddDays(2);
            var listShowTime1 = DataProvider.Ins.DB.ShowTimes.Where(x=> x.date == Date1 && x.movie_id == MovieInfo.id);
            var listShowTime2 = DataProvider.Ins.DB.ShowTimes.Where(x => x.date == Date2 && x.movie_id == MovieInfo.id);
            var listShowTime3 = DataProvider.Ins.DB.ShowTimes.Where(x => x.date == Date3 && x.movie_id == MovieInfo.id);
            
            ListShowTime1 = new ObservableCollection<ShowTime>(listShowTime1);

            ListShowTime3 = new ObservableCollection<ShowTime>(listShowTime3);

            ListShowTime2 = new ObservableCollection<ShowTime>(listShowTime2);

        }
    }
}
