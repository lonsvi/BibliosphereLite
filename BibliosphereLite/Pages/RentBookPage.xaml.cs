using BibliosphereLite.Data;
using BibliosphereLite.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace BibliosphereLite.Pages
{
    public partial class RentBookPage : Page
    {
        private readonly LibraryDatabase libraryDatabase;
        private readonly SubscriptionsPage subscriptionsPage;

        public RentBookPage(SubscriptionsPage subscriptionsPage)
        {
            InitializeComponent();
            libraryDatabase = new LibraryDatabase();
            this.subscriptionsPage = subscriptionsPage;
            LoadReadersAndBooks();
        }

        private void LoadReadersAndBooks()
        {
            try
            {
                var readers = libraryDatabase.LoadReaders();
                var books = libraryDatabase.LoadBooks().Where(b => !b.IsRented).ToList();

                ReaderComboBox.ItemsSource = readers;
                BookComboBox.ItemsSource = books;

                // Отладочная информация
                MessageBox.Show($"Загружено читателей: {readers.Count}\nЗагружено доступных книг: {books.Count}", "Отладка", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RentButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReaderComboBox.SelectedItem == null || BookComboBox.SelectedItem == null)
            {
                MessageBox.Show("Все поля должны быть заполнены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var subscription = new Subscription
                {
                    ReaderId = (int)ReaderComboBox.SelectedValue,
                    BookId = (int)BookComboBox.SelectedValue,
                    IssueDate = DateTime.Now,
                    ReturnDate = DateTime.Now.AddDays(30), // Фиксированный срок 30 дней
                    IsActive = true
                };
                libraryDatabase.AddSubscription(subscription);
                subscriptionsPage.RefreshSubscriptions();

                // Обновляем статус книги
                libraryDatabase.UpdateBookRentalStatus(subscription.BookId, true);

                // Обновляем списки на других страницах
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    // Переходим на BooksPage и обновляем
                    mainWindow.MainFrame.Navigate(new Uri("Pages/BooksPage.xaml", UriKind.Relative));
                    if (mainWindow.MainFrame.Content is BooksPage booksPage)
                    {
                        booksPage.RefreshBooks();
                    }

                    // Переходим на ReadersPage и обновляем
                    mainWindow.MainFrame.Navigate(new Uri("Pages/ReadersPage.xaml", UriKind.Relative));
                    if (mainWindow.MainFrame.Content is ReadersPage readersPage)
                    {
                        readersPage.RefreshReaders();
                    }
                }

                // Переходим на SubscriptionsPage
                NavigationService.Navigate(new SubscriptionsPage());
            }
            catch
            {
                MessageBox.Show("Ошибка при выдаче книги.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SubscriptionsPage());
        }
    }
}