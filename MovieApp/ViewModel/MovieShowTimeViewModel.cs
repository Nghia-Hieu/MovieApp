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
    public class MovieShowTime
    {
        public ShowTime ShowTimeInfo { get; set; }
        public string MovieName { get; set; }
    }
    internal class MovieShowTimeViewModel : BaseViewModel
    {
        private MovieDirector _SelectedShowTime;
        public MovieDirector SelectedShowTime { get => _SelectedShowTime; set { _SelectedShowTime = value; OnPropertyChanged(); } }

        private ObservableCollection<MovieShowTime> _ListShowTime;
        public ObservableCollection<MovieShowTime> ListShowTime { get => _ListShowTime; set { _ListShowTime = value; OnPropertyChanged(); } }

        private string _SearchText;
        public string SearchText { get => _SearchText; set { _SearchText = value; OnPropertyChanged(); } }

        public MovieShowTime ChosenShowTime;

        public ICommand LoadShowTimeCommand { get; set; }
        public ICommand NewShowTimeCommand { get; set; }
        public ICommand ToSeatCommand { get; set; }
        public ICommand EditShowTimeCommand { get; set; }
        public ICommand DeleteShowTimeCommand { get; set; }
        public ICommand SearchMovieCommand { get; set; }

        public MovieShowTimeViewModel()
        {
            LoadShowTimeCommand = new RelayCommand<object>((p) => { return true; }, (p) => { LoadShowTime(); });
            NewShowTimeCommand = new RelayCommand<object>((p) => { return true; }, (p) => { NewShowTime(); });
            ToSeatCommand = new RelayCommand<MovieShowTime>((p) => { return true; }, (p) => { ToSeat(p); });
            EditShowTimeCommand = new RelayCommand<MovieShowTime>((p) => { return true; }, (p) => { EditShowTime(p); });
            DeleteShowTimeCommand = new RelayCommand<MovieShowTime>((p) => { return true; }, (p) => { DeleteShowTime(p); });
            SearchMovieCommand = new RelayCommand<object>((p) => { return true; }, (p) => { SearchMovieShowTime(); });


        }

        private void LoadShowTime()
        {
            SearchText = "";
            var showTimeMovieData = DataProvider.Ins.DB.ShowTimes
                                  .Join(DataProvider.Ins.DB.Movies,
                                         showtime => showtime.movie_id,
                                         movie => movie.id,
                                         (showtime, movie) => new MovieShowTime
                                         {
                                             ShowTimeInfo = showtime,
                                             MovieName = movie.name,
                                         }).OrderByDescending(entry => entry.ShowTimeInfo.date);

            ListShowTime = new ObservableCollection<MovieShowTime>(showTimeMovieData);
        }

        private void NewShowTime()
        {
            AddShowTimeWindow addShowTimeWindow = new AddShowTimeWindow();
            addShowTimeWindow.ShowDialog();

        }

        private void EditShowTime(MovieShowTime p)
        {
            ChosenShowTime = p;
            ShowInfoWindow showInfoWindow = new ShowInfoWindow();
            showInfoWindow.ShowDialog();
        }

        private void DeleteShowTime(MovieShowTime p)
        {
            MessageBoxResult result = MessageBox.Show($"Bạn có chắc muốn xóa suất chiếu {p.ShowTimeInfo.id} ngày {p.ShowTimeInfo.date.ToShortDateString()} lúc {p.ShowTimeInfo.time} của phim {p.MovieName}", "Cảnh báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var deleteData = DataProvider.Ins.DB.ShowTimes.Where(s => s.id == p.ShowTimeInfo.id).FirstOrDefault();
                if (deleteData != null)
                {
                    var seatToRemove = DataProvider.Ins.DB.Seats.Where(x => x.show_id == p.ShowTimeInfo.id);

                    if (seatToRemove != null)
                    {
                        foreach(var seat in seatToRemove)
                        {                   
                            DataProvider.Ins.DB.Seats.Remove(seat);
                        }

                        DataProvider.Ins.DB.SaveChanges();
                    }
                       
                    DataProvider.Ins.DB.ShowTimes.Remove(deleteData);
                    DataProvider.Ins.DB.SaveChanges();
                    MessageBox.Show("Xoá thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);


                    var showTimeMovieData = DataProvider.Ins.DB.ShowTimes
                                .Join(DataProvider.Ins.DB.Movies,
                                       showtime => showtime.movie_id,
                                       movie => movie.id,
                                       (showtime, movie) => new MovieShowTime
                                       {
                                           ShowTimeInfo = showtime,
                                           MovieName = movie.name,
                                       }).OrderByDescending(entry => entry.ShowTimeInfo.date);

                    ListShowTime = new ObservableCollection<MovieShowTime>(showTimeMovieData);
                }
            }
        }

        private void ToSeat(MovieShowTime p)
        {
            ChosenShowTime = p;
            ShowTicketWindow showTicketWindow = new ShowTicketWindow();
            showTicketWindow.ShowDialog();
        }

        private void SearchMovieShowTime()
        {
            if (SearchText != "")
            {
                var showTimeMovieData = DataProvider.Ins.DB.ShowTimes
                                 .Join(DataProvider.Ins.DB.Movies,
                                        showtime => showtime.movie_id,
                                        movie => movie.id,
                                        (showtime, movie) => new MovieShowTime
                                        {
                                            ShowTimeInfo = showtime,
                                            MovieName = movie.name,
                                        }).Where(m => m.MovieName.Contains(SearchText));
                ListShowTime = new ObservableCollection<MovieShowTime>(showTimeMovieData);
            }
        }
    }
}
