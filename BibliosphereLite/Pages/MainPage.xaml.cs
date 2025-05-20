//using BibliosphereLite.Data;
//using BibliosphereLite.Models;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Navigation;

//namespace BibliosphereLite.Pages
//{
//    public partial class MainPage : Page
//    {
//        private readonly LibraryDatabase libraryDatabase;
//        private Book selectedBook;

//        public MainPage()
//        {
//            InitializeComponent();
//            libraryDatabase = new LibraryDatabase();
//            LoadBooks();
//        }

//        private void LoadBooks()
//        {
//            try
//            {
//                BooksGrid.ItemsSource = libraryDatabase.LoadBooks();
//            }
//            catch
//            {
//                MessageBox.Show("Ошибка при загрузке данных. Проверьте файл базы данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        private void AddButton_Click(object sender, RoutedEventArgs e)
//        {
//            NavigationService.Navigate(new AddBookPage(this));
//        }

//        private void EditButton_Click(object sender, RoutedEventArgs e)
//        {
//            selectedBook = BooksGrid.SelectedItem as Book;
//            if (selectedBook == null)
//            {
//                MessageBox.Show("Выберите книгу для редактирования.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
//                return;
//            }

//            NavigationService.Navigate(new EditBookPage(selectedBook, this));
//        }

//        private void RemoveButton_Click(object sender, RoutedEventArgs e)
//        {
//            selectedBook = BooksGrid.SelectedItem as Book;
//            if (selectedBook == null)
//            {
//                MessageBox.Show("Выберите книгу для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
//                return;
//            }

//            if (MessageBox.Show($"Вы уверены, что хотите удалить книгу '{selectedBook.Title}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
//            {
//                try
//                {
//                    libraryDatabase.RemoveBook(selectedBook.Id);
//                    LoadBooks();
//                }
//                catch
//                {
//                    MessageBox.Show("Ошибка при удалении книги.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
//                }
//            }
//        }

//        public void RefreshBooks()
//        {
//            LoadBooks();
//        }
//    }
//}