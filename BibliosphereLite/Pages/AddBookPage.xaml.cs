using BibliosphereLite.Data;
using BibliosphereLite.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BibliosphereLite.Pages
{
    public partial class AddBookPage : Page
    {
        private readonly LibraryDatabase libraryDatabase;
        private readonly BooksPage booksPage;

        public AddBookPage(BooksPage booksPage)
        {
            InitializeComponent();
            libraryDatabase = new LibraryDatabase();
            this.booksPage = booksPage;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text) || string.IsNullOrWhiteSpace(AuthorTextBox.Text) || string.IsNullOrWhiteSpace(YearTextBox.Text))
            {
                MessageBox.Show("Все поля (Название, Автор, Год) должны быть заполнены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(YearTextBox.Text, out int year) || year < 0 || year > DateTime.Now.Year)
            {
                MessageBox.Show("Год должен быть числом от 0 до текущего года.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(PageCountTextBox.Text, out int pageCount) || pageCount < 0)
            {
                MessageBox.Show("Количество страниц должно быть положительным числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var book = new Book
                {
                    Title = TitleTextBox.Text,
                    Author = AuthorTextBox.Text,
                    Year = year,
                    Genre = GenreTextBox.Text,
                    Publisher = PublisherTextBox.Text,
                    ISBN = ISBNTextBox.Text,
                    Description = DescriptionTextBox.Text,
                    PageCount = pageCount,
                    IsRented = false
                };
                libraryDatabase.AddBook(book);
                booksPage.RefreshBooks();
                NavigationService.Navigate(new BooksPage());
            }
            catch
            {
                MessageBox.Show("Ошибка при добавлении книги.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new BooksPage());
        }
    }
}