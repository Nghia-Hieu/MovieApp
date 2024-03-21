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
    internal class MovieAdminViewModel : BaseViewModel
    {
        private Movie _SelectedMovie;
        public Movie SelectedMovie { get => _SelectedMovie; set { _SelectedMovie = value; OnPropertyChanged(); } }

        private ObservableCollection<Movie> _ListMovie { get; set; }
        public ObservableCollection<Movie> ListMovie { get => _ListMovie; set { _ListMovie = value; OnPropertyChanged(); } }


        public ICommand AddMovieCommand { get; set; }
        public ICommand EditMovieCommand { get; set; }
        public ICommand DeleteMovieCommand { get; set; }
        public ICommand LoadMovieCommand { get; set; }
        public ICommand NewMovieCommand { get; set; }


        public MovieAdminViewModel()
        {
            LoadMovieCommand = new RelayCommand<Movie>((p) => { return true; }, (p) => { LoadMovieList(); });

            AddMovieCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { });
            EditMovieCommand = new RelayCommand<Movie>((p) => { return true; }, (p) => { SelectedMovie = p; MovieInfoWindow movieInfo = new MovieInfoWindow(); movieInfo.ShowDialog(); });
            DeleteMovieCommand = new RelayCommand<Movie>((p) => { return true; }, (p) => { Debug.WriteLine(p.name); DeleteMovie(p); });
            NewMovieCommand = new RelayCommand<Movie>((p) => { return true; }, (p) => { ToNewMovie(); });

        }

        public void LoadMovieList()
        {
            var listMovie = DataProvider.Ins.DB.Movies.ToList();
            ListMovie = new ObservableCollection<Movie>(listMovie);
        }

        private void ToNewMovie()
        {
            AddMovieWindow addMovieWindow = new AddMovieWindow();
            addMovieWindow.ShowDialog();
        }

        private void DeleteMovie(Movie p)
        {
            MessageBoxResult result = MessageBox.Show($"Bạn có chắc muốn xóa phim {p.name}", "Cảnh báo", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                MessageBoxResult resultCont = MessageBox.Show("Xoá phim sẽ bao gồm xóa toàn bộ thông tin ĐẠO DIỄN, DIỄN VIÊN và SUẤT CHIẾU. Bạn có chắc muốn tiếp tục?", "Cảnh báo", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (resultCont == MessageBoxResult.OK)
                {
                    MessageBoxResult resultContCont = MessageBox.Show($"Xác nhận xóa phim {p.name} khỏi hệ thống?", "Cảnh báo", MessageBoxButton.OKCancel, MessageBoxImage.Question);

                    if (resultCont == MessageBoxResult.OK)
                    {
                        var movieShow = DataProvider.Ins.DB.ShowTimes.Where(s => s.movie_id == p.id);

                        if (movieShow != null)
                        {
                            foreach (var show in movieShow)
                            {
                                var movieSeat = DataProvider.Ins.DB.Seats.Where(s => s.show_id == show.id);
                                // Delete seat
                                foreach (var seat in movieSeat)
                                {
                                    DataProvider.Ins.DB.Seats.Remove(seat);
                                }

                                //Delete ShowTime
                                DataProvider.Ins.DB.ShowTimes.Remove(show);
                                System.GC.Collect();
                                System.GC.WaitForPendingFinalizers();
                            }
                            DataProvider.Ins.DB.SaveChanges();
                        }

                        // Delete Director
                        var movieDirector = DataProvider.Ins.DB.Directors.Where(d => d.movie_id == p.id);
                        if (movieDirector != null)
                        {
                            foreach (var director in movieDirector)
                            {
                                DataProvider.Ins.DB.Directors.Remove(director);
                            }
                            DataProvider.Ins.DB.SaveChanges();
                        }

                        // Delete Actor
                        var movieActor = DataProvider.Ins.DB.Actors.Where(a => a.movie_id == p.id);
                        if (movieActor != null)
                        {
                            foreach (var actor in movieActor)
                            {
                                DataProvider.Ins.DB.Actors.Remove(actor);
                            }
                            DataProvider.Ins.DB.SaveChanges();
                        }
                        var movieGenre = DataProvider.Ins.DB.MovieGenres.Where(g => g.movie_id == p.id);
                        if (movieGenre != null)
                        {
                            foreach (var genre in movieGenre)
                            {
                                DataProvider.Ins.DB.MovieGenres.Remove(genre);
                            }
                            DataProvider.Ins.DB.SaveChanges();
                        }

                        // Delete Movie
                        var deleteMovie = DataProvider.Ins.DB.Movies.Where(m => m.id == p.id).FirstOrDefault();
                        if (deleteMovie != null)
                        {
                            DataProvider.Ins.DB.Movies.Remove(deleteMovie);
                            DataProvider.Ins.DB.SaveChanges();
                        }
                        MessageBox.Show("Xoá Phim thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                        var listMovie = DataProvider.Ins.DB.Movies.ToList();
                        ListMovie = new ObservableCollection<Movie>(listMovie);
                    }

                }
            }
        }

    }
}
