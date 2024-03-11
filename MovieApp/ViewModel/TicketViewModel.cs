using MovieApp.Models;
using MovieApp.ModelView;
using MovieApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    public class BooleanToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal amount)
            {
                return amount.ToString("#,##0") + " VND";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ReverseBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    internal class TicketViewModel : BaseViewModel
    {
        private Movie _MovieInfo;
        public Movie MovieInfo { get => _MovieInfo; set { _MovieInfo = value; OnPropertyChanged(); } }

        private ShowTime _ShowTimeInfo;
        public ShowTime ShowTimeInfo { get { return _ShowTimeInfo; } set { _ShowTimeInfo = value; OnPropertyChanged(); } }

        private ObservableCollection<Seat> _SeatPosition;
        public ObservableCollection<Seat> SeatPosition { get => _SeatPosition; set { _SeatPosition = value; OnPropertyChanged(); } }

        private ObservableCollection<Seat> _ChoosenSeats;
        public ObservableCollection<Seat> ChoosenSeats { get => _ChoosenSeats; set { _ChoosenSeats = value; OnPropertyChanged(); } }

        private decimal _TotalAmount;
        public decimal TotalAmount { get { return _TotalAmount; } set { _TotalAmount = value; OnPropertyChanged(); } }

        public ICommand LoadTicketCommand { get; set; }

        public ICommand OccupySeatCommand { get; set; }


        public TicketViewModel()
        {
            LoadTicketCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { LoadSeat(); });
            OccupySeatCommand = new RelayCommand<Seat>((p) => { return true; }, (p) => { OccupySeat(p); });

        }

        private void OccupySeat(Seat seat)
        {

            if (seat.background == Brushes.Gray)
            {
                seat.background = Brushes.Green;
                ChoosenSeats.Remove(seat);
                decimal total = 0;
                foreach (var item in ChoosenSeats)
                {
                    // Assuming Amount is the property you want to sum up
                    total += item.price;
                }
                TotalAmount = total;
            }
            else
            {
                if (ChoosenSeats.Count > 4)
                {
                    MessageBox.Show("Chỉ được đặt cùng lúc 5 vé !", "Cảnh báo");
                }
                else
                {
                    seat.background = Brushes.Gray;
                    ChoosenSeats.Add(seat);

                    decimal total = 0;
                    foreach (var item in ChoosenSeats)
                    {
                        // Assuming Amount is the property you want to sum up
                        total += item.price;
                    }
                    TotalAmount = total;
                }
            }

        }

        private void LoadSeat()
        {
            MovieDetail movieDetail = new MovieDetail();
            var movieDetailData = movieDetail.DataContext as MovieDetailViewModel;
            MovieInfo = movieDetailData.MovieInfo;
            ShowTimeInfo = movieDetailData.SelectedShowTime;
            Debug.WriteLine(ShowTimeInfo.id);

            ChoosenSeats = new ObservableCollection<Seat>();
            TotalAmount = 0;

            var SeatList = DataProvider.Ins.DB.Seats.Where(x => x.show_id == ShowTimeInfo.id);
            SeatPosition = new ObservableCollection<Seat>(SeatList);
            foreach (var i in SeatPosition)
            {
                if (i.status == true)
                {
                    i.background = Brushes.Red;
                }
                else
                    i.background = Brushes.Green;
            }
            Debug.WriteLine(SeatList.ToList().Count);
        }
    }
}
