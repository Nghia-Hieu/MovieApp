using LiveCharts;
using LiveCharts.Wpf;
using MovieApp.Models;
using MovieApp.ModelView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MovieApp.ViewModel
{
     
    public class MovieViewData
    {
        public string MovieId { get; set; }
        public string MovieName { get; set;}
        public string ReleaseDate { get; set; }
        public long NumberOfSeats { get; set; }
        public long TotalPrice { get; set; }

    }
    internal class AdminViewModel : BaseViewModel
    {
        private SeriesCollection _SeriesCollection;
        public SeriesCollection SeriesCollection { get => _SeriesCollection; set { _SeriesCollection = value; OnPropertyChanged(); } }

        private SeriesCollection _CartesianSeriesCollection;
        public SeriesCollection CartesianSeriesCollection { get => _CartesianSeriesCollection; set { _CartesianSeriesCollection = value; OnPropertyChanged(); } }

        private int _AvailableMovies;
        public int AvailableMovies { get => _AvailableMovies; set { _AvailableMovies = value; OnPropertyChanged(); } }

        private int _DailyShows;
        public int DailyShows { get => _DailyShows; set { _DailyShows = value; OnPropertyChanged(); } }

        private int _WeeklyShows;
        public int WeeklyShows { get => _WeeklyShows; set { _WeeklyShows = value; OnPropertyChanged(); } }

        private string _WeekChartDate;
        public string WeekChartDate { get => _WeekChartDate; set { _WeekChartDate = value; OnPropertyChanged(); } }

        private string _MonthChartDate;
        public string MonthChartDate { get => _MonthChartDate; set { _MonthChartDate = value; OnPropertyChanged(); } }

        private List<string> _Labels { get; set; }
        public List<string> Labels { get => _Labels; set { _Labels = value; OnPropertyChanged(); } }

        private Func<double, string> _Formatter { get; set; }
        public Func<double, string> Formatter { get => _Formatter; set { _Formatter = value; OnPropertyChanged(); } }

        private List<string> _Labels2 { get; set; }
        public List<string> Labels2 { get => _Labels2; set { _Labels2 = value; OnPropertyChanged(); } }
        private Func<double, string> _Formatter2 { get; set; }
        public Func<double, string> Formatter2 { get => _Formatter2; set { _Formatter2 = value; OnPropertyChanged(); } }

        private ObservableCollection<MovieViewData> _ListTopMovie { get; set; }
        public ObservableCollection<MovieViewData> ListTopMovie { get => _ListTopMovie; set { _ListTopMovie = value; OnPropertyChanged(); } }



        public ICommand LoadChartCommand { get; set; }
        public ICommand GetDateCommand { get; set; }
        public ICommand GetMonthCommand { get; set; }


        public AdminViewModel()
        {
            ListTopMovie = new ObservableCollection<MovieViewData>();

            Formatter2 = value => value.ToString("#,0");


            Formatter = value => value.ToString("#,0");
            LoadChartCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { LoadChart(); });

            GetDateCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { WeeklyRevenueChart(); });

            GetMonthCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { Debug.WriteLine(MonthChartDate); MonthlyRevenueChart(); });

        }
        public void LoadChart()
        {
            DateTime currentDate = DateTime.Parse("03/12/2024");
            var moviesCount = DataProvider.Ins.DB.ShowTimes.Where(show => show.date == currentDate).
                                                            Select(show => show.movie_id).Distinct().Count();
            AvailableMovies = moviesCount;
            var dailyCount = DataProvider.Ins.DB.ShowTimes.Where(show => show.date == currentDate).
                                                            Select(show => show.id).Count();
            DailyShows = dailyCount;
            DateTime weekDate = currentDate.AddDays(7);
            var weeklyCount = DataProvider.Ins.DB.ShowTimes.Where(show => show.date >= currentDate && show.date <= weekDate).
                                                            Select(show => show.id).Count();
            WeeklyShows = weeklyCount;

            // Get data chart for weekly chart 
            Labels = new List<string>();
            CartesianSeriesCollection = new SeriesCollection();
            List<double> weekData = new List<double>();

            WeekChartDate = currentDate.ToString();
            for (int i = 0; i < 6; i++)
            {
                DateTime checkDate = currentDate.AddDays(i);
                Labels.Add(checkDate.ToShortDateString());
                var dailyRevenue = DataProvider.Ins.DB.ShowTimes.Where(show => show.date == checkDate).
                                                                Join(DataProvider.Ins.DB.Seats.Where(seat => seat.status == true),
                                                                        showTime => showTime.id,
                                                                        seat => seat.show_id,
                                                                        (showTime, seat) => seat.price);
                if (dailyRevenue.Count() > 0)
                {

                    Double daily = dailyRevenue.Sum();

                    weekData.Add(daily);
                }
                else
                {
                    weekData.Add(0);
                }
            }
            var newLine = new StackedColumnSeries
            {
                DataLabels = true,
                Values = new ChartValues<double>(weekData),
            };
            CartesianSeriesCollection.Add(newLine);

            //Get data chart for monthly chart
            Labels2 = new List<string>();
            SeriesCollection = new SeriesCollection();
            List<double> monthData = new List<double>();
            MonthChartDate = currentDate.ToString();
            for (int i = -1; i < 3; i++)
            {
                DateTime checkDate = currentDate.AddDays(i * 7);
                DateTime aWeekDate = checkDate.AddDays(6);
                Labels2.Add(checkDate.ToShortDateString());
                var weeklyRevenue = DataProvider.Ins.DB.ShowTimes.Where(show => show.date >= checkDate && show.date <= aWeekDate).
                                                                Join(DataProvider.Ins.DB.Seats.Where(seat => seat.status == true),
                                                                        showTime => showTime.id,
                                                                        seat => seat.show_id,
                                                                        (showTime, seat) => seat.price);
                if (weeklyRevenue.Count() > 0)
                {

                    Double daily = weeklyRevenue.Sum();

                    monthData.Add(daily);
                }
                else
                {
                    monthData.Add(0);
                }
            }
            var newDataLine = new StackedColumnSeries
            {
                Title = "WeekRevenue",
                Values = new ChartValues<double>(monthData),
            };
            SeriesCollection.Add(newDataLine);

            //Get top ticket movies
            var result = DataProvider.Ins.DB.Seats
                                    .Where(seat => seat.status == true)
                                    .Join(DataProvider.Ins.DB.ShowTimes, seat => seat.show_id, showtime => showtime.id, (seat, showtime) => new { Seat = seat, ShowTime = showtime })
                                    .Join(DataProvider.Ins.DB.Movies, join => join.ShowTime.movie_id, movie => movie.id, (join, movie) => new { Movie = movie, Seat = join.Seat })
                                    .GroupBy(join => new { join.Movie.id, join.Movie.name, join.Movie.release_date })
                                    .Select(group => new
                                    {
                                        MovieId = group.Key.id,
                                        MovieName = group.Key.name,
                                        ReleaseDate = group.Key.release_date,
                                        NumberOfSeats = group.Count(),
                                        TotalPrice = group.Sum(x => x.Seat.price)
                                    }).OrderByDescending(item => item.NumberOfSeats);
            
            foreach (var item in result)
            {
                MovieViewData data = new MovieViewData();
                data.MovieId = item.MovieId;
                data.MovieName = item.MovieName;
                data.ReleaseDate = item.ReleaseDate.ToShortDateString();
                data.NumberOfSeats = item.NumberOfSeats;
                data.TotalPrice = item.TotalPrice;
                ListTopMovie.Add(data);

                Console.WriteLine($"Movie Name: {item.MovieName}, Number of Seats: {item.NumberOfSeats}, Total Price: {item.TotalPrice}");
            }

        }

        public void WeeklyRevenueChart()
        {
            Labels = new List<string>();
            CartesianSeriesCollection = new SeriesCollection();
            List<double> weekData = new List<double>();

            DateTime selectedDate = DateTime.Parse(WeekChartDate); Debug.WriteLine(selectedDate);
            for (int i = 0; i < 6; i++)
            {
                DateTime checkDate = selectedDate.AddDays(i);
                Labels.Add(checkDate.ToShortDateString());
                var dailyRevenue = DataProvider.Ins.DB.ShowTimes.Where(show => show.date == checkDate).
                                                                Join(DataProvider.Ins.DB.Seats.Where(seat => seat.status == true),
                                                                        showTime => showTime.id,
                                                                        seat => seat.show_id,
                                                                        (showTime, seat) => seat.price);
                Debug.WriteLine("Count" + dailyRevenue.Count());
                if (dailyRevenue.Count() > 0)
                {

                    Double daily = dailyRevenue.Sum();

                    weekData.Add(daily);
                }
                else
                {
                    weekData.Add(0);
                }
            }
            var newLine = new StackedColumnSeries
            {
                DataLabels = true,
                Values = new ChartValues<double>(weekData),
            };
            CartesianSeriesCollection.Add(newLine);

        }

        public void MonthlyRevenueChart()
        {
            Labels2 = new List<string>();
            SeriesCollection = new SeriesCollection();
            List<double> monthData = new List<double>();
            DateTime selectedDate = DateTime.Parse(MonthChartDate);
            for (int i = -1; i < 3; i++)
            {
                DateTime checkDate = selectedDate.AddDays(i * 7);
                DateTime aWeekDate = checkDate.AddDays(6);
                Labels2.Add(checkDate.ToShortDateString());
                var weeklyRevenue = DataProvider.Ins.DB.ShowTimes.Where(show => show.date >= checkDate && show.date <= aWeekDate).
                                                                Join(DataProvider.Ins.DB.Seats.Where(seat => seat.status == true),
                                                                        showTime => showTime.id,
                                                                        seat => seat.show_id,
                                                                        (showTime, seat) => seat.price);
                if (weeklyRevenue.Count() > 0)
                {

                    Double daily = weeklyRevenue.Sum();

                    monthData.Add(daily);
                }
                else
                {
                    monthData.Add(0);
                }
            }
            var newDataLine = new StackedColumnSeries
            {
                Title = "WeekRevenue",
                Values = new ChartValues<double>(monthData),
            };
            SeriesCollection.Add(newDataLine);
        }

    }
}
