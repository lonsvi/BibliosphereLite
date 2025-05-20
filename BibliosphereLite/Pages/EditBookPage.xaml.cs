using BibliosphereLite.Data;
using BibliosphereLite.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BibliosphereLite.Pages
{
    public partial class EditBookPage : Page
    {
        private readonly LibraryDatabase libraryDatabase;
        private readonly BooksPage booksPage;
        private readonly Book book;

        public EditBookPage(Book book, BooksPage booksPage)
        {
            InitializeComponent();
            libraryDatabase = new LibraryDatabase();
            this.booksPage = booksPage;
            this.book = book;

            TitleTextBox.Text = book.Title;
            AuthorTextBox.Text = book.Author;
            YearTextBox.Text = book.Year.ToString();
            GenreTextBox.Text = book.Genre;
            PublisherTextBox.Text = book.Publisher;
            ISBNTextBox.Text = book.ISBN;
            DescriptionTextBox.Text = book.Description;
            PageCountTextBox.Text = book.PageCount.ToString();
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
                book.Title = TitleTextBox.Text;
                book.Author = AuthorTextBox.Text;
                book.Year = year;
                book.Genre = GenreTextBox.Text;
                book.Publisher = PublisherTextBox.Text;
                book.ISBN = ISBNTextBox.Text;
                book.Description = DescriptionTextBox.Text;
                book.PageCount = pageCount;
                libraryDatabase.EditBook(book);
                booksPage.RefreshBooks();
                NavigationService.Navigate(new BooksPage());
            }
            catch
            {
                MessageBox.Show("Ошибка при редактировании книги.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new BooksPage());
        }
    }
}