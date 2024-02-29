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
    internal class LoginViewModel :BaseViewModel
    {
        public bool isLogin {  get; set; }
        public ICommand LoginCommand { get; set; }
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
            PasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { Password = p.Password; });
        }

        void Login(Window p)
        {
            if (p == null)
                return;

            var accCount = DataProvider.Ins.DB.Users.Where(x=> x.username == Username && x.password == Password).Count();

            if (accCount > 0)
            {
                isLogin = true;
                MessageBox.Show("LOGIN SUCCESS");
                p.Close();
            }
            else
            {
                isLogin = false;
                MessageBox.Show("FAILED");
            }

        }
    }
}
