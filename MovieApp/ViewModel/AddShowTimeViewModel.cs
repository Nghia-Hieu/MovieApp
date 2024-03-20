using MovieApp.Models;
using MovieApp.ModelView;
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
    internal class AddShowTimeViewModel : BaseViewModel
    {
        private DateTime _ShowDate;
        public DateTime ShowDate { get => _ShowDate; set { _ShowDate = value; OnPropertyChanged(); } }

        private string _ShowTime;
        public string ShowTime { get => _ShowTime; set { _ShowTime = value; OnPropertyChanged(); } }

        private MovieDataList _SelectedMovie;
        public MovieDataList SelectedMovie { get => _SelectedMovie; set { _SelectedMovie = value; OnPropertyChanged(); } }

        private ObservableCollection<MovieDataList> _MovieList;
        public ObservableCollection<MovieDataList> MovieList { get => _MovieList; set { _MovieList = value; OnPropertyChanged(); } }
        public ICommand LoadPageCommand { get; set; }
        public ICommand SelectedDateCommand { get; set; }
        public ICommand SaveShowCommand { get; set; }

        private static IEnumerable<(string ShowId, string SeatId, int Price, bool Status, string UserId)> CreateSeatRange(string showId, char startRow, char endRow)
        {
            int price = 0;
            if (startRow >= 'A' && endRow <= 'C')
            {
                price = 60000;
            }
            else if (startRow >= 'D' && endRow <= 'I')
            {
                price = 70000;
            }
            else if (startRow >= 'J' && endRow <= 'K')
            {
                price = 80000;
            }

            return Enumerable.Range(startRow, endRow - startRow + 1)
                .SelectMany(row =>
                    Enumerable.Range(1, 12).Select(seatNumber =>
                        (
                            ShowId: showId,
                            SeatId: $"{(char)row}{seatNumber:D2}",
                            Price: price,
                            Status: false,
                            UserId: "U000"
                        )
                    )
                );
        }
        public AddShowTimeViewModel()
        {
            LoadPageCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { LoadPage(); });
            SelectedDateCommand = new RelayCommand<object>((p) => { return true; }, (p) => { LoadMovie(); });
            SaveShowCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { AddShow(p); });

        }
        private void LoadPage()
        {
            ShowDate = DateTime.Today;
            ShowTime = "12:00";
            LoadMovie();
        }

        private void LoadMovie()
        {
            SelectedMovie = null;
            var limitDate = ShowDate.AddDays(-30);
            var movies = DataProvider.Ins.DB.Movies.Where(m => m.release_date < ShowDate && m.release_date > limitDate);

            MovieList = new ObservableCollection<MovieDataList>();
            foreach (var movie in movies)
            {
                MovieList.Add(new MovieDataList() { Movie = movie });
            }
        }
        private void AddShow(Window p)
        {
            var time = DateTime.Parse(ShowTime).ToString("HH:mm");
            string maxId = DataProvider.Ins.DB.ShowTimes
                       .Select(d => d.id)
                       .OrderByDescending(id => id)
                       .FirstOrDefault();

            // Extract the numeric part of the id and increment it
            int nextIdNumber = 1;
            if (!string.IsNullOrEmpty(maxId) && maxId.Length > 2 && int.TryParse(maxId.Substring(2), out int maxNumericId))
            {
                nextIdNumber = maxNumericId + 1;
            }
            string newId = nextIdNumber < 100 ? $"SC{nextIdNumber:000}" : $"SC{nextIdNumber}";

            if (SelectedMovie == null)
            {
                MessageBox.Show("Vui lòng chọn phim", "Thông báo", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            }
            else
            {
                Debug.WriteLine($"{newId} {SelectedMovie.Movie.id} {ShowDate} {time}");
                var checkExistShow = DataProvider.Ins.DB.ShowTimes.Where(x => x.movie_id == SelectedMovie.Movie.id && x.date == ShowDate && x.time ==time);
                if (checkExistShow.Count() > 0)
                {
                    MessageBox.Show("Suất chiếu này đã tồn tại", "Thông báo", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                }
                else
                {
                    ShowTime newShow = new ShowTime() { id = newId, date = ShowDate, movie_id = SelectedMovie.Movie.id, time = time };

                    
                    // Generate seat 
                    var seatData = new List<(string show_id, string seat_id, int price, bool status, string user_id)>();

                    seatData.AddRange(CreateSeatRange(newId, 'A', 'C')); // A-C: 60000
                    seatData.AddRange(CreateSeatRange(newId, 'D', 'I')); // D-I: 70000
                    seatData.AddRange(CreateSeatRange(newId, 'J', 'K')); // J-K: 80000

                  
                    foreach (var seat in seatData)
                    {
                        Seat newSeat = new Seat() { show_id = seat.show_id, seat_id = seat.seat_id, price = seat.price, status = seat.status, user_id = seat.user_id };
                        DataProvider.Ins.DB.Seats.Add(newSeat);
                        //Console.WriteLine($"{statement.seat_id} {statement.price} {statement.user_id} ");
                    }

                    DataProvider.Ins.DB.ShowTimes.Add(newShow);
                    DataProvider.Ins.DB.SaveChanges();
                    MessageBox.Show("Thêm thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

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
