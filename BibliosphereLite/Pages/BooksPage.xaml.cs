using BibliosphereLite.Data;
using BibliosphereLite.Models;
using System.Windows;
using System.Windows.Controls;

namespace BibliosphereLite.Pages
{
    public partial class BooksPage : Page
    {
        private readonly LibraryDatabase libraryDatabase;

        public BooksPage()
        {
            InitializeComponent();
            libraryDatabase = new LibraryDatabase();
            RefreshBooks();
        }

        public void RefreshBooks()
        {
            BooksItemsControl.ItemsSource = libraryDatabase.LoadBooks();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddBookPage(this));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Book selectedBook)
            {
                NavigationService.Navigate(new EditBookPage(selectedBook, this));
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Book selectedBook)
            {
                if (MessageBox.Show($"Вы уверены, что хотите удалить книгу '{selectedBook.Title}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        libraryDatabase.RemoveBook(selectedBook.Id);
                        RefreshBooks();
                    }
                    catch
                    {
                        MessageBox.Show("Ошибка при удалении книги.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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