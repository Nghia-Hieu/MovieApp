using MovieApp.ModelView;
using MovieApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MovieApp.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public bool isLoaded = false;
        
        public ICommand ToLoginCommand { get; set; }
        public MainViewModel() {
            if(isLoaded) {
                isLoaded = true;
                MainWindow mainWindow = new MainWindow();
                mainWindow.ShowDialog();
            }
            ToLoginCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { LoginWindow loginWindow = new LoginWindow(); loginWindow.ShowDialog(); });
        }

        
    }
}
