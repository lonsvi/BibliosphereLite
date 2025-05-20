using BibliosphereLite.Data;
using BibliosphereLite.Models;
using System.Windows;
using System.Windows.Controls;

namespace BibliosphereLite.Pages
{
    public partial class SubscriptionsPage : Page
    {
        private readonly LibraryDatabase libraryDatabase;

        public SubscriptionsPage()
        {
            InitializeComponent();
            libraryDatabase = new LibraryDatabase();
            RefreshSubscriptions();
        }

        public void RefreshSubscriptions()
        {
            SubscriptionsItemsControl.ItemsSource = libraryDatabase.LoadSubscriptions();
        }

        private void RentButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RentBookPage(this));
        }

        private void ExtendButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Subscription subscription)
            {
                if (!subscription.IsActive)
                {
                    MessageBox.Show("Этот абонемент уже закрыт.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                try
                {
                    libraryDatabase.ExtendSubscription(subscription.Id);
                    RefreshSubscriptions();
                }
                catch
                {
                    MessageBox.Show("Ошибка при продлении аренды.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Subscription subscription)
            {
                if (!subscription.IsActive)
                {
                    MessageBox.Show("Этот абонемент уже закрыт.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                try
                {
                    libraryDatabase.ReturnBook(subscription.Id);
                    RefreshSubscriptions();
                }
                catch
                {
                    MessageBox.Show("Ошибка при возврате книги.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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