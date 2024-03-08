using MovieApp.Models;
using MovieApp.ModelView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
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

    public class MovieComparer : IEqualityComparer<Movie>
    {
        public bool Equals(Movie x, Movie y)
        {
            return x.id == y.id;
        }

        public int GetHashCode(Movie obj)
        {
            return obj.id.GetHashCode();
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

        private ObservableCollection<Movie> StoredMovieSet;

        private string _SearchKey;
        public string SearchKey { get { return _SearchKey; } set { _SearchKey = value; OnPropertyChanged(); } }


        private int _CurrentPage = 1;
        public int CurrentPage { get { return _CurrentPage; } set { _CurrentPage = value; OnPropertyChanged(); } }

        private int pageSize = 6;


        public ICommand SortReleaseYearCommand { get; set; }
        public ICommand SortShowDateCommand { get; set; }
        public ICommand SortRatingCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand NextPageCommand { get; set; }
        public ICommand PreviousPageCommand { get; set; }



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

            SearchKey = "";

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
            StoredMovieSet = MovieSet;
            //SortReleaseYearCommand = new RelayCommand<object>((p) => { return true; }, (p) => { SortMovieByYearRelease(); });
            //SortRatingCommand = new RelayCommand<object>((p) => { return true; }, (p) => { SortMovieByRate(); });
            Pagination(1, pageSize);
            SearchCommand = new RelayCommand<object>((p) => { return true; }, (p) => { SearchMovie(); });
            NextPageCommand = new RelayCommand<object>((p) => { return true; }, (p) => { Debug.WriteLine("OK"); CurrentPage = CurrentPage + 1; Pagination(CurrentPage, pageSize); });

            PreviousPageCommand = new RelayCommand<object>((p) => { return true; }, (p) => { Debug.WriteLine("OK"); CurrentPage = CurrentPage - 1; Pagination(CurrentPage, pageSize); });

        }
        public void GetAllMovie()
        {
            var MovieDirector = DataProvider.Ins.DB.Movies;
            foreach (var i in MovieDirector)
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
        }
        public void SearchMovie()
        {
            if (SearchKey != "")
            {
                var MovieDirector = DataProvider.Ins.DB.Movies.Join(DataProvider.Ins.DB.Directors, c1 => c1.id, c2 => c2.movie_id, (c1, c2) => new { Movies = c1, Director = c2 })
                                                               .Where(result => result.Movies.name.Contains(SearchKey) || result.Director.name.Contains(SearchKey))
                                                               .Select(result => new { result.Movies, result.Director });
                MovieSet.Clear();
                foreach (var i in MovieDirector)
                {
                    Movie movie = new Movie();
                    movie.id = i.Movies.id;
                    movie.name = i.Movies.name;
                    movie.duration = i.Movies.duration;
                    movie.certification = i.Movies.certification;
                    movie.rating = i.Movies.rating;
                    movie.release_date = i.Movies.release_date;
                    movie.status = i.Movies.status;
                    movie.description = i.Movies.description;
                    movie.image = $"/Images/{i.Movies.id}.jpg"; Debug.WriteLine(movie.release_date.Year.ToString());

                    MovieSet.Add(movie);
                }
                //StoredMovieSet = MovieSet;
                //SearchKey = "";
            }
            else
            {
                var MovieDirector = DataProvider.Ins.DB.Movies;
                MovieSet.Clear();
                foreach (var i in MovieDirector)
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
                //StoredMovieSet = MovieSet;
                //SearchKey = "";
            }
            SortMovieByRate();
            SortMovieByYearRelease();
            SortMovieByShowDate();
            StoredMovieSet = MovieSet;
            Pagination(1, pageSize);
        }
        public void SortMovieByRate()
        {
            if (ChoosenRatingType.Id == 1)
            {
                MovieSet = new ObservableCollection<Movie>(MovieSet.OrderBy(x => x.rating));

            }
            else if (ChoosenRatingType.Id == 2)
            {
                MovieSet = new ObservableCollection<Movie>(MovieSet.OrderByDescending(x => x.rating));
            }
        }
        public void SortMovieByYearRelease()
        {
            if(ChoosenReleaseYear.Id != 0)
            {
                Debug.WriteLine(ChoosenReleaseYear.Year);
                var set = MovieSet.Where(item => item.release_date.Year.ToString() == ChoosenReleaseYear.Year);
                MovieSet = new ObservableCollection<Movie>(set);
            }
        }
        public void SortMovieByShowDate()
        {
            if(ChoosenShowDate.Id != 0)
            {
                var parseDate = DateTime.ParseExact(ChoosenShowDate.Date, "dd/MM/yyyy", null);
                Debug.WriteLine(ChoosenShowDate.Date+ " "+parseDate);
                var moviesInDate = DataProvider.Ins.DB.Movies.Join(DataProvider.Ins.DB.ShowTimes, movie => movie.id, showtime => showtime.movie_id, (movie, showTime) => new { Movie = movie, ShowTime = showTime })
                                                             .Where(joinResult => joinResult.ShowTime.date == parseDate)
                                                             .Select(joinResult => joinResult.Movie)
                                                             .Distinct();
                ObservableCollection<Movie> newMovieSet = new ObservableCollection<Movie>();
                Debug.WriteLine(moviesInDate.Count()+" "+ MovieSet.Count());

                if (moviesInDate.Count() > 0)
                {
                    foreach (var i in moviesInDate)
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
                        newMovieSet.Add(movie);
                    }
                    MovieSet = new ObservableCollection<Movie>(MovieSet.Intersect(newMovieSet, new MovieComparer()));

                }
            }
        }
        
        private void Pagination(int page, int pageSize)
        {
            if (page > 0)
            {
                if (pageSize > 0)
                {
                    CurrentPage = page;
                    Debug.WriteLine("OK");
                    //MovieSet.Clear();
                    int startIndex = (CurrentPage - 1) * pageSize;
                    int count = Math.Min(pageSize, StoredMovieSet.Count - startIndex);
                    MovieSet = new ObservableCollection<Movie> (StoredMovieSet.Skip(startIndex).Take(count) );
                }
            }
        } 
    }

}
