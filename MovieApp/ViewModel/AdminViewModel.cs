using LiveCharts;
using LiveCharts.Wpf;
using MovieApp.Models;
using MovieApp.ModelView;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MovieApp.ViewModel
{
    public class PieChartData
    {
        public string Title { get; set; }
        public double Value { get; set; }
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

        public List<string> Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        public ICommand LoadChartCommand { get; set; }
        public ICommand GetDateCommand { get; set; }


        public AdminViewModel()
        {
            SeriesCollection = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Category 1",
                    Values = new ChartValues<double> { 3 },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Category 2",
                    Values = new ChartValues<double> { 4 },
                    DataLabels = true
                },
            };

            CartesianSeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Series 1",
                    Values = new ChartValues<double> { 4000000, 60000, 50000, 20000, 40000 }
                },
                new LineSeries
                {
                    Title = "Series 2",
                    Values = new ChartValues<double> { 60000, 70000, 30000, 40000, 60000 },
                    PointGeometry = null
                },
                new LineSeries
                {
                    Title = "Series 3",
                    Values = new ChartValues<double> { 40000, 20000, 70000, 20000, 70000 },
                    PointGeometry = DefaultGeometries.Square,
                    PointGeometrySize = 10
                }
            };

            //Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" };
            Formatter = value => value.ToString("#,0");
            LoadChartCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { LoadChart(); });

            GetDateCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { Debug.WriteLine(WeekChartDate); });


        }
        public void LoadChart()
        {
            DateTime currentDate =  DateTime.Parse("03/13/2024");
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
            Labels = new List<string>();
            CartesianSeriesCollection = new SeriesCollection();
            List <double> doubles = new List<double>();

            WeekChartDate = currentDate.ToString();
            for (int i = 0; i<6; i++)
            {
                DateTime checkDate = currentDate.AddDays(i);
                Labels.Add(checkDate.ToShortDateString());
                var dailyRevenue = DataProvider.Ins.DB.ShowTimes.Where(show => show.date == checkDate).
                                                                Join(DataProvider.Ins.DB.Seats.Where(seat => seat.status == true),
                                                                        showTime => showTime.id,
                                                                        seat => seat.show_id,
                                                                        (showTime, seat) => seat.price).Sum();
                Double daily = (double)dailyRevenue;

                doubles.Add(daily);

               
            }
            var newLine = new StackedColumnSeries
            {
                DataLabels = true,
                Values = new ChartValues<double> (doubles),
            };
            CartesianSeriesCollection.Add(newLine);
        }

        public void WeeklyRevenueChart()
        {

        }

    }
}
