using MovieApp.Models;
using MovieApp.ModelView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MovieApp.ViewModel
{
    public class GenderType
    {
        public string Name { get; set; }
        // Other properties as needed
        public override string ToString()
        {
            return Name;
        }

    }
    
    public class UserValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if(((string)value).Length  < 5) 
            {
                return new ValidationResult(false, "User needs at least 5 characters!");
            }

            var userAccount = DataProvider.Ins.DB.Users;
            foreach (var user in userAccount)
            {
                if (user.username == (string)value)
                {
                    return new ValidationResult(false, "This username already exisit!");
                }
            }
            return ValidationResult.ValidResult;
        }
    }
    public class PassValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Regex regex = new Regex("(?=(.*[0-9]))((?=.*[A-Za-z0-9])(?=.*[A-Z])(?=.*[a-z]))^.{8,}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if(regex.IsMatch((string)value))
                return ValidationResult.ValidResult;
            else
                return new ValidationResult(false, "Ít nhất 8 kí tự bao gồm kí tự HOA và SỐ!");
        }
    }

    internal class RegisterViewModel : BaseViewModel
    {
        public ICommand RegisterCommand { get; set; }

        public ICommand PasswordChangedCommand { get; set; }

        public ICommand RePasswordChangedCommand { get; set; }

        public ICommand DobChangedCommand { get; set; }

        public ObservableCollection<GenderType> GenderTypes { get; set; }


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

        private GenderType _Gender;
        public GenderType Gender { get => _Gender; set { _Gender = value; OnPropertyChanged(); } }

        public RegisterViewModel()
        {
            Fullname = ""; Password = ""; Repassword = ""; DOB = ""; Gender = new GenderType() {Name="" };
            GenderTypes = new ObservableCollection<GenderType>
            {
               new GenderType { Name = "Nam" },
               new GenderType {Name="Nữ"},
               new GenderType {Name="Khác"}
            };
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
                MessageBox.Show("PASSWORD chưa trùng khớp!", "Cảnh báo");
            else if(Fullname == "" || Password=="" || Repassword == "" || DOB == "" || Gender.Name == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Cảnh báo");
            }
            else if ( DateTime.Parse(DOB) >= DateTime.Today)
            {
                MessageBox.Show("Thông tin NGÀY SINH không hợp lệ", "Cảnh báo");

            }
            else
            {
                var hashPass = PassEncode(Password);
                DateTime dayOfBirth = DateTime.Parse(DOB);
                DataProvider.Ins.DB.Users.Add(new User {id=$"{Username}", name = Fullname, username=Username, password = hashPass, day_of_birth = dayOfBirth, gender = Gender.Name });
                DataProvider.Ins.DB.SaveChanges();
                Debug.WriteLine(Gender.Name);
                MessageBoxResult result = MessageBox.Show("Đăng ký thành công");
                if(result == MessageBoxResult.OK)
                    p.Close();
            }
        }

        public static string PassEncode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
