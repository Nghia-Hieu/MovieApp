using MovieApp.Models;
using MovieApp.ModelView;
using MovieApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Migrations.Model;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;

namespace MovieApp.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public bool isLoaded = false;
        public double currentProfile = 0;

        private Page _CurrentPage;
        public Page CurrentPage { get { return _CurrentPage; } set { _CurrentPage = value; OnPropertyChanged(); } }
        private string _SearchKey;
        public string SearchKey { get { return _SearchKey; } set { _SearchKey = value; OnPropertyChanged(); } }
        private string _MovieImage;
        public string MovieImage { get { return _MovieImage; } set { _MovieImage = value; OnPropertyChanged(); } }
        private Movie _SelectedMovie;
        public Movie SelectedMovie { get { return _SelectedMovie; } set { _SelectedMovie = value; OnPropertyChanged(); } }

        private Visibility _LoggedInMenu;
        public Visibility LoggedInMenu { get { return _LoggedInMenu; } set { _LoggedInMenu = value; OnPropertyChanged(); } }

        private Visibility _NotLoggedInMenu;
        public Visibility NotLoggedInMenu { get { return _NotLoggedInMenu; } set { _NotLoggedInMenu = value; OnPropertyChanged(); } }

        private Visibility _AdminMenu;
        public Visibility AdminMenu { get { return _AdminMenu; } set { _AdminMenu = value; OnPropertyChanged(); } }

        private User _UserInfo;
        public User UserInfo { get { return _UserInfo; } set { _UserInfo = value; OnPropertyChanged(); } }
        public ICommand SearchCommand { get; set; }
        public ICommand ToLoginCommand { get; set; }
        public ICommand ToMovieListCommand { get; set; }
        public ICommand ToMainPageCommand { get; set; }
        public ICommand ToProfileCommand { get; set; }
        public ICommand LogOutCommand { get; set; }
        public ICommand ToAdminCommand { get; set; }


        public ICommand RemoveBackEntryCommand { get; set; }


        public MainViewModel()
        {
            LoggedInMenu = Visibility.Collapsed;
            NotLoggedInMenu = Visibility.Visible;
            AdminMenu = Visibility.Collapsed;
            CurrentPage = new MovieFront();  
            
            if (isLoaded)
            {
                isLoaded = true;
                MainWindow mainWindow = new MainWindow();
                mainWindow.ShowDialog();
            }

            ToLoginCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { LogIn(); });
            SearchCommand = new RelayCommand<object>((p) => { return true; }, (p) => { Search(); });
            ToMovieListCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CurrentPage = new MovieSearch(); });
            ToMainPageCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CurrentPage = new MovieFront(); });
            RemoveBackEntryCommand = new RelayCommand<object>((p) => { return true; }, (p) => {  });
            ToProfileCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CurrentPage = new ProfilePage(); });
            LogOutCommand = new RelayCommand<object>((p) => { return true; }, (p) => { LogOut(); });
            ToAdminCommand = new RelayCommand<object>((p) => { return true; }, (p) => { AdminWindow adminWindow = new AdminWindow(); adminWindow.ShowDialog(); });

        }

        public void LogIn()
        {
            LoginWindow loginWindow = new LoginWindow(); 
            loginWindow.ShowDialog();


            var loginData = loginWindow.DataContext as LoginViewModel;

            var LoginStatus = loginData.isLogin;
            

            if (LoginStatus == true)
            {
                if(loginData.isAdmin == true)
                {
                    AdminMenu = Visibility.Visible;
                }
                Debug.WriteLine("DONE");
                UserInfo = loginData.UserAccount;
                LoggedInMenu = Visibility.Visible;
                NotLoggedInMenu = Visibility.Collapsed;
            }
            else Debug.WriteLine("Fail");
        }
        public void LogOut()
        {

            LoginWindow loginWindow = new LoginWindow();
            var loginData = loginWindow.DataContext as LoginViewModel;

            loginData.isLogin = false;
            loginData.isAdmin = false;
            UserInfo = null;
            LoggedInMenu = Visibility.Collapsed;
            NotLoggedInMenu = Visibility.Visible;
            AdminMenu = Visibility.Collapsed;
            if(CurrentPage.GetType() == new ProfilePage().GetType())
                CurrentPage = new MovieFront();
        }
        public void Search()
        {
            Debug.WriteLine(SearchKey);
        }      
    }
}
