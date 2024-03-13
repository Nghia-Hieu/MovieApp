using LiveCharts;
using LiveCharts.Wpf;
using MovieApp.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

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

            Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" };
            YFormatter = value => value.ToString("#,0");
           
        }
    }
}
