using MovieApp.Models;
using MovieApp.ModelView;
using MovieApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MovieApp.ViewModel
{
    internal class ActorInfoViewModel : BaseViewModel
    {
        private string _ActorDescription;
        public string ActorDescription { get => _ActorDescription; set { _ActorDescription = value; OnPropertyChanged(); } }

        private string _ActorName;
        public string ActorName { get => _ActorName; set { _ActorName = value; OnPropertyChanged(); } }

        private string _ActorId;
        public string ActorId { get => _ActorId; set { _ActorId = value; OnPropertyChanged(); } }

        private MovieDataList _SelectedMovie;
        public MovieDataList SelectedMovie { get => _SelectedMovie; set { _SelectedMovie = value; OnPropertyChanged(); } }

        private ObservableCollection<MovieDataList> _MovieList;
        public ObservableCollection<MovieDataList> MovieList { get => _MovieList; set { _MovieList = value; OnPropertyChanged(); } }

        public ICommand UpdateActorCommand { get; set; }
        public ICommand LoadActorInfoCommand { get; set; }
        public ActorInfoViewModel() 
        {
            UpdateActorCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { UpdateActor(p); });
            LoadActorInfoCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { LoadActorInfo(); });
        }

        private void LoadActorInfo()
        {
            ActorDescription = ""; ActorName = ""; SelectedMovie = new MovieDataList();
            SelectedMovie.Movie = new Movie();

            var movies = DataProvider.Ins.DB.Movies;

            MovieList = new ObservableCollection<MovieDataList>();
            foreach (var movie in movies)
            {
                MovieList.Add(new MovieDataList() { Movie = movie });
            }
            MovieAttendant movieAttendant = new MovieAttendant();
            var maData = movieAttendant.DataContext as MovieAttendantViewModel;
            if (maData != null)
            {
                if (maData.SelectedActor != null)
                {
                    ActorName = maData.SelectedActor.ActorInfo.name;
                    ActorDescription = maData.SelectedActor.ActorInfo.description;
                    ActorId = maData.SelectedActor.ActorInfo.id;
                    SelectedMovie = MovieList.FirstOrDefault(m => m.Movie.name == maData.SelectedActor.MovieName);
                }
            }
        }

        private void UpdateActor(Window p)
        {
            if (ActorDescription == "" || ActorName == "")
                MessageBox.Show("Vui lòng điền đầy đủ thông tin", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
            {
                if (ActorName.Length > 100 || ActorName.Length < 2)
                {
                    MessageBox.Show("Tên không hợp lệ", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (ActorDescription.Length > 2000)
                {
                    MessageBox.Show("Mô tả quá dài", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    var actorInfo = DataProvider.Ins.DB.Actors.Where(a => a.id == ActorId).FirstOrDefault();
                    if (actorInfo != null)
                    {
                        actorInfo.name = ActorName;
                        actorInfo.description = ActorDescription;
                        actorInfo.movie_id = SelectedMovie.Movie.id;
                        DataProvider.Ins.DB.SaveChanges();
                        MessageBox.Show("Update thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                        MovieAttendant movieAttendant = new MovieAttendant();


                        var maData = movieAttendant.DataContext as MovieAttendantViewModel;

                        var actorMovieData = DataProvider.Ins.DB.Actors
                                  .Join(DataProvider.Ins.DB.Movies,
                                         actor => actor.movie_id,
                                         movie => movie.id,
                                         (actor, movie) => new MovieActor
                                         {
                                             ActorInfo = actor,
                                             MovieName = movie.name,
                                             MovieId = movie.id
                                         });
                        maData.ListActor = new ObservableCollection<MovieActor>(actorMovieData);
                        p.Close();
                    }

                }
            }
        }
    }
}
