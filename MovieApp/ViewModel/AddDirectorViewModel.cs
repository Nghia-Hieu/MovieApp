using MovieApp.Models;
using MovieApp.ModelView;
using MovieApp.Views;
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
using System.Windows.Input;

namespace MovieApp.ViewModel
{
    public class MovieDataList
    {
        public Movie Movie { get; set; }
        
        public override string ToString()
        {
            return Movie.name;
        }
    }
    internal class AddDirectorViewModel : BaseViewModel
    {
        private string _DirectorDescription;
        public string DirectorDescription { get => _DirectorDescription; set { _DirectorDescription = value; OnPropertyChanged(); } }

        private string _DirectorName;
        public string DirectorName { get => _DirectorName; set { _DirectorName = value; OnPropertyChanged(); } }

        private MovieDataList _SelectedMovie;
        public MovieDataList SelectedMovie { get => _SelectedMovie; set { _SelectedMovie = value; OnPropertyChanged(); } }

        private ObservableCollection<MovieDataList> _MovieList;
        public ObservableCollection<MovieDataList> MovieList { get => _MovieList; set { _MovieList = value; OnPropertyChanged(); } }

        public ICommand SaveDirectorCommand { get; set; }
        public ICommand LoadAddDirectorCommand { get; set; }

        public AddDirectorViewModel()
        {
            SaveDirectorCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { SaveDirector(p); });
            LoadAddDirectorCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { LoadPage(); });

        }

        private void LoadPage()
        {
            DirectorDescription = ""; DirectorName = "";SelectedMovie = null;

            var moviesWithoutDirector = DataProvider.Ins.DB.Movies
                                               .GroupJoin(
                                                    DataProvider.Ins.DB.Directors,
                                                   movie => movie.id,
                                                   director => director.movie_id,
                                                   (movie, directorGroup) => new { Movie = movie, Directors = directorGroup })
                                               .Where(x => !x.Directors.Any())
                                               .Select(x => x.Movie);
            MovieList = new ObservableCollection<MovieDataList>();

            foreach (var movie in moviesWithoutDirector)
            {
                MovieList.Add(new MovieDataList() { Movie = movie });
            }
        }

        private void SaveDirector(Window p)
        {
            string maxId = DataProvider.Ins.DB.Directors
                        .Select(d => d.id)
                        .OrderByDescending(id => id)
                        .FirstOrDefault();

            // Extract the numeric part of the id and increment it
            int nextIdNumber = 1;
            if (!string.IsNullOrEmpty(maxId) && maxId.Length > 2 && int.TryParse(maxId.Substring(1), out int maxNumericId))
            {
                nextIdNumber = maxNumericId + 1;
            }
            string newId = $"M{nextIdNumber:D2}";

            if (DirectorDescription == "" || DirectorName == "" || SelectedMovie == null)
                MessageBox.Show("Vui lòng điền đầy đủ thông tin", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
            {
                if (DirectorName.Length > 100 || DirectorName.Length < 2)
                {
                    MessageBox.Show("Tên không hợp lệ", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if(DirectorDescription.Length > 2000) {
                    MessageBox.Show("Mô tả quá dài", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    Director newDir = new Director() { id = newId, name = DirectorName, description = DirectorDescription, movie_id=SelectedMovie.Movie.id};
                    DataProvider.Ins.DB.Directors.Add(newDir);
                    DataProvider.Ins.DB.SaveChanges();
                    MessageBox.Show("Thêm thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    MovieAttendant movieAttendant = new MovieAttendant();
                    var maData = movieAttendant.DataContext as MovieAttendantViewModel;

                    var directorMovieData = DataProvider.Ins.DB.Directors
                                  .Join(DataProvider.Ins.DB.Movies,
                                         director => director.movie_id,
                                         movie => movie.id,
                                         (director, movie) => new MovieDirector
                                         {
                                             DirectorInfo = director,
                                             MovieName = movie.name
                                         });
                    maData.ListDirector = new ObservableCollection<MovieDirector>(directorMovieData);
                    p.Close();
                }
            }
        }
    }
}
