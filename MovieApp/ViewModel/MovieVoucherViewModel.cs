using MovieApp.Models;
using MovieApp.ModelView;
using MovieApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace MovieApp.ViewModel
{
    public class MovieStatusNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool amount)
            {
                if (amount == true)
                    return "Đang chiếu";
                else
                    return "Ngừng chiếu";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class MovieStatusNameButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool amount)
            {
                if (amount == true)
                    return "Dừng chiếu";
                else
                    return "Mở chiếu";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class MovieStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool amount)
            {
                if (amount == true)
                    return Brushes.Green;
                else
                    return Brushes.Red;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MovieStatusColorButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool amount)
            {
                if (amount == true)
                    return Brushes.Red;
                else
                    return Brushes.Green;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class MovieVoucherViewModel : BaseViewModel
    {
        private ObservableCollection<Movie> _MovieList { get; set; }
        public ObservableCollection<Movie> MovieList { get => _MovieList; set { _MovieList = value; OnPropertyChanged(); } }

        private ObservableCollection<Voucher> _VoucherList { get; set; }
        public ObservableCollection<Voucher> VoucherList { get => _VoucherList; set { _VoucherList = value; OnPropertyChanged(); } }

        private string _SearchText;
        public string SearchText { get => _SearchText; set { _SearchText = value; OnPropertyChanged(); } }

        public Voucher SelectedVoucher;

        public ICommand LoadInfoCommand { get; set; }
        public ICommand SearchMovieCommand { get; set; }
        public ICommand ReverseStatusCommand { get; set; }
        public ICommand NewVoucherCommand { get; set; }
        public ICommand EditVoucherCommand { get; set; }
        public ICommand DeleteVoucherCommand { get; set; }


        public MovieVoucherViewModel()
        {
            LoadInfoCommand = new RelayCommand<object>((p) => { return true; }, (p) => { LoadInfo(); });
            SearchMovieCommand = new RelayCommand<object>((p) => { return true; }, (p) => { SearchMovie(); });
            ReverseStatusCommand = new RelayCommand<Movie>((p) => { return true; }, (p) => { ReverseStatusMovie(p); });

            NewVoucherCommand = new RelayCommand<object>((p) => { return true; }, (p) => { AddVoucher(); });
            EditVoucherCommand = new RelayCommand<Voucher>((p) => { return true; }, (p) => { EditVoucher(p); });
            DeleteVoucherCommand = new RelayCommand<Voucher>((p) => { return true; }, (p) => { DeleteVoucher(p); });

        }

        private void LoadMovie()
        {
            var listMovie = DataProvider.Ins.DB.Movies.ToList();
            MovieList = new ObservableCollection<Movie>(listMovie);
        }
        private void LoadVoucher()
        {
            var listVoucher = DataProvider.Ins.DB.Vouchers.ToList();
            VoucherList = new ObservableCollection<Voucher>(listVoucher);
        }
        private void LoadInfo()
        {
            LoadMovie();
            LoadVoucher();
        }
        private void SearchMovie()
        {
            {
                var movieData = DataProvider.Ins.DB.Movies.
                                        Where(m => m.name.Contains(SearchText));
                MovieList = new ObservableCollection<Movie>(movieData);
            }
        }
        private void ReverseStatusMovie(Movie p)
        {
            if(p.status == true)
            {
                MessageBoxResult result = MessageBox.Show($"Đóng chiếu phim {p.name} ?","Thông báo", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if(result == MessageBoxResult.OK)
                {
                    var changeMovie = DataProvider.Ins.DB.Movies.Where(m => m.id == p.id).FirstOrDefault();
                    changeMovie.status = !changeMovie.status;
                    DataProvider.Ins.DB.SaveChanges();
                    MessageBox.Show($"Đóng chiếu phim {p.name} thành công!", "Thông báo", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                    LoadMovie();
                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show($"Mở chiếu phim {p.name} ?", "Thông báo", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.OK)
                {
                    var changeMovie = DataProvider.Ins.DB.Movies.Where(m => m.id == p.id).FirstOrDefault();
                    changeMovie.status = !changeMovie.status;
                    DataProvider.Ins.DB.SaveChanges();
                    MessageBox.Show($"Mở chiếu phim {p.name} thành công!", "Thông báo", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                    LoadMovie();
                }
            }
        }

        private void AddVoucher()
        {
            AddVoucherWindow addVoucher = new AddVoucherWindow();
            addVoucher.ShowDialog();
        }

        private void EditVoucher(Voucher p)
        {
            SelectedVoucher = p;
            VoucherInfoWindow voucherInfoWindow = new VoucherInfoWindow();
            voucherInfoWindow.ShowDialog();
        }

        private void DeleteVoucher(Voucher p)
        {
            MessageBoxResult result = MessageBox.Show($"Bạn có chắc muốn xóa mã {p.id}","Thông báo", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                DataProvider.Ins.DB.Vouchers.Remove(p);
                DataProvider.Ins.DB.SaveChanges();
                MessageBox.Show("Xóa mã thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadVoucher();
            }
        }
    }
}
