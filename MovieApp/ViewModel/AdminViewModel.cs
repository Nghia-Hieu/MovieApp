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
        public string MovieName { get; set; }
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

        private SeriesCollection _DailyMovieShowCollection;
        public SeriesCollection DailyMovieShowCollection { get => _DailyMovieShowCollection; set { _DailyMovieShowCollection = value; OnPropertyChanged(); } }

        private SeriesCollection _WeeklyMovieShowCollection;
        public SeriesCollection WeeklyMovieShowCollection { get => _WeeklyMovieShowCollection; set { _WeeklyMovieShowCollection = value; OnPropertyChanged(); } }

        private int _AvailableMovies;
        public int AvailableMovies { get => _AvailableMovies; set { _AvailableMovies = value; OnPropertyChanged(); } }

        private int _DailyShows;
        public int DailyShows { get => _DailyShows; set { _DailyShows = value; OnPropertyChanged(); } }

        private int _WeeklyShows;
        public int WeeklyShows { get => _WeeklyShows; set { _WeeklyShows = value; OnPropertyChanged(); } }

        private DateTime _WeekChartDate;
        public DateTime WeekChartDate { get => _WeekChartDate; set { _WeekChartDate = value; OnPropertyChanged(); } }

        private DateTime _MonthChartDate;
        public DateTime MonthChartDate { get => _MonthChartDate; set { _MonthChartDate = value; OnPropertyChanged(); } }

        private DateTime _WeekShowChartDate;
        public DateTime WeekShowChartDate { get => _WeekShowChartDate; set { _WeekShowChartDate = value; OnPropertyChanged(); } }

        private DateTime _MonthShowChartDate;
        public DateTime MonthShowChartDate { get => _MonthShowChartDate; set { _MonthShowChartDate = value; OnPropertyChanged(); } }

        private List<string> _Labels { get; set; }
        public List<string> Labels { get => _Labels; set { _Labels = value; OnPropertyChanged(); } }

        private Func<double, string> _Formatter { get; set; }
        public Func<double, string> Formatter { get => _Formatter; set { _Formatter = value; OnPropertyChanged(); } }

        private List<string> _Labels2 { get; set; }
        public List<string> Labels2 { get => _Labels2; set { _Labels2 = value; OnPropertyChanged(); } }
        private Func<double, string> _Formatter2 { get; set; }
        public Func<double, string> Formatter2 { get => _Formatter2; set { _Formatter2 = value; OnPropertyChanged(); } }

        private List<string> _DailyShowLabels { get; set; }
        public List<string> DailyShowLabels { get => _DailyShowLabels; set { _DailyShowLabels = value; OnPropertyChanged(); } }
        private List<string> _WeeklyShowLabels { get; set; }
        public List<string> WeeklyShowLabels { get => _WeeklyShowLabels; set { _WeeklyShowLabels = value; OnPropertyChanged(); } }
        private Func<double, string> _DailyShowFormatter { get; set; }
        public Func<double, string> DailyShowFormatter { get => _DailyShowFormatter; set { _DailyShowFormatter = value; OnPropertyChanged(); } }

        private Func<double, string> _WeeklyShowFormatter { get; set; }
        public Func<double, string> WeeklyShowFormatter { get => _WeeklyShowFormatter; set { _WeeklyShowFormatter = value; OnPropertyChanged(); } }

        private MovieViewData _DailyMovieSelected;
        public MovieViewData DailyMovieSelected { get => _DailyMovieSelected; set { _DailyMovieSelected = value; OnPropertyChanged(); } }

        private MovieViewData _WeeklyMovieSelected;
        public MovieViewData WeeklyMovieSelected { get => _WeeklyMovieSelected; set { _WeeklyMovieSelected = value; OnPropertyChanged(); } }

        private ObservableCollection<MovieViewData> _ListTopMovie { get; set; }
        public ObservableCollection<MovieViewData> ListTopMovie { get => _ListTopMovie; set { _ListTopMovie = value; OnPropertyChanged(); } }



        public ICommand LoadChartCommand { get; set; }
        public ICommand GetDateCommand { get; set; }
        public ICommand GetMonthCommand { get; set; }
        public ICommand GetDateShowCommand { get; set; }
        public ICommand GetMonthShowCommand { get; set; }
        public ICommand GetDateMovieShowCommand { get; set; }
        public ICommand GetWeekMovieShowCommand { get; set; }


        public AdminViewModel()
        {
            ListTopMovie = new ObservableCollection<MovieViewData>();




            Formatter2 = value => value.ToString("#,0");
            Formatter = value => value.ToString("#,0");
            LoadChartCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { LoadChart(); });

            GetDateCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { WeeklyRevenueChart(); });

            GetMonthCommand = new RelayCommand<Window>((p) => { return true; }, (p) => {MonthlyRevenueChart(); });
            GetDateShowCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { WeeklyShowChart(); });
            GetDateMovieShowCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { WeeklyShowChart(); });

            GetMonthShowCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { MonthlyShowChart(); });
            GetWeekMovieShowCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { MonthlyShowChart(); });


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

            // -----------------------------------Get data chart for weekly chart -----------------------------------
            Labels = new List<string>();
            CartesianSeriesCollection = new SeriesCollection();
            List<double> weekData = new List<double>();

            WeekChartDate = currentDate;
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
                Title = "Daily Revenue",
                DataLabels = true,
                Values = new ChartValues<double>(weekData),
                Fill = Brushes.DarkOliveGreen
            };

            CartesianSeriesCollection.Add(newLine);

            //-----------------------------------Get data chart for monthly chart-----------------------------------
            Labels2 = new List<string>();
            SeriesCollection = new SeriesCollection();
            List<double> monthData = new List<double>();
            MonthChartDate = currentDate;
            for (int i = -1; i < 3; i++)
            {
                DateTime checkDate = currentDate.AddDays(i * 7);
                DateTime aWeekDate = checkDate.AddDays(6);
                Labels2.Add(aWeekDate.ToShortDateString());
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
                Title = "Week Revenue",
                DataLabels = true,
                Values = new ChartValues<double>(monthData),
                Fill = Brushes.RoyalBlue
            };

            SeriesCollection.Add(newDataLine);

            //------------------------------------------------Get top ticket movies-------------------------------------------------------------
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
            }
            //-------------------------------------Get data for daily show----------------------------------

            WeekShowChartDate = currentDate;
            DailyMovieSelected = ListTopMovie[2];
            WeeklyShowChart();
            //-------------------------------------Get data for weekly show----------------------------------
            MonthShowChartDate = currentDate;
            WeeklyMovieSelected = ListTopMovie[2];
            MonthlyShowChart();
        }

        public void WeeklyRevenueChart()
        {
            Labels = new List<string>();
            CartesianSeriesCollection = new SeriesCollection();
            List<double> weekData = new List<double>();

            DateTime selectedDate = WeekChartDate;
            for (int i = 0; i < 6; i++)
            {
                DateTime checkDate = selectedDate.AddDays(i);
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
                Title = "Daily Revenue",
                DataLabels = true,
                Values = new ChartValues<double>(weekData),
                Fill = Brushes.DarkOliveGreen
            };
            CartesianSeriesCollection.Add(newLine);

        }

        public void MonthlyRevenueChart()
        {
            Labels2 = new List<string>();
            SeriesCollection = new SeriesCollection();
            List<double> monthData = new List<double>();
            DateTime selectedDate = MonthChartDate;
            for (int i = -1; i < 3; i++)
            {
                DateTime checkDate = selectedDate.AddDays(i * 7);
                DateTime aWeekDate = checkDate.AddDays(6);
                Labels2.Add(aWeekDate.ToShortDateString());
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
                DataLabels = true,
                Values = new ChartValues<double>(monthData),
                Fill = Brushes.RoyalBlue
            };
            SeriesCollection.Add(newDataLine);
        }

        public void WeeklyShowChart()
        {
            DateTime currentDate = WeekShowChartDate;
            DailyMovieShowCollection = new SeriesCollection();
            DailyShowLabels = new List<string>();
            List<double> weekShowData = new List<double>();

            for (int i = 0; i < 6; i++)
            {
                DateTime checkDate = currentDate.AddDays(i);
                DailyShowLabels.Add(checkDate.ToShortDateString());

                var DailyShowChartData = DataProvider.Ins.DB.Seats
                                        .Where(seat => seat.status == true)
                                        .Join(DataProvider.Ins.DB.ShowTimes, seat => seat.show_id, showtime => showtime.id, (seat, showtime) => new { Seat = seat, ShowTime = showtime })
                                        .Where(join => join.ShowTime.date == checkDate && join.ShowTime.movie_id == DailyMovieSelected.MovieId) // Add conditions here
                                        .Join(DataProvider.Ins.DB.Movies, join => join.ShowTime.movie_id, movie => movie.id, (join, movie) => new { Movie = movie, Seat = join.Seat })
                                        .GroupBy(join => new { join.Movie.id, join.Movie.name, join.Movie.release_date })
                                        .Select(group => new
                                        {
                                            MovieId = group.Key.id,
                                            MovieName = group.Key.name,
                                            ReleaseDate = group.Key.release_date,
                                            NumberOfSeats = group.Count(),
                                            TotalPrice = group.Sum(x => x.Seat.price)
                                        })
                                        .OrderByDescending(item => item.NumberOfSeats);
                if (DailyShowChartData.Count() > 0)
                {
                    Double daily = DailyShowChartData.First().NumberOfSeats;
                    weekShowData.Add(daily);
                }
                else
                {
                    weekShowData.Add(0);
                }
            }
            var newWeekShowDataLine = new LineSeries
            {
                Title = "Daily Seats Sold",
                Values = new ChartValues<double>(weekShowData),
            };

            DailyMovieShowCollection.Add(newWeekShowDataLine);
        }

        public void MonthlyShowChart()
        {
            DateTime currentDate = MonthShowChartDate;
            WeeklyMovieShowCollection = new SeriesCollection();
            WeeklyShowLabels = new List<string>();
            List<double> monthShowData = new List<double>();

            for (int i = -1; i < 3; i++)
            {
                DateTime checkDate = currentDate.AddDays(i*7);
                DateTime aWeekDate = checkDate.AddDays(6);
                WeeklyShowLabels.Add(aWeekDate.ToShortDateString());

                var WeeklyShowChartData = DataProvider.Ins.DB.Seats
                                        .Where(seat => seat.status == true)
                                        .Join(DataProvider.Ins.DB.ShowTimes, seat => seat.show_id, showtime => showtime.id, (seat, showtime) => new { Seat = seat, ShowTime = showtime })
                                        .Where(join => join.ShowTime.date >= checkDate && join.ShowTime.date <= aWeekDate && join.ShowTime.movie_id == WeeklyMovieSelected.MovieId) // Add conditions here
                                        .Join(DataProvider.Ins.DB.Movies, join => join.ShowTime.movie_id, movie => movie.id, (join, movie) => new { Movie = movie, Seat = join.Seat })
                                        .GroupBy(join => new { join.Movie.id, join.Movie.name, join.Movie.release_date })
                                        .Select(group => new
                                        {
                                            MovieId = group.Key.id,
                                            MovieName = group.Key.name,
                                            ReleaseDate = group.Key.release_date,
                                            NumberOfSeats = group.Count(),
                                            TotalPrice = group.Sum(x => x.Seat.price)
                                        })
                                        .OrderByDescending(item => item.NumberOfSeats);
                if (WeeklyShowChartData.Count() > 0)
                {
                    Double weekly = WeeklyShowChartData.First().NumberOfSeats;
                    monthShowData.Add(weekly);
                }
                else
                {
                    monthShowData.Add(0);
                }
            }
            var newWeekShowDataLine = new LineSeries
            {
                Title = "Weekly Seats Sold",
                Values = new ChartValues<double>(monthShowData),
            };

            WeeklyMovieShowCollection.Add(newWeekShowDataLine);
        }
    }
}
