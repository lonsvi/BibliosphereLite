using System;
using System.Windows;
using System.Windows.Navigation;

namespace BibliosphereLite
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Uri("Pages/LoginPage.xaml", UriKind.Relative));
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            // Пустой метод, так как управление навигацией теперь на страницах
        }
    }
}