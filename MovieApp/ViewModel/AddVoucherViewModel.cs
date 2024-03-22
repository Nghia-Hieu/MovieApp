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

namespace MovieApp.ViewModel
{
    internal class AddVoucherViewModel : BaseViewModel
    {
        private string _VoucherDescription;
        public string VoucherDescription { get => _VoucherDescription; set { _VoucherDescription = value; OnPropertyChanged(); } }

        private string _VoucherId;
        public string VoucherId { get => _VoucherId; set { _VoucherId = value; OnPropertyChanged(); } }

        private double _VoucherDiscount;
        public double VoucherDiscount { get => _VoucherDiscount; set { _VoucherDiscount = value; OnPropertyChanged(); } }

        private bool _SelectedStatus;
        public bool SelectedStatus { get => _SelectedStatus; set { _SelectedStatus = value; OnPropertyChanged(); } }

        private ObservableCollection<bool> _StatusList;
        public ObservableCollection<bool> StatusList { get => _StatusList; set { _StatusList = value; OnPropertyChanged(); } }

        public ICommand SaveVoucherCommand { get; set; }
        public ICommand LoadAddDirectorCommand { get; set; }
        public AddVoucherViewModel() 
        {
            VoucherId = ""; VoucherDescription = ""; VoucherDiscount = 0; SelectedStatus = true; 
            StatusList = new ObservableCollection<bool>() { true, false };
            SaveVoucherCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { SaveVoucher(p); });

        }

        private void SaveVoucher(Window p)
        {
            if (VoucherId == "" || VoucherDescription==""|| VoucherDiscount== 0)
            {
                MessageBox.Show("Vui lòng nhập đập đủ thông tin!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                var checkID = DataProvider.Ins.DB.Vouchers.Where(v => v.id == VoucherId).FirstOrDefault();
                if (checkID != null)
                {
                    MessageBox.Show("Mã Voucher đã tồn tại. Vui lòng chọn mã khác!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    Voucher newVoucher = new Voucher() { description = VoucherDescription, discount = (long)VoucherDiscount, id = VoucherId, status = SelectedStatus };
                    DataProvider.Ins.DB.Vouchers.Add(newVoucher);
                    DataProvider.Ins.DB.SaveChanges();

                    MovieVoucher movieVoucher = new MovieVoucher();
                    var voucherData = movieVoucher.DataContext as MovieVoucherViewModel;
                    var listVoucher = DataProvider.Ins.DB.Vouchers.ToList();
                    voucherData.VoucherList = new ObservableCollection<Voucher>(listVoucher);
                    MessageBox.Show("Thêm thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    p.Close();
                    VoucherId = ""; VoucherDescription = ""; VoucherDiscount = 0; SelectedStatus = true;
                }
            }
        }
    }
}
