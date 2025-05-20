using BibliosphereLite.Data;
using BibliosphereLite.Models;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace BibliosphereLite.Pages
{
    public partial class AddReaderPage : Page
    {
        private readonly LibraryDatabase libraryDatabase;
        private readonly ReadersPage readersPage;

        public AddReaderPage(ReadersPage readersPage)
        {
            InitializeComponent();
            libraryDatabase = new LibraryDatabase();
            this.readersPage = readersPage;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) || string.IsNullOrWhiteSpace(PhoneTextBox.Text))
            {
                MessageBox.Show("Все поля должны быть заполнены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Regex.IsMatch(PhoneTextBox.Text, @"^\+7\d{10}$"))
            {
                MessageBox.Show("Номер телефона должен быть в формате +7xxxxxxxxxx (10 цифр).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var reader = new Reader
                {
                    Name = NameTextBox.Text,
                    Phone = PhoneTextBox.Text
                };
                libraryDatabase.AddReader(reader);
                readersPage.RefreshReaders();
                NavigationService.Navigate(new ReadersPage());
            }
            catch
            {
                MessageBox.Show("Ошибка при добавлении читателя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ReadersPage());
        }
    }
}