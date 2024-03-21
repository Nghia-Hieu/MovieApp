using Microsoft.Win32;
using MovieApp.Models;
using MovieApp.ModelView;
using MovieApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MovieApp.ViewModel
{
    internal class MovieInfoViewModel : BaseViewModel
    {

        private string _MovieName;
        public string MovieName { get => _MovieName; set { _MovieName = value; OnPropertyChanged(); } }

        private string _MovieId;
        public string MovieId { get => _MovieId; set { _MovieId = value; OnPropertyChanged(); } }

        private string _MovieDuration;
        public string MovieDuration { get => _MovieDuration; set { _MovieDuration = value; OnPropertyChanged(); } }

        private double _MovieRating;
        public double MovieRating { get => _MovieRating; set { _MovieRating = value; OnPropertyChanged(); } }

        private DateTime _MovieRelease;
        public DateTime MovieRelease { get => _MovieRelease; set { _MovieRelease = value; OnPropertyChanged(); } }

        private string _MovieDescription;
        public string MovieDescription { get => _MovieDescription; set { _MovieDescription = value; OnPropertyChanged(); } }

        private string _MovieCert;
        public string MovieCert { get => _MovieCert; set { _MovieCert = value; OnPropertyChanged(); } }

        private ObservableCollection<GenreStatus> _ListGenre;
        public ObservableCollection<GenreStatus> ListGenre { get => _ListGenre; set { _ListGenre = value; OnPropertyChanged(); } }

        private ObservableCollection<string> _CertType;
        public ObservableCollection<string> CertType { get => _CertType; set { _CertType = value; OnPropertyChanged(); } }

        private string selectedImagePath;

        private BitmapImage _DisplayedImage;
        public BitmapImage DisplayedImage
        {
            get { return _DisplayedImage; }
            set
            {
                _DisplayedImage = value;
                OnPropertyChanged();
            }
        }


        public ICommand GetImageCommand { get; set; }
        public ICommand SaveMovieCommand { get; set; }
        public ICommand LoadMovieInfoCommand { get; set; }

        public MovieInfoViewModel()
        {
            LoadMovieInfoCommand = new RelayCommand<object>((p) => { return true; }, (p) => { LoadMovie(); });
            GetImageCommand = new RelayCommand<object>((p) => { return true; }, (p) => { SelectImage(); });
            SaveMovieCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { SaveMovie(p); });

        }
        private void LoadMovie()
        {
            MovieAdmin movieAdmin = new MovieAdmin();
            var data = movieAdmin.DataContext as MovieAdminViewModel;
            CertType = new ObservableCollection<string>();
            DisplayedImage = null;
            selectedImagePath = "";

            var genreData = DataProvider.Ins.DB.MovieGenres.Select(x => x.genre_name).Distinct();
            ListGenre = new ObservableCollection<GenreStatus>();
            if (genreData != null)
            {
                foreach (var genreItem in genreData)
                {
                    GenreStatus newGenre = new GenreStatus() { Name = genreItem, IsSelected = false };
                    ListGenre.Add(newGenre);
                }
            }

            var certData = DataProvider.Ins.DB.Certifications;
            if (certData != null)
            {
                foreach (var cert in certData)
                {
                    CertType.Add(cert.id);
                }
            }

            if (data != null)
            {
                var movieData = data.SelectedMovie;
                MovieId = movieData.id;
                MovieName = movieData.name;
                MovieDuration = movieData.duration;
                MovieCert = movieData.certification;
                MovieRating = (double)movieData.rating;
                MovieRelease = movieData.release_date;
                MovieDescription = movieData.description;
                string path = $"{AppDomain.CurrentDomain.BaseDirectory}/Images/{movieData.id}.jpg";
                if (File.Exists(path))
                    DisplayedImage = new BitmapImage(new Uri(path));
                var movieGenres = DataProvider.Ins.DB.MovieGenres.Where(g => g.movie_id == movieData.id);
                if (movieGenres != null)
                {
                    foreach (var genre in ListGenre)
                    {
                        if (movieGenres.Any(g => g.genre_name == genre.Name))
                        {
                            genre.IsSelected = true;
                        }

                    }
                }
            }
        }

        private void SaveMovie(Window p)
        {
            var movieInfo = DataProvider.Ins.DB.Movies.Where(m => m.id == MovieId).FirstOrDefault();
            if (MovieName == "" || MovieCert == "" || MovieId == "" || MovieDuration == "" || DisplayedImage == null || ListGenre.Count(x => x.IsSelected) == 0)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (movieInfo != null)
                {
                    movieInfo.name = MovieName;
                    movieInfo.description = MovieDescription;
                    movieInfo.rating = MovieRating;
                    movieInfo.release_date = MovieRelease;
                    movieInfo.duration = MovieDuration;
                    movieInfo.certification = MovieCert;
                    if (selectedImagePath != "")
                        SaveImageToResourceFolder(selectedImagePath);
                    DataProvider.Ins.DB.SaveChanges();

                    var movieGenreNamesFromDB = DataProvider.Ins.DB.MovieGenres
                                                .Where(g => g.movie_id == MovieId)
                                                .Select(g => g.genre_name)
                                                .ToList();
                    var listGenreNames = ListGenre
                        .Where(g => g.IsSelected == true) // Filter items where Status is true
                        .Select(g => g.Name)
                        .ToList();
                    bool areListsEqual = movieGenreNamesFromDB.Count == listGenreNames.Count &&
                         movieGenreNamesFromDB.All(listGenreNames.Contains);
                    if (!areListsEqual)
                    {
                        var currentGenre = DataProvider.Ins.DB.MovieGenres.Where(g => g.movie_id == MovieId);
                        if (currentGenre != null)
                        {
                            foreach(var genre in currentGenre)
                            {
                                DataProvider.Ins.DB.MovieGenres.Remove(genre);
                            }
                        }
                        foreach (var genre in ListGenre)
                        {
                            if (genre.IsSelected)
                            {
                                MovieGenre newMovieGenre = new MovieGenre() { movie_id = MovieId, genre_name = genre.Name };
                                DataProvider.Ins.DB.MovieGenres.Add(newMovieGenre);
                            }
                        }
                        DataProvider.Ins.DB.SaveChanges();

                    }

                    MessageBox.Show("Cập nhật phim thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    MovieAdmin movieAdminWindow = new MovieAdmin();
                    var data = movieAdminWindow.DataContext as MovieAdminViewModel;
                    if (data != null)
                    {
                        var listMovie = DataProvider.Ins.DB.Movies.ToList();
                        data.ListMovie = new ObservableCollection<Movie>(listMovie);

                    }
                    p.Close();
                }
            }
        }
        private void SelectImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                selectedImagePath = openFileDialog.FileName;
                DisplayedImage = new BitmapImage(new Uri(selectedImagePath));

            }
        }
        private void SaveImageToResourceFolder(string selectedImagePath)
        {
            string resourceFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
            if (!Directory.Exists(resourceFolderPath))
            {
                Directory.CreateDirectory(resourceFolderPath);
            }
            string fileName = $"{MovieId}.jpg";
            string destinationPath = Path.Combine(resourceFolderPath, fileName);
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            File.Copy(selectedImagePath, destinationPath, true);
        }
    }
}
