using MovieApp.Models;
using MovieApp.ModelView;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MovieApp.ViewModel
{
    internal class RegisterViewModel : BaseViewModel
    {
        public ICommand RegisterCommand { get; set; }

        public ICommand PasswordChangedCommand { get; set; }

        public ICommand RePasswordChangedCommand { get; set; }

        public ICommand DobChangedCommand { get; set; }



        private string _Fullname;
        public string Fullname { get => _Fullname; set { _Fullname = value; OnPropertyChanged(); } }

        private string _Username;
        public string Username { get => _Username; set { _Username = value; OnPropertyChanged(); } }

        private string _Password;
        public string Password { get => _Password; set { _Password = value; OnPropertyChanged(); } }

        private string _Repassword;
        public string Repassword { get => _Repassword; set { _Repassword = value; OnPropertyChanged(); } }

        private string _DOB;
        public string DOB { get => _DOB; set { _DOB = value; OnPropertyChanged(); } }

        private string _Gender;
        public string Gender { get => _Gender; set { _Gender = value; OnPropertyChanged(); } }

        public RegisterViewModel()
        {
            Fullname = ""; Password = ""; Repassword = ""; DOB = ""; Gender = "";
            RegisterCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { Register(p); });
            PasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { Password = p.Password; });
            RePasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { Repassword = p.Password; });

        }

        void Register(Window p)
        {
            if (p == null)
                return;
            Debug.WriteLine($"{Fullname}-{Username}-{Password}-{Repassword}-{DOB}-{Gender}");
            if (Password != Repassword)
                MessageBox.Show("Password chưa trùng khớp!", "Cảnh báo");
            else if(Fullname == "" || Password=="" || Repassword == "" || DOB == "" || Gender == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Cảnh báo");
            }
            else
            {
                DateTime dayOfBirth = DateTime.Parse(DOB);
                DataProvider.Ins.DB.Users.Add(new User {id=$"{Username}", name = Fullname, username=Username, password = Password, day_of_birth = dayOfBirth, gender = Gender });
                DataProvider.Ins.DB.SaveChanges();
                MessageBoxResult result = MessageBox.Show("Đăng ký thành công");
                if(result == MessageBoxResult.OK)
                    p.Close();
            }
        }
    }
}
