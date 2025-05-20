using BibliosphereLite.Data;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace BibliosphereLite.Pages
{
    public partial class LoginPage : Page
    {
        private readonly LibraryDatabase libraryDatabase;

        public LoginPage()
        {
            InitializeComponent();
            libraryDatabase = new LibraryDatabase();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            try
            {
                if (libraryDatabase.AuthenticateUser(username, password))
                {
                    // Используем NavigationService, который привязан к Frame
                    NavigationService.Navigate(new Uri("Pages/BooksPage.xaml", UriKind.Relative));
                }
                else
                {
                    MessageBox.Show("Неверное имя пользователя или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка авторизации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}