using MovieApp.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MovieApp.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public bool isLoaded = false;
        public MainViewModel() {
            if(isLoaded) {
                isLoaded = true;
                MainWindow mainWindow = new MainWindow();
                mainWindow.ShowDialog();
            }
        }
    }
}
