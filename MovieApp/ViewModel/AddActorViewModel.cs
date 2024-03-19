using MovieApp.Models;
using MovieApp.ModelView;
using MovieApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MovieApp.ViewModel
{
    internal class AddActorViewModel : BaseViewModel
    {
        private string _ActorDescription;
        public string ActorDescription { get => _ActorDescription; set { _ActorDescription = value; OnPropertyChanged(); } }

        private string _ActorName;
        public string ActorName { get => _ActorName; set { _ActorName = value; OnPropertyChanged(); } }

        private MovieDataList _SelectedMovie;
        public MovieDataList SelectedMovie { get => _SelectedMovie; set { _SelectedMovie = value; OnPropertyChanged(); } }

        private ObservableCollection<MovieDataList> _MovieList;
        public ObservableCollection<MovieDataList> MovieList { get => _MovieList; set { _MovieList = value; OnPropertyChanged(); } }

        public ICommand SaveActorCommand { get; set; }
        public ICommand LoadAddActorCommand { get; set; }
        public AddActorViewModel()
        {
            SaveActorCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { SaveActor(p); });
            LoadAddActorCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { LoadPage(); });
        }
        private void LoadPage()
        {
            ActorDescription = ""; ActorName = ""; SelectedMovie = null;

            var movies = DataProvider.Ins.DB.Movies;

            MovieList = new ObservableCollection<MovieDataList>();
            foreach (var movie in movies)
            {
                MovieList.Add(new MovieDataList() { Movie = movie });
            }
        }

        private void SaveActor(Window p)
        {
            string maxId = DataProvider.Ins.DB.Actors
                        .Select(d => d.id)
                        .OrderByDescending(id => id)
                        .FirstOrDefault();

            // Extract the numeric part of the id and increment it
            int nextIdNumber = 1;
            if (!string.IsNullOrEmpty(maxId) && maxId.Length > 2 && int.TryParse(maxId.Substring(1), out int maxNumericId))
            {
                nextIdNumber = maxNumericId + 1;
            }
            string newId = nextIdNumber < 100 ? $"A{nextIdNumber:000}" : $"A{nextIdNumber}";

            if (ActorDescription == "" || ActorName == "" || SelectedMovie == null)
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
                    Actor newAct = new Actor() { id = newId, name = ActorName, description = ActorDescription, movie_id = SelectedMovie.Movie.id };
                    DataProvider.Ins.DB.Actors.Add(newAct);
                    DataProvider.Ins.DB.SaveChanges();
                    MessageBox.Show("Thêm thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    MovieAttendant movieAttendant = new MovieAttendant();
                    var maData = movieAttendant.DataContext as MovieAttendantViewModel;

                    var ActorMovieData = DataProvider.Ins.DB.Actors
                                  .Join(DataProvider.Ins.DB.Movies,
                                         Actor => Actor.movie_id,
                                         movie => movie.id,
                                         (Actor, movie) => new MovieActor
                                         {
                                             ActorInfo = Actor,
                                             MovieName = movie.name
                                         });
                    maData.ListActor = new ObservableCollection<MovieActor>(ActorMovieData);
                    p.Close();
                }
            }
        }
    }
}
