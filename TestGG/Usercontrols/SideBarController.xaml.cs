using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace TestGG.Usercontrols
{
    public partial class SideBarController : UserControl
    {
        public event EventHandler<string> ButtonClicked;
        private static Button _previousButton = null;

        public SideBarController()
        {
            InitializeComponent();
        }

        public static void ApplyCircularStyle(Button button)
        {
            if (button == null) return;

            if (_previousButton != null && _previousButton != button)
            {
                ResetButtonStyle(_previousButton);
            }

            button.Width = 50;
            button.Height = 50;
            button.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(128, 0, 0, 0));
            button.BorderThickness = new Thickness(0);
            button.Clip = new EllipseGeometry(new Rect(0, 0, 50, 50));

            button.Effect = new DropShadowEffect
            {
                Color = Colors.Black,
                BlurRadius = 10,
                ShadowDepth = 2,
                Opacity = 0.5
            };

            _previousButton = button;
        }

        public static void ResetButtonStyle(Button button)
        {
            if (button == null) return;

            button.Background = Brushes.Transparent;
            button.BorderBrush = Brushes.Transparent;
            button.BorderThickness = new Thickness(0);
            button.Width = double.NaN;
            button.Height = double.NaN;
            button.Clip = null;
            button.Effect = null;
        }

        public void SetBackground(String View)
        {
            switch (View)
            {
                case "Home":
                    ApplyCircularStyle(HomeBtn);
                    break;
                case "Editor":
                    ApplyCircularStyle(CodeBtn);
                    break;
                case "Settings":
                    ApplyCircularStyle(SettingsBtn);
                    break;
                default:
                    ApplyCircularStyle(HomeBtn);
                    break;
            }
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke(this, "Home");
           
        }

        private void CodeButton_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke(this, "Editor");
            
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke(this, "Settings");
           
        }
    }
}