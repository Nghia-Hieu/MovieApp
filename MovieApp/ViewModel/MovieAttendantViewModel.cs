using MovieApp.Models;
using MovieApp.ModelView;
using MovieApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MovieApp.ViewModel
{
    public class MovieDirector
    {
        public Director DirectorInfo { get; set; }
        public string MovieName { get; set; }
    }
    internal class MovieAttendantViewModel : BaseViewModel
    {
        private MovieDirector _SelectedDirector;
        public MovieDirector SelectedDirector { get => _SelectedDirector; set { _SelectedDirector = value; OnPropertyChanged(); } }


        private ObservableCollection<MovieDirector> _ListDirector;
        public ObservableCollection<MovieDirector> ListDirector { get => _ListDirector; set { _ListDirector = value; OnPropertyChanged(); } }
        public ICommand LoadAttendantCommand { get; set; }
        public ICommand NewDirectorCommand { get; set; }
        public ICommand EditDirectorCommand { get; set; }
        public ICommand DeleteDirectorCommand { get; set; }

        public MovieAttendantViewModel()
        {
            LoadAttendantCommand = new RelayCommand<object>((p) => { return true; }, (p) => { LoadAttendant(); });
            NewDirectorCommand = new RelayCommand<object>((p) => { return true; }, (p) => { NewDirector(); });
            EditDirectorCommand = new RelayCommand<MovieDirector>((p) => { return true; }, (p) => { EditDirector(p); });
            DeleteDirectorCommand = new RelayCommand<MovieDirector>((p) => { return true; }, (p) => { DeleteDirector(p); });

        }

        private void LoadAttendant()
        {
            var directorMovieData = DataProvider.Ins.DB.Directors
                                   .Join(DataProvider.Ins.DB.Movies,
                                          director => director.movie_id,
                                          movie => movie.id,
                                          (director, movie) => new MovieDirector
                                          {
                                              DirectorInfo = director,
                                              MovieName = movie.name
                                          });
            ListDirector = new ObservableCollection<MovieDirector>(directorMovieData);
        }

        private void EditDirector(MovieDirector p)
        {
            Debug.WriteLine(p.DirectorInfo.name);
            SelectedDirector = p;
            DirectorInfoWindow directorInfoWindow = new DirectorInfoWindow();
            directorInfoWindow.ShowDialog();
        }

        private void DeleteDirector(MovieDirector p)
        {
            Debug.WriteLine(p.DirectorInfo.name);
            MessageBoxResult result = MessageBox.Show($"Bạn có chắc muốn xóa {p.DirectorInfo.name}", "Cảnh báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes) {
                var deleteData = DataProvider.Ins.DB.Directors.Where(d => d.id == p.DirectorInfo.id).FirstOrDefault();
                if(deleteData != null)
                {
                    DataProvider.Ins.DB.Directors.Remove(deleteData);
                    DataProvider.Ins.DB.SaveChanges();
                    MessageBox.Show("Xoá thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
             

                    var directorMovieData = DataProvider.Ins.DB.Directors
                                  .Join(DataProvider.Ins.DB.Movies,
                                         director => director.movie_id,
                                         movie => movie.id,
                                         (director, movie) => new MovieDirector
                                         {
                                             DirectorInfo = director,
                                             MovieName = movie.name
                                         });
                    ListDirector = new ObservableCollection<MovieDirector>(directorMovieData);
                }
            }
        }

        private void NewDirector()
        {
            AddDirectorWindow addDirectorWindow = new AddDirectorWindow();

            addDirectorWindow.ShowDialog();
        }
    }
}
