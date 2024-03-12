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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

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

    public class CouponValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {

            var voucherList = DataProvider.Ins.DB.Vouchers;
            foreach (var voucher in voucherList)
            {
                if (voucher.id == (string)value)
                {
                    if (voucher.status == false)
                        return new ValidationResult(false, $"Voucher hết hạn");
                    else
                        return ValidationResult.ValidResult;
                }
            }
            return new ValidationResult(false, $"Voucher không hợp lệ");
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

        private string _VoucherCode;
        public string VoucherCode { get => _VoucherCode; set { _VoucherCode = value; OnPropertyChanged(); } }

        private Voucher _VoucherAdded;
        public Voucher VoucherAdded { get => _VoucherAdded; set { _VoucherAdded = value; OnPropertyChanged(); } }

        private decimal _TotalAmount;
        public decimal TotalAmount { get { return _TotalAmount; } set { _TotalAmount = value; OnPropertyChanged(); } }

        private string _DiscountAmount;
        public string DiscountAmount { get { return _DiscountAmount; } set { _DiscountAmount = value; OnPropertyChanged(); } }

        private decimal _Amount;
        public decimal Amount { get { return _Amount; } set { _Amount = value; OnPropertyChanged(); } }

        private bool _EnableOccupy;
        public bool EnableOccupy { get { return _EnableOccupy; } set { _EnableOccupy = value; OnPropertyChanged(); } }


        public ICommand LoadTicketCommand { get; set; }

        public ICommand OccupySeatCommand { get; set; }

        public ICommand TextCompletedCommand { get; set; }

        public ICommand OccupyTicketCommand { get; set; }



        public TicketViewModel()
        {
            LoadTicketCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { LoadSeat(); });
            OccupySeatCommand = new RelayCommand<Seat>((p) => { return true; }, (p) => { OccupySeat(p); });
            TextCompletedCommand = new RelayCommand<Seat>((p) => { return true; }, (p) => { CheckVoucher(); });
            OccupyTicketCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { OccupyTicket(p); });
        }

        private void OccupySeat(Seat seat)
        {

            if (seat.background == Brushes.Gray)
            {
                seat.background = Brushes.Green;
                ChoosenSeats.Remove(seat);
                if (ChoosenSeats.Count == 0)
                    EnableOccupy = false;
                decimal total = 0;
                foreach (var item in ChoosenSeats)
                {
                    // Assuming Amount is the property you want to sum up
                    total += item.price;
                }
                TotalAmount = total;
                if (VoucherAdded.discount > 0)
                {
                    Amount = TotalAmount - TotalAmount * VoucherAdded.discount / 100;
                }
                else
                    Amount = TotalAmount;
            }
            else
            {
                EnableOccupy = true;
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
                    if (VoucherAdded.discount > 0)
                    {
                        Amount = TotalAmount - TotalAmount * VoucherAdded.discount / 100;
                    }
                    else
                        Amount = TotalAmount;
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
            VoucherAdded = new Voucher();
            VoucherCode = "";
            TotalAmount = 0;
            Amount = 0;
            EnableOccupy = false;

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

        private void CheckVoucher()
        {
            if (VoucherCode == "")
            {
                DiscountAmount = "";
                Amount = TotalAmount;
            }
            else
            {

                var voucher = DataProvider.Ins.DB.Vouchers.Where(x => x.id == VoucherCode);
                if (voucher.Count()>0)
                {
                    VoucherAdded = voucher.FirstOrDefault();
                    DiscountAmount = VoucherAdded.description;
                    Amount = TotalAmount - TotalAmount * VoucherAdded.discount / 100;
                }
                else
                {
                    DiscountAmount = "Voucher không hợp lệ";
                    Amount = TotalAmount;
                }
            }
        }

        private void OccupyTicket(Window p)
        {
            string OccupiedSeats = "";
            foreach(var i in ChoosenSeats)
            {
                OccupiedSeats += i.seat_id + " ";
            }
            string amountTickets =  Amount.ToString("#,##0") + " VND";
            string dateShowTime = ShowTimeInfo.date.ToString("dd/MM/yyyy");

            MainWindow mainWindow = new MainWindow();
            var mainWindowData = mainWindow.DataContext as MainViewModel;
            var UserAccount = mainWindowData.UserInfo;
            if(UserAccount == null)
            {
                MessageBox.Show("Login để Đặt vé!", "Cảnh báo");
            }
            else
            {
                MessageBoxResult result = MessageBox.Show($"Xác nhận đặt vé phim {MovieInfo.name} suất {dateShowTime}, {ShowTimeInfo.time} ở các hàng " +
                 $"{OccupiedSeats} với giá {amountTickets}", "Đặt vé", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    foreach (Seat i in ChoosenSeats)
                    {
                        var connectData = DataProvider.Ins.DB.Seats.FirstOrDefault(x => x.show_id == i.show_id && x.seat_id == i.seat_id);
                        if (connectData != null)
                        {
                            connectData.status = true;
                            connectData.user_id = UserAccount.id;
                            DataProvider.Ins.DB.SaveChanges();
                            MessageBox.Show("Đặt vé THÀNH CÔNG!", "SUCCESS");
                            p.Close();
                        }
                    }
                }
            }
            
        }

    }
}
