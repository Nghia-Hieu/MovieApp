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
    internal class ShowInfoViewModel : BaseViewModel
    {
        private DateTime _ShowDate;
        public DateTime ShowDate { get => _ShowDate; set { _ShowDate = value; OnPropertyChanged(); } }

        private DateTime _ShowTime;
        public DateTime ShowTime { get => _ShowTime; set { _ShowTime = value; OnPropertyChanged(); } }

        private MovieDataList _SelectedMovie;
        public MovieDataList SelectedMovie { get => _SelectedMovie; set { _SelectedMovie = value; OnPropertyChanged(); } }

        private string ShowId;

        private ObservableCollection<MovieDataList> _MovieList;
        public ObservableCollection<MovieDataList> MovieList { get => _MovieList; set { _MovieList = value; OnPropertyChanged(); } }
        public ICommand LoadPageCommand { get; set; }
        public ICommand SelectedDateCommand { get; set; }
        public ICommand UpdateShowCommand { get; set; }
        public ShowInfoViewModel()
        {
            LoadPageCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { LoadPage(); });
            SelectedDateCommand = new RelayCommand<object>((p) => { return true; }, (p) => { LoadMovie(); });
            UpdateShowCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { UpdateShow(p); });

        }
        private void LoadPage()
        {
            MovieApp.Views.MovieShowTime movieShowTime = new Views.MovieShowTime();
            var showInfo = movieShowTime.DataContext as MovieShowTimeViewModel;
            if (showInfo != null)
            {
                ShowId = showInfo.ChosenShowTime.ShowTimeInfo.id;
                ShowDate = showInfo.ChosenShowTime.ShowTimeInfo.date;
                ShowTime = DateTime.Parse(showInfo.ChosenShowTime.ShowTimeInfo.time);
                LoadMovie();

                SelectedMovie = MovieList.FirstOrDefault(x => x.Movie.name == showInfo.ChosenShowTime.MovieName);
                Debug.WriteLine(showInfo.ChosenShowTime.MovieName);
            }
        }

        private void LoadMovie()
        {
            SelectedMovie = null;
            var limitDate = ShowDate.AddDays(-30);
            var movies = DataProvider.Ins.DB.Movies.Where(m => m.release_date <= ShowDate && m.status == true);

            MovieList = new ObservableCollection<MovieDataList>();
            foreach (var movie in movies)
            {
                MovieList.Add(new MovieDataList() { Movie = movie });
            }
        }

        private void UpdateShow(Window p)
        {
            var time = ShowTime.ToString("HH:mm");


            if (SelectedMovie == null)
            {
                MessageBox.Show("Vui lòng chọn phim", "Thông báo", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            }
            else
            {
                var checkExistShow = DataProvider.Ins.DB.ShowTimes.Where(x => x.movie_id == SelectedMovie.Movie.id && x.date == ShowDate && x.time == time);
                if (checkExistShow.Count() > 0)
                {
                    MessageBox.Show("Suất chiếu này đã tồn tại", "Thông báo", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                }
                else
                {
                    var editInfo = DataProvider.Ins.DB.ShowTimes.Where(m => m.id == ShowId).FirstOrDefault();
                    if (editInfo != null)
                    {
                        editInfo.date = ShowDate;
                        editInfo.time = time;
                        editInfo.movie_id = SelectedMovie.Movie.id;
                    }
                    DataProvider.Ins.DB.SaveChanges();
                    MessageBox.Show("Update thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    //Update listview ShowTime
                    MovieApp.Views.MovieShowTime movieShowTime = new Views.MovieShowTime();
                    var showInfo = movieShowTime.DataContext as MovieShowTimeViewModel;
                    var showTimeMovieData = DataProvider.Ins.DB.ShowTimes
                              .Join(DataProvider.Ins.DB.Movies,
                                     showtime => showtime.movie_id,
                                     movie => movie.id,
                                     (showtime, movie) => new MovieShowTime
                                     {
                                         ShowTimeInfo = showtime,
                                         MovieName = movie.name,
                                     }).OrderByDescending(entry => entry.ShowTimeInfo.date);

                    showInfo.ListShowTime = new ObservableCollection<MovieShowTime>(showTimeMovieData);
                    p.Close();
                }

            }
        }



    }
}
