using MovieApp.Models;
using MovieApp.ModelView;
using MovieApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MovieApp.ViewModel
{
    public class MovieDirector
    {
        public Director DirectorInfo { get; set; }
        public string MovieName { get; set; }
    }
    public class MovieActor
    {
        public Actor ActorInfo { get; set; }
        public string MovieName { get; set; }
        public string MovieId { get; set; }

    }
    internal class MovieAttendantViewModel : BaseViewModel
    {
        private MovieDirector _SelectedDirector;
        public MovieDirector SelectedDirector { get => _SelectedDirector; set { _SelectedDirector = value; OnPropertyChanged(); } }

        private MovieActor _SelectedActor;
        public MovieActor SelectedActor { get => _SelectedActor; set { _SelectedActor = value; OnPropertyChanged(); } }


        private ObservableCollection<MovieDirector> _ListDirector;
        public ObservableCollection<MovieDirector> ListDirector { get => _ListDirector; set { _ListDirector = value; OnPropertyChanged(); } }

        private ObservableCollection<MovieActor> _ListActor;
        public ObservableCollection<MovieActor> ListActor { get => _ListActor; set { _ListActor = value; OnPropertyChanged(); } }
        public ICommand LoadAttendantCommand { get; set; }
        public ICommand NewDirectorCommand { get; set; }
        public ICommand EditDirectorCommand { get; set; }
        public ICommand DeleteDirectorCommand { get; set; }
        public ICommand NewActorCommand { get; set; }
        public ICommand EditActorCommand { get; set; }
        public ICommand DeleteActorCommand { get; set; }

        public MovieAttendantViewModel()
        {
            LoadAttendantCommand = new RelayCommand<object>((p) => { return true; }, (p) => { LoadAttendant(); });
            NewDirectorCommand = new RelayCommand<object>((p) => { return true; }, (p) => { NewDirector(); });
            EditDirectorCommand = new RelayCommand<MovieDirector>((p) => { return true; }, (p) => { EditDirector(p); });
            DeleteDirectorCommand = new RelayCommand<MovieDirector>((p) => { return true; }, (p) => { DeleteDirector(p); });

            NewActorCommand = new RelayCommand<object>((p) => { return true; }, (p) => { NewActor(); });
            EditActorCommand = new RelayCommand<MovieActor>((p) => { return true; }, (p) => { EditActor(p); });
            DeleteActorCommand = new RelayCommand<MovieActor>((p) => { return true; }, (p) => { DeleteActor(p); });

        }

        private void LoadAttendant()
        {
            var directorMovieData = DataProvider.Ins.DB.Directors
                                   .Join(DataProvider.Ins.DB.Movies,
                                          director => director.movie_id,
                                          movie => movie.id,
                                          (director, movie) => new MovieDirector
                                          {
                                              DirectorInfo = director,
                                              MovieName = movie.name,
                                          });
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
            ListDirector = new ObservableCollection<MovieDirector>(directorMovieData);
            ListActor = new ObservableCollection<MovieActor>(actorMovieData);
        }

        private void EditDirector(MovieDirector p)
        {
            Debug.WriteLine(p.DirectorInfo.name);
            SelectedDirector = p;
            DirectorInfoWindow directorInfoWindow = new DirectorInfoWindow();
            directorInfoWindow.ShowDialog();
        }

        private void DeleteDirector(MovieDirector p)
        {
            MessageBoxResult result = MessageBox.Show($"Bạn có chắc muốn xóa {p.DirectorInfo.name}", "Cảnh báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes) {
                var deleteData = DataProvider.Ins.DB.Directors.Where(d => d.id == p.DirectorInfo.id).FirstOrDefault();
                if(deleteData != null)
                {
                    DataProvider.Ins.DB.Directors.Remove(deleteData);
                    DataProvider.Ins.DB.SaveChanges();
                    MessageBox.Show("Xoá thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
             

                    var directorMovieData = DataProvider.Ins.DB.Directors
                                  .Join(DataProvider.Ins.DB.Movies,
                                         director => director.movie_id,
                                         movie => movie.id,
                                         (director, movie) => new MovieDirector
                                         {
                                             DirectorInfo = director,
                                             MovieName = movie.name
                                         });
                    ListDirector = new ObservableCollection<MovieDirector>(directorMovieData);
                }
            }
        }

        private void NewDirector()
        {
            AddDirectorWindow addDirectorWindow = new AddDirectorWindow();

            addDirectorWindow.ShowDialog();
        }

        private void NewActor()
        {
            AddActorWindow addActorWindow = new AddActorWindow();

            addActorWindow.ShowDialog();
        }

        private void EditActor(MovieActor p)
        {
            SelectedActor = p;
            ActorInfoWindow actorInfoWindow = new ActorInfoWindow();
            actorInfoWindow.ShowDialog();
        }

        private void DeleteActor(MovieActor p)
        {
            MessageBoxResult result = MessageBox.Show($"Bạn có chắc muốn xóa {p.ActorInfo.name}", "Cảnh báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var deleteData = DataProvider.Ins.DB.Actors.Where(d => d.id == p.ActorInfo.id).FirstOrDefault();
                if (deleteData != null)
                {
                    DataProvider.Ins.DB.Actors.Remove(deleteData);
                    DataProvider.Ins.DB.SaveChanges();
                    MessageBox.Show("Xoá thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);


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
                    ListActor = new ObservableCollection<MovieActor>(actorMovieData);
                }
            }
        }
    }
}
