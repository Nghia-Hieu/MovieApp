using Microsoft.Win32;
using MovieApp.Models;
using MovieApp.ModelView;
using MovieApp.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MovieApp.ViewModel
{
    internal class MovieInfoViewModel : BaseViewModel
    {

        private Movie _EditMovie;
        public Movie EditMovie { get { return _EditMovie; } set { _EditMovie = value; OnPropertyChanged(); } }

        private string _image;
        public string image { get { return _image; } set { _image = value; OnPropertyChanged(); } }

        public ICommand LoadMovieInfoCommand { get; set; }
        public ICommand ShowTimeClickCommand { get; set; }
        public ICommand SelectImageCommand { get; set; }

        public MovieInfoViewModel() 
        {
            LoadMovieInfoCommand = new RelayCommand<object>((p) => { return true; }, (p) => { LoadMovie(); });
            SelectImageCommand = new RelayCommand<object>((p) => { return true; }, (p) => { LoadImage(); });

        }
        private void LoadMovie()
        {
            MovieAdmin movieAdmin = new MovieAdmin();
            var movieData = movieAdmin.DataContext as MovieAdminViewModel;
            if(movieData.SelectedMovie != null)
            {
                EditMovie = movieData.SelectedMovie;
            }
        }

        private void LoadImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedImagePath = openFileDialog.FileName;
                BitmapImage DisplayedImage = new BitmapImage(new Uri(selectedImagePath));

                // Save the selected image to the resource folder
                SaveImageToResourceFolder(selectedImagePath);
            }
        }
        private void SaveImageToResourceFolder(string selectedImagePath)
        {
            string resourceFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
            if (!Directory.Exists(resourceFolderPath))
            {
                Directory.CreateDirectory(resourceFolderPath);
            }

            string fileName = Path.GetFileName(selectedImagePath);
            string destinationPath = Path.Combine(resourceFolderPath, fileName);
            File.Copy(selectedImagePath, destinationPath, true);
        }
    }
}
