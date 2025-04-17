using System;
using System.Windows;
using TestGG.Pages;
using TestGG.Usercontrols;

namespace TestGG
{
    public partial class MainWindow : Window
    {
        private SideBarController _navControl;

        public MainWindow()
        {
            InitializeComponent();
            _navControl = SideBar; 

           
            _navControl.ButtonClicked += NavButtons_ButtonClicked;
        }

        private void NavButtons_ButtonClicked(object sender, string pageName)
        {
            Navigate(pageName);
        }

        public void Navigate(string view)
        {
            _navControl.SetBackground(view);
            switch (view)
            {
                case "Home":
                    MainFrame.Content = new HomeView();
                    break;
                case "Editor":
                    MainFrame.Content = new EditorPage();
                    break;
                case "Settings":
                    MainFrame.Content = new SettingsView();
                    break;
                default:
                    MainFrame.Content = new HomeView(); 
                    break;
            }
            
        }
    }
}
