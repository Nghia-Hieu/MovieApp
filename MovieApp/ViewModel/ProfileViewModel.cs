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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MovieApp.ViewModel
{
    public class UserValidation2 : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (((string)value).Length < 5)
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
    public class PassValidation2 : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Regex regex = new Regex("(?=(.*[0-9]))((?=.*[A-Za-z0-9])(?=.*[A-Z])(?=.*[a-z]))^.{8,}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (regex.IsMatch((string)value))
                return ValidationResult.ValidResult;
            else
                return new ValidationResult(false, "Ít nhất 8 kí tự bao gồm kí tự HOA và SỐ!");
        }
    }
    internal class ProfileViewModel : BaseViewModel
    {
        public ICommand RegisterCommand { get; set; }

        public ICommand LoadProfileCommand { get; set; }
        public ICommand SaveInfoCommand { get; set; }

        public ICommand SavePasswordCommand { get; set; }

        public ICommand DobChangedCommand { get; set; }

        public ObservableCollection<GenderType> GenderTypes { get; set; }

        public User CurrentUser { get; set; }
        private int _Index;
        public int Index { get => _Index; set { _Index = value; OnPropertyChanged(); } }

        private string _Fullname;
        public string Fullname { get => _Fullname; set { _Fullname = value; OnPropertyChanged(); } }

        private string _Username;
        public string Username { get => _Username; set { _Username = value; OnPropertyChanged(); } }

        private string _Password;
        public string Password { get => _Password; set { _Password = value; OnPropertyChanged(); } }

        private string _NewPassword;
        public string NewPassword { get => _NewPassword; set { _NewPassword = value; OnPropertyChanged(); } }

        private string _Repassword;
        public string Repassword { get => _Repassword; set { _Repassword = value; OnPropertyChanged(); } }

        private string _DOB;
        public string DOB { get => _DOB; set { _DOB = value; OnPropertyChanged(); } }

        private GenderType _Gender;
        public GenderType Gender { get => _Gender; set { _Gender = value; OnPropertyChanged(); } }
        public ProfileViewModel()
        {
            CurrentUser = new User();

            GenderTypes = new ObservableCollection<GenderType>
            {
               new GenderType { Name = "Nam" },
               new GenderType {Name="Nữ"},
               new GenderType {Name="Khác"}
            };

            Fullname = ""; Password = ""; Repassword = ""; DOB = new DateTime().ToString(); Gender = GenderTypes.FirstOrDefault(g => g.Name == "Nam")
            ; Index = 1;

            LoadProfileCommand = new RelayCommand<Page>((p) => { return true; }, (p) => { LoadPage(); });
            SaveInfoCommand = new RelayCommand<object>((p) => { return true; }, (p) => { SaveInfo(); });
            SavePasswordCommand = new RelayCommand<object>((p) => { return true; }, (p) => { SavePassword(); });

        }

        private void LoadPage()
        {
            MainWindow mainWindow = new MainWindow();
            var mainData  = mainWindow.DataContext as MainViewModel;
            CurrentUser = mainData.UserInfo;
            Fullname = CurrentUser.name;
            Username = CurrentUser.username;
            DOB = CurrentUser.day_of_birth.ToShortDateString();
            Debug.WriteLine(CurrentUser.gender);
            Gender = GenderTypes.FirstOrDefault(g=>g.Name==CurrentUser.gender);
        }

        private void SaveInfo()
        {
            var connectUser = DataProvider.Ins.DB.Users.FirstOrDefault(x => x.id == CurrentUser.id);
            if (connectUser != null)
            {
                connectUser.username = Username;
                connectUser.gender = Gender.Name;
                connectUser.name = Fullname;
                connectUser.day_of_birth = DateTime.Parse(DOB);
                DataProvider.Ins.DB.SaveChanges();
                MessageBox.Show("Update Thông tin THÀNH CÔNG!", "SUCCESS");

            }
        }

        private void SavePassword()
        {
            if(PassEncode(Password) != CurrentUser.password)
            {
                MessageBox.Show("Password CŨ không đúng!", "Cảnh báo");
            }
            else if(NewPassword != Repassword)
            {
                MessageBox.Show("Password chưa TRÙNG KHỚP!", "Cảnh báo");
            }
            else
            {
                var connectUser = DataProvider.Ins.DB.Users.FirstOrDefault(x=>x.id == CurrentUser.id);
                if(connectUser != null)
                {
                    connectUser.password = PassEncode(NewPassword);
                    DataProvider.Ins.DB.SaveChanges();
                    MessageBox.Show("Update Password THÀNH CÔNG!", "SUCCESS");

                }
            }
        }

        public static string PassEncode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
