using BibliosphereLite.Data;
using BibliosphereLite.Models;
using System.Windows;
using System.Windows.Controls;

namespace BibliosphereLite.Pages
{
    public partial class ReadersPage : Page
    {
        private readonly LibraryDatabase libraryDatabase;

        public ReadersPage()
        {
            InitializeComponent();
            libraryDatabase = new LibraryDatabase();
            RefreshReaders();
        }

        public void RefreshReaders()
        {
            ReadersItemsControl.ItemsSource = libraryDatabase.LoadReaders();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddReaderPage(this));
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Reader selectedReader)
            {
                if (selectedReader.RentedBooks > 0)
                {
                    MessageBox.Show("Нельзя удалить читателя, у которого есть активные абонементы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (MessageBox.Show($"Вы уверены, что хотите удалить читателя '{selectedReader.Name}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        libraryDatabase.RemoveReader(selectedReader.Id);
                        RefreshReaders();
                    }
                    catch
                    {
                        MessageBox.Show("Ошибка при удалении читателя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BooksNavButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new BooksPage());
        }

        private void ReadersNavButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ReadersPage());
        }

        private void SubscriptionsNavButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SubscriptionsPage());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }
    }
}