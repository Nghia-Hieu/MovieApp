using MovieApp.Models;
using MovieApp.ModelView;
using MovieApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MovieApp.ViewModel
{
    internal class ShowTicketViewModel : BaseViewModel
    {
        private string _MovieName;
        public string MovieName { get => _MovieName; set { _MovieName = value; OnPropertyChanged(); } }

        private ShowTime _ShowTimeInfo;
        public ShowTime ShowTimeInfo { get { return _ShowTimeInfo; } set { _ShowTimeInfo = value; OnPropertyChanged(); } }

        private ObservableCollection<Seat> _SeatPosition;
        public ObservableCollection<Seat> SeatPosition { get => _SeatPosition; set { _SeatPosition = value; OnPropertyChanged(); } }

        private ObservableCollection<Seat> _ChoosenBookedSeats;
        public ObservableCollection<Seat> ChoosenBookedSeats { get => _ChoosenBookedSeats; set { _ChoosenBookedSeats = value; OnPropertyChanged(); } }

        private ObservableCollection<Seat> _ChoosenUnBookedSeats;
        public ObservableCollection<Seat> ChoosenUnBookedSeats { get => _ChoosenUnBookedSeats; set { _ChoosenUnBookedSeats = value; OnPropertyChanged(); } }

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


        public ICommand LoadSeatCommand { get; set; }

        public ICommand OccupySeatCommand { get; set; }

        public ICommand UpdateTicketCommand { get; set; }
        public ShowTicketViewModel()
        {
            LoadSeatCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { LoadSeat(); });
            OccupySeatCommand = new RelayCommand<Seat>((p) => { return true; }, (p) => { OccupySeat(p); });
            UpdateTicketCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { UpdateTicket(p); });
        }

        private void OccupySeat(Seat seat)
        {

            if (seat.background == Brushes.Gray)
            {
                seat.background = Brushes.Green;
                ChoosenUnBookedSeats.Remove(seat);
                if (ChoosenUnBookedSeats.Count == 0 && ChoosenBookedSeats.Count == 0)
                    EnableOccupy = false;
            }
            else if (seat.background == Brushes.LightGreen)
            {
                seat.background = Brushes.Red;
                ChoosenBookedSeats.Remove(seat);
                if (ChoosenUnBookedSeats.Count == 0 && ChoosenBookedSeats.Count == 0)
                    EnableOccupy = false;
            }
            else
            {

                EnableOccupy = true;
                if (ChoosenBookedSeats.Count + ChoosenUnBookedSeats.Count > 4)
                {
                    MessageBox.Show("Chỉ thay đổi cùng lúc 5 vé !", "Cảnh báo");
                }
                else
                {
                    if (seat.background == Brushes.Red)
                    {
                        seat.background = Brushes.LightGreen;
                        ChoosenBookedSeats.Add(seat);
                    }
                    else
                    {
                        seat.background = Brushes.Gray;
                        ChoosenUnBookedSeats.Add(seat);
                    }
                }
            }

        }

        private void LoadSeat()
        {
            Views.MovieShowTime movieShowTime = new Views.MovieShowTime();
            var movieShowData = movieShowTime.DataContext as MovieShowTimeViewModel;
            MovieName = movieShowData.ChosenShowTime.MovieName;

            ShowTimeInfo = movieShowData.ChosenShowTime.ShowTimeInfo;

            ChoosenUnBookedSeats = new ObservableCollection<Seat>();
            ChoosenBookedSeats = new ObservableCollection<Seat>();

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
        }


        private void UpdateTicket(Window p)
        {
            string OccupiedSeats = ""; string CancelSeats = "";
            string dateShowTime = ShowTimeInfo.date.ToString("dd/MM/yyyy");
            MessageBoxResult result;
            foreach (var i in ChoosenUnBookedSeats)
            {
                OccupiedSeats += i.seat_id + " ";
            }
            foreach (var i in ChoosenBookedSeats)
            {
                CancelSeats += i.seat_id + " ";
            }

            if (ChoosenUnBookedSeats.Count == 0 )
            {
                result = MessageBox.Show($"Xác nhận đặt vé phim {MovieName} suất {dateShowTime}, {ShowTimeInfo.time} ở hàng " +
                 $"{OccupiedSeats}", "Đặt vé", MessageBoxButton.OKCancel);
            }

            else if (ChoosenBookedSeats.Count == 0)
            {
                result = MessageBox.Show($"Xác nhận huỷ vé phim {MovieName} suất {dateShowTime}, {ShowTimeInfo.time} ở hàng " +
                 $"{CancelSeats}", "Hủy vé", MessageBoxButton.OKCancel);
            }
            else
            {
                result = MessageBox.Show($"Xác nhận huỷ vé phim {MovieName} suất {dateShowTime}, {ShowTimeInfo.time} ở hàng " +
                 $"{CancelSeats} và dặt vé ở hàng {OccupiedSeats}", "Đổi vé", MessageBoxButton.OKCancel);
            }


                
            if (result == MessageBoxResult.OK)
            {
                foreach (Seat i in ChoosenUnBookedSeats)
                {
                    var connectData = DataProvider.Ins.DB.Seats.FirstOrDefault(x => x.show_id == i.show_id && x.seat_id == i.seat_id);
                    if (connectData != null)
                    {
                        connectData.status = !connectData.status;
                        DataProvider.Ins.DB.SaveChanges();
                    }
                }
                foreach (Seat i in ChoosenBookedSeats)
                {
                    var connectData = DataProvider.Ins.DB.Seats.FirstOrDefault(x => x.show_id == i.show_id && x.seat_id == i.seat_id);
                    if (connectData != null)
                    {
                        connectData.status = !connectData.status;
                        DataProvider.Ins.DB.SaveChanges();
                    }
                }
                MessageBox.Show("Đã thay đổi trạng thái vé thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                p.Close();
            }


        }
    }
}
