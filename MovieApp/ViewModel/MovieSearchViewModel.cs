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
using System.Xml.Linq;

namespace MovieApp.ViewModel
{
    public class ReleaseYear
    {

        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _Year;

        public string Year
        {
            get { return _Year; }
            set { _Year = value; }
        }
        public override string ToString()
        {
            return Year;
        }
    }
    public class ShowDate
    {

        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _Date;

        public string Date
        {
            get { return _Date; }
            set { _Date = value; }
        }
        public override string ToString()
        {
            return Date;
        }
    }

    public class Rating
    {

        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _RateType;

        public string RateType
        {
            get { return _RateType; }
            set { _RateType = value; }
        }
        public override string ToString()
        {
            return RateType;
        }
    }

    internal class MovieSearchViewModel : BaseViewModel
    {
        private ObservableCollection<ReleaseYear> _MovieByReleaseYear;
        public ObservableCollection<ReleaseYear> MovieByReleaseYear
        {
            get { return _MovieByReleaseYear; }
            set { _MovieByReleaseYear = value; }
        }
        private ReleaseYear _ChoosenReleaseYear;
        public ReleaseYear ChoosenReleaseYear
        {
            get { return _ChoosenReleaseYear; }
            set { _ChoosenReleaseYear = value; }
        }


        private ObservableCollection<ShowDate> _MovieByShowDate;
        public ObservableCollection<ShowDate> MovieByShowDate
        {
            get { return _MovieByShowDate; }
            set { _MovieByShowDate = value; }
        }
        private ShowDate _ChoosenShowDate;
        public ShowDate ChoosenShowDate
        {
            get { return _ChoosenShowDate; }
            set { _ChoosenShowDate = value; }
        }


        private ObservableCollection<Rating> _MovieByRating;
        public ObservableCollection<Rating> MovieByRating
        {
            get { return _MovieByRating; }
            set { _MovieByRating = value; }
        }
        private Rating _ChoosenRatingType;
        public Rating ChoosenRatingType
        {
            get { return _ChoosenRatingType; }
            set { _ChoosenRatingType = value; }
        }

        private string _ReleaseYear;
        public string ReleaseYear { get { return _ReleaseYear; } set { _ReleaseYear = value; OnPropertyChanged(); } }

        private string _ShowDate;
        public string ShowDate { get { return _ShowDate; } set { _ShowDate = value; OnPropertyChanged(); } }

        private string _Rating;
        public string Rating { get { return _Rating; } set { _Rating = value; OnPropertyChanged(); } }

        private Movie _SelectedMovie;
        public Movie SelectedMovie { get { return _SelectedMovie; } set { _SelectedMovie = value; OnPropertyChanged(); } }

        private ObservableCollection<Movie> _MovieSet;
        public ObservableCollection<Movie> MovieSet
        {
            get { return _MovieSet; }
            set
            {
                _MovieSet = value;
                OnPropertyChanged();
            }
        }

        public ICommand SortReleaseYearCommand { get; set; }
        public ICommand SortShowDateCommand { get; set; }
        public ICommand SortRatingCommand { get; set; }

        public MovieSearchViewModel() 
        {
            MovieByReleaseYear = new ObservableCollection<ReleaseYear>()
            {
                new ReleaseYear() {Id=0, Year = "Tất cả"},
                new ReleaseYear() {Id=1, Year = "2024"},
                new ReleaseYear() {Id=2, Year = "2023"},
                new ReleaseYear() {Id=3, Year = "2022"},
            };
            ChoosenReleaseYear = MovieByReleaseYear.FirstOrDefault();

            MovieByShowDate = new ObservableCollection<ShowDate>()
            {
                new ShowDate() {Id=0, Date = "Tất cả"},
                new ShowDate() {Id=1, Date = "08/03/2024"},
                new ShowDate() {Id=2, Date = "09/03/2024"},
                new ShowDate() {Id=3, Date = "10/03/2024"},
            };
            ChoosenShowDate = MovieByShowDate.FirstOrDefault();

            _MovieByRating = new ObservableCollection<Rating>()
            {
                new Rating() {Id=0, RateType = "Mặc định"},
                new Rating() {Id=1, RateType = "Thấp đến Cao"},
                new Rating() {Id=2, RateType = "Cao đến Thấp"},
            };
            ChoosenRatingType = MovieByRating.FirstOrDefault();

            MovieSet = new ObservableCollection<Movie>();

            var countMovie = DataProvider.Ins.DB.Movies;

            foreach (var i in countMovie)
            {
                Movie movie = new Movie();
                movie.id = i.id;
                movie.name = i.name;
                movie.duration = i.duration;
                movie.certification = i.certification;
                movie.rating = i.rating;
                movie.release_date = i.release_date;
                movie.status = i.status;
                movie.description = i.description;
                movie.image = $"/Images/{i.id}.jpg";
                MovieSet.Add(movie);
            }

            SortRatingCommand = new RelayCommand<object>((p) => { return true; }, (p) => { Debug.WriteLine(ChoosenRatingType.Id); });
        }
    }
}
