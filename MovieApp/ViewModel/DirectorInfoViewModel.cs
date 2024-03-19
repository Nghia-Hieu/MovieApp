using MovieApp.Models;
using MovieApp.ModelView;
using MovieApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MovieApp.ViewModel
{
    internal class DirectorInfoViewModel : BaseViewModel
    {
        private string _DirectorDescription;
        public string DirectorDescription { get => _DirectorDescription; set { _DirectorDescription = value; OnPropertyChanged(); } }

        private string _DirectorName;
        public string DirectorName { get => _DirectorName; set { _DirectorName = value; OnPropertyChanged(); } }

        private Director _EditDirector;
        public Director EditDirector { get => _EditDirector; set { _EditDirector = value; OnPropertyChanged(); } }

        private MovieDataList _SelectedMovie;
        public MovieDataList SelectedMovie { get => _SelectedMovie; set { _SelectedMovie = value; OnPropertyChanged(); } }

        private ObservableCollection<MovieDataList> _MovieList;
        public ObservableCollection<MovieDataList> MovieList { get => _MovieList; set { _MovieList = value; OnPropertyChanged(); } }

        private string DirectorId;

        public ICommand UpdateDirectorCommand { get; set; }
        public ICommand LoadDirectorInfoCommand { get; set; }
        public DirectorInfoViewModel() {
            LoadDirectorInfoCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { LoadDirectorInfo(); });
            UpdateDirectorCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { UpdateDirector(p); });
        }

        private void LoadDirectorInfo()
        {
            MovieAttendant movieAttendant = new MovieAttendant();
            var maData = movieAttendant.DataContext as MovieAttendantViewModel;
            if (maData != null)
            {
                if(maData.SelectedDirector != null)
                {
                    DirectorId = maData.SelectedDirector.DirectorInfo.id;
                    DirectorName = maData.SelectedDirector.DirectorInfo.name;
                    DirectorDescription = maData.SelectedDirector.DirectorInfo.description;
                }
            }
        }

        private void UpdateDirector(Window p)
        {
            if (DirectorDescription == "" || DirectorName == "")
                MessageBox.Show("Vui lòng điền đầy đủ thông tin", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
            {
                if (DirectorName.Length > 100 || DirectorName.Length < 2)
                {
                    MessageBox.Show("Tên không hợp lệ", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (DirectorDescription.Length > 2000)
                {
                    MessageBox.Show("Mô tả quá dài", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    var directorInfo = DataProvider.Ins.DB.Directors.Where(d => d.id == DirectorId).FirstOrDefault();
                    if(directorInfo != null)
                    {
                        directorInfo.name = DirectorName;
                        directorInfo.description = DirectorDescription; 
                        DataProvider.Ins.DB.SaveChanges();
                        MessageBox.Show("Update thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

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
}
