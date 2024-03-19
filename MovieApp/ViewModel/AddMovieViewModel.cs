using Microsoft.Win32;
using MovieApp.Models;
using MovieApp.ModelView;
using MovieApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MovieApp.ViewModel
{
    public class MovieIdValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Regex regex = new Regex("^[A-Z]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (regex.IsMatch((string)value))
            {
                var checkID = DataProvider.Ins.DB.Movies.Where(c => c.id == (string)value).FirstOrDefault();
                if (checkID != null)
                {
                    return new ValidationResult(false, "Id đã tồn tại, vui lòng chọn id khác");
                }
                return ValidationResult.ValidResult;

            }
            else
                return new ValidationResult(false, "Id gồm các kí tự in hoa");
        }
    }
    public class DoubleValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string input = value as string;

            double result;
            if (double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                return ValidationResult.ValidResult;
            }
            else
            {
                return new ValidationResult(false, "Input must be a valid double.");
            }
        }
    }
    internal class AddMovieViewModel : BaseViewModel
    {
        private Movie _NewMovie;
        public Movie NewMovie { get => _NewMovie; set { _NewMovie = value; OnPropertyChanged(); } }

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
        public ICommand LoadAddCommand { get; set; }


        public AddMovieViewModel()
        {
            NewMovie = new Movie() { name = "", certification = "", id = "", duration = "", rating = null, release_date = DateTime.Today, status = false, description = "", image = "" };
            MovieName = ""; MovieId = ""; MovieDescription = ""; MovieDuration = ""; MovieRating = 0; MovieRelease = DateTime.Today;
            SaveMovieCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { SaveMovie(p); });
            GetImageCommand = new RelayCommand<object>((p) => { return true; }, (p) => { SelectImage(); });
            LoadAddCommand = new RelayCommand<object>((p) => { return true; }, (p) => { LoadAddWindow(); });

        }

        private void LoadAddWindow()
        {
            CertType = new ObservableCollection<string>();
            DisplayedImage = null;
            selectedImagePath = "";
            var certData = DataProvider.Ins.DB.Certifications;
            if (certData != null)
            {
                foreach (var cert in certData)
                {
                    CertType.Add(cert.id);
                }
            }
        }

        private void SaveMovie(Window p)
        {
            if (MovieName == "" || MovieCert == "" || MovieId == "" || MovieDuration == "" || DisplayedImage==null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Cảnh báo");
            }
            else
            {
                string durationMovie = "";
                if(MovieDuration.Contains("phút"))
                    durationMovie = MovieDuration;
                else
                    durationMovie = MovieDuration+ " phút";
                //Debug.WriteLine($"{MovieName}+{MovieCert}+{MovieId}+{MovieDuration}+{MovieRelease}+{MovieDescription}+{MovieCert}");
                SaveImageToResourceFolder(selectedImagePath);
                NewMovie = new Movie() { id = MovieId, name = MovieName, certification=MovieCert, duration = durationMovie, rating = MovieRating, release_date = MovieRelease, status=false, description = MovieDescription, image = null };
                DataProvider.Ins.DB.Movies.Add(NewMovie);
                DataProvider.Ins.DB.SaveChanges();
                MessageBox.Show("Thêm Phim thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                MovieAdmin movieAdminWindow = new MovieAdmin();
                var data = movieAdminWindow.DataContext as MovieAdminViewModel;
                if (data != null)
                {
                    var listMovie = DataProvider.Ins.DB.Movies.ToList();
                    data.ListMovie = new ObservableCollection<Movie>(listMovie);
                   
                }
                NewMovie = new Movie() { name = "", certification = "", id = "", duration = "", rating = null, release_date = DateTime.Today, status = false, description = "", image = "" };
                MovieName = ""; MovieId = ""; MovieDescription = ""; MovieDuration = ""; MovieRating = 0; MovieRelease = DateTime.Today;
                p.Close();
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
                //Selected
                // Save the selected image to the resource folder
                //SaveImageToResourceFolder(selectedImagePath);
            }
        }
        private void SaveImageToResourceFolder(string selectedImagePath)
        {
            string resourceFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
            if (!Directory.Exists(resourceFolderPath))
            {
                Directory.CreateDirectory(resourceFolderPath);
            }

            //string fileName = Path.GetFileName(selectedImagePath);
            string fileName = $"{MovieId}.jpg";
            string destinationPath = Path.Combine(resourceFolderPath, fileName);
            Debug.WriteLine(destinationPath);

            File.Copy(selectedImagePath, destinationPath, true);
        }
    }
}
