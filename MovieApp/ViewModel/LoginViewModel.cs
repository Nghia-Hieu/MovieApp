using MovieApp.Models;
using MovieApp.ModelView;
using MovieApp.Views;
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
    internal class LoginViewModel :BaseViewModel
    {
        public bool isLogin {  get; set; }

        private User _UserAccount;
        public User UserAccount { get { return _UserAccount; } set { _UserAccount = value; OnPropertyChanged(); } }
        public ICommand LoginCommand { get; set; }

        public ICommand ToRegisterCommand { get; set; }

        public ICommand PasswordChangedCommand { get; set; }


        private string _Username;
        public string Username { get => _Username; set { _Username = value; OnPropertyChanged(); } }


        private string _Password;
        public string Password { get => _Password; set { _Password = value; OnPropertyChanged(); } }

        public LoginViewModel() {
            isLogin = false;
            Password = "";
            Username = "";

            LoginCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { Login(p); });

            ToRegisterCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { RegisterWindow registerWindow = new RegisterWindow(); registerWindow.ShowDialog();  });

            PasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { Password = p.Password; });
        }

        void Login(Window p)
        {
            if (p == null)
                return;
            var encodePass = PassEncode(Password);
            var accCount = DataProvider.Ins.DB.Users.Where(x=> x.username == Username && x.password == encodePass);

            if (accCount.Count() > 0)
            {
                isLogin = true;
                MessageBox.Show("LOGIN SUCCESS");
                isLogin = true;
                UserAccount = new User();
                UserAccount = accCount.First();
                
                p.Close();
            }
            else
            {
                isLogin = false;
                MessageBox.Show("FAILED");
            }

        }
        public static string PassEncode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
