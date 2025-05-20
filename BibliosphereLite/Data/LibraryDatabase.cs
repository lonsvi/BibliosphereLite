using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using BibliosphereLite.Models;
using System.IO;
using System.Windows;

namespace BibliosphereLite.Data
{
    public class LibraryDatabase
    {
        private readonly string connectionString;
        private readonly string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "error.log");

        public LibraryDatabase()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "library.db");
            connectionString = $"Data Source={dbPath};Version=3;";
            InitializeDatabase();
        }

        public string ConnectionString => connectionString;

        private void InitializeDatabase()
        {
            try
            {
                if (!File.Exists("library.db"))
                {
                    SQLiteConnection.CreateFile("library.db");
                }

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    // Таблица книг
                    string createBooksTableQuery = @"
                CREATE TABLE IF NOT EXISTS Books (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Author TEXT NOT NULL,
                    Year INTEGER NOT NULL,
                    Genre TEXT,
                    Publisher TEXT,
                    ISBN TEXT,
                    Description TEXT,
                    PageCount INTEGER,
                    IsRented INTEGER NOT NULL DEFAULT 0
                )";
                    using (var command = new SQLiteCommand(createBooksTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Проверяем и добавляем новые столбцы, если их нет
                    string checkColumnQuery = "PRAGMA table_info(Books)";
                    bool hasGenreColumn = false, hasPublisherColumn = false, hasISBNColumn = false, hasIsRentedColumn = false, hasDescriptionColumn = false, hasPageCountColumn = false;
                    using (var command = new SQLiteCommand(checkColumnQuery, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string columnName = reader.GetString(1);
                                if (columnName == "Genre") hasGenreColumn = true;
                                if (columnName == "Publisher") hasPublisherColumn = true;
                                if (columnName == "ISBN") hasISBNColumn = true;
                                if (columnName == "IsRented") hasIsRentedColumn = true;
                                if (columnName == "Description") hasDescriptionColumn = true;
                                if (columnName == "PageCount") hasPageCountColumn = true;
                            }
                        }
                    }

                    if (!hasGenreColumn)
                    {
                        string alterQuery = "ALTER TABLE Books ADD COLUMN Genre TEXT";
                        using (var command = new SQLiteCommand(alterQuery, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    if (!hasPublisherColumn)
                    {
                        string alterQuery = "ALTER TABLE Books ADD COLUMN Publisher TEXT";
                        using (var command = new SQLiteCommand(alterQuery, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    if (!hasISBNColumn)
                    {
                        string alterQuery = "ALTER TABLE Books ADD COLUMN ISBN TEXT";
                        using (var command = new SQLiteCommand(alterQuery, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    if (!hasDescriptionColumn)
                    {
                        string alterQuery = "ALTER TABLE Books ADD COLUMN Description TEXT";
                        using (var command = new SQLiteCommand(alterQuery, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    if (!hasPageCountColumn)
                    {
                        string alterQuery = "ALTER TABLE Books ADD COLUMN PageCount INTEGER";
                        using (var command = new SQLiteCommand(alterQuery, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    if (!hasIsRentedColumn)
                    {
                        string alterQuery = "ALTER TABLE Books ADD COLUMN IsRented INTEGER NOT NULL DEFAULT 0";
                        using (var command = new SQLiteCommand(alterQuery, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    // Таблица читателей
                    string createReadersTableQuery = @"
                CREATE TABLE IF NOT EXISTS Readers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Phone TEXT NOT NULL
                )";
                    using (var command = new SQLiteCommand(createReadersTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Таблица абонементов
                    string createSubscriptionsTableQuery = @"
                CREATE TABLE IF NOT EXISTS Subscriptions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ReaderId INTEGER NOT NULL,
                    BookId INTEGER NOT NULL,
                    IssueDate TEXT NOT NULL,
                    ReturnDate TEXT NOT NULL,
                    IsActive INTEGER NOT NULL DEFAULT 1,
                    FOREIGN KEY (ReaderId) REFERENCES Readers(Id),
                    FOREIGN KEY (BookId) REFERENCES Books(Id)
                )";
                    using (var command = new SQLiteCommand(createSubscriptionsTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Таблица пользователей
                    string createUsersTableQuery = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL
                )";
                    using (var command = new SQLiteCommand(createUsersTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Добавляем начального пользователя (admin/admin)
                    string checkUserQuery = "SELECT COUNT(*) FROM Users WHERE Username = 'admin'";
                    using (var command = new SQLiteCommand(checkUserQuery, connection))
                    {
                        long count = (long)command.ExecuteScalar();
                        if (count == 0)
                        {
                            string insertUserQuery = "INSERT INTO Users (Username, Password) VALUES ('admin', 'admin')";
                            using (var insertCommand = new SQLiteCommand(insertUserQuery, connection))
                            {
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to initialize database: {ex.Message}");
                throw;
            }
        }

        public void AddBook(Book book)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string insertQuery = "INSERT INTO Books (Title, Author, Year, Genre, Publisher, ISBN, Description, PageCount, IsRented) VALUES (@Title, @Author, @Year, @Genre, @Publisher, @ISBN, @Description, @PageCount, @IsRented)";
                    using (var command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Title", book.Title);
                        command.Parameters.AddWithValue("@Author", book.Author);
                        command.Parameters.AddWithValue("@Year", book.Year);
                        command.Parameters.AddWithValue("@Genre", book.Genre ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Publisher", book.Publisher ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ISBN", book.ISBN ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Description", book.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@PageCount", book.PageCount);
                        command.Parameters.AddWithValue("@IsRented", book.IsRented ? 1 : 0);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Книга успешно добавлена в базу данных.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось добавить книгу: затронуто 0 строк.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to add book: {ex.Message}");
                MessageBox.Show($"Ошибка при добавлении книги: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        public void RemoveBook(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    // Проверяем, есть ли активные абонементы на эту книгу
                    string checkQuery = "SELECT COUNT(*) FROM Subscriptions WHERE BookId = @BookId AND IsActive = 1";
                    using (var checkCommand = new SQLiteCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@BookId", id);
                        long count = (long)checkCommand.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show("Нельзя удалить книгу, так как она находится в аренде.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    string deleteQuery = "DELETE FROM Books WHERE Id = @Id";
                    using (var command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to remove book: {ex.Message}");
                throw;
            }
        }

        public void EditBook(Book book)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string updateQuery = "UPDATE Books SET Title = @Title, Author = @Author, Year = @Year, Genre = @Genre, Publisher = @Publisher, ISBN = @ISBN, Description = @Description, PageCount = @PageCount, IsRented = @IsRented WHERE Id = @Id";
                    using (var command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", book.Id);
                        command.Parameters.AddWithValue("@Title", book.Title);
                        command.Parameters.AddWithValue("@Author", book.Author);
                        command.Parameters.AddWithValue("@Year", book.Year);
                        command.Parameters.AddWithValue("@Genre", book.Genre ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Publisher", book.Publisher ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ISBN", book.ISBN ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Description", book.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@PageCount", book.PageCount);
                        command.Parameters.AddWithValue("@IsRented", book.IsRented ? 1 : 0);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to edit book: {ex.Message}");
                throw;
            }
        }

        public ObservableCollection<Book> LoadBooks()
        {
            var books = new ObservableCollection<Book>();
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string selectQuery = "SELECT Id, Title, Author, Year, Genre, Publisher, ISBN, Description, PageCount, IsRented FROM Books";
                    using (var command = new SQLiteCommand(selectQuery, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                books.Add(new Book
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Author = reader.GetString(reader.GetOrdinal("Author")),
                                    Year = reader.GetInt32(reader.GetOrdinal("Year")),
                                    Genre = reader.IsDBNull(reader.GetOrdinal("Genre")) ? null : reader.GetString(reader.GetOrdinal("Genre")),
                                    Publisher = reader.IsDBNull(reader.GetOrdinal("Publisher")) ? null : reader.GetString(reader.GetOrdinal("Publisher")),
                                    ISBN = reader.IsDBNull(reader.GetOrdinal("ISBN")) ? null : reader.GetString(reader.GetOrdinal("ISBN")),
                                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                    PageCount = reader.IsDBNull(reader.GetOrdinal("PageCount")) ? 0 : reader.GetInt32(reader.GetOrdinal("PageCount")),
                                    IsRented = reader.GetInt32(reader.GetOrdinal("IsRented")) == 1
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to load books: {ex.Message}");
                MessageBox.Show($"Ошибка при загрузке книг: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return books;
        }

        public void UpdateBookRentalStatus(int bookId, bool isRented)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string updateQuery = "UPDATE Books SET IsRented = @IsRented WHERE Id = @Id";
                    using (var command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", bookId);
                        command.Parameters.AddWithValue("@IsRented", isRented ? 1 : 0);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to update book rental status: {ex.Message}");
                throw;
            }
        }

        public void AddReader(Reader reader)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string insertQuery = "INSERT INTO Readers (Name, Phone) VALUES (@Name, @Phone)";
                    using (var command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", reader.Name);
                        command.Parameters.AddWithValue("@Phone", reader.Phone);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Читатель успешно добавлен в базу данных.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось добавить читателя: затронуто 0 строк.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to add reader: {ex.Message}");
                MessageBox.Show($"Ошибка при добавлении читателя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        public void RemoveReader(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    // Проверяем, есть ли активные абонементы у читателя
                    string checkQuery = "SELECT COUNT(*) FROM Subscriptions WHERE ReaderId = @ReaderId AND IsActive = 1";
                    using (var checkCommand = new SQLiteCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@ReaderId", id);
                        long count = (long)checkCommand.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show("Нельзя удалить читателя, так как у него есть активные абонементы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    string deleteQuery = "DELETE FROM Readers WHERE Id = @Id";
                    using (var command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to remove reader: {ex.Message}");
                throw;
            }
        }

        public ObservableCollection<Reader> LoadReaders()
        {
            var readers = new ObservableCollection<Reader>();
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string selectQuery = @"
                SELECT r.Id, r.Name, r.Phone, COUNT(s.Id) as RentedBooks
                FROM Readers r
                LEFT JOIN Subscriptions s ON r.Id = s.ReaderId AND s.IsActive = 1
                GROUP BY r.Id";
                    using (var command = new SQLiteCommand(selectQuery, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                readers.Add(new Reader
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                    RentedBooks = reader.GetInt32(reader.GetOrdinal("RentedBooks"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to load readers: {ex.Message}");
                MessageBox.Show($"Ошибка при загрузке читателей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return readers;
        }

        public void AddSubscription(Subscription subscription)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    // Проверяем, не арендована ли книга
                    string checkBookQuery = "SELECT IsRented FROM Books WHERE Id = @BookId";
                    using (var checkCommand = new SQLiteCommand(checkBookQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@BookId", subscription.BookId);
                        var result = checkCommand.ExecuteScalar();
                        if (result != null && Convert.ToInt32(result) == 1)
                        {
                            MessageBox.Show("Эта книга уже арендована.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    // Проверяем, нет ли у читателя этой книги уже в аренде
                    string checkSubscriptionQuery = "SELECT COUNT(*) FROM Subscriptions WHERE ReaderId = @ReaderId AND BookId = @BookId AND IsActive = 1";
                    using (var checkCommand = new SQLiteCommand(checkSubscriptionQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@ReaderId", subscription.ReaderId);
                        checkCommand.Parameters.AddWithValue("@BookId", subscription.BookId);
                        long count = (long)checkCommand.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show("Читатель уже арендовал эту книгу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    // Добавляем абонемент
                    string insertQuery = "INSERT INTO Subscriptions (ReaderId, BookId, IssueDate, ReturnDate, IsActive) VALUES (@ReaderId, @BookId, @IssueDate, @ReturnDate, @IsActive)";
                    using (var command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ReaderId", subscription.ReaderId);
                        command.Parameters.AddWithValue("@BookId", subscription.BookId);
                        command.Parameters.AddWithValue("@IssueDate", subscription.IssueDate.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@ReturnDate", subscription.ReturnDate.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@IsActive", subscription.IsActive ? 1 : 0);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            // Обновляем статус книги
                            UpdateBookRentalStatus(subscription.BookId, true);
                            MessageBox.Show("Абонемент успешно добавлен в базу данных.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось добавить абонемент: затронуто 0 строк.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to add subscription: {ex.Message}");
                MessageBox.Show($"Ошибка при добавлении абонемента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        public void ReturnBook(int subscriptionId)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    // Получаем BookId перед возвратом
                    int bookId = 0;
                    string selectQuery = "SELECT BookId FROM Subscriptions WHERE Id = @Id";
                    using (var selectCommand = new SQLiteCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@Id", subscriptionId);
                        var result = selectCommand.ExecuteScalar();
                        if (result != null)
                        {
                            bookId = Convert.ToInt32(result);
                        }
                    }

                    // Обновляем статус абонемента
                    string updateQuery = "UPDATE Subscriptions SET IsActive = 0 WHERE Id = @Id";
                    using (var command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", subscriptionId);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            // Обновляем статус книги
                            UpdateBookRentalStatus(bookId, false);
                            MessageBox.Show("Книга успешно возвращена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to return book: {ex.Message}");
                MessageBox.Show($"Ошибка при возврате книги: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        public void ExtendSubscription(int subscriptionId)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string updateQuery = "UPDATE Subscriptions SET ReturnDate = @ReturnDate WHERE Id = @Id";
                    using (var command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", subscriptionId);
                        command.Parameters.AddWithValue("@ReturnDate", DateTime.Now.AddDays(30).ToString("yyyy-MM-dd"));
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Срок аренды успешно продлён на 30 дней.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to extend subscription: {ex.Message}");
                MessageBox.Show($"Ошибка при продлении аренды: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        public ObservableCollection<Subscription> LoadSubscriptions()
        {
            var subscriptions = new ObservableCollection<Subscription>();
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string selectQuery = @"
                SELECT s.Id, s.ReaderId, s.BookId, s.IssueDate, s.ReturnDate, s.IsActive,
                       r.Name as ReaderName, b.Title as BookTitle
                FROM Subscriptions s
                JOIN Readers r ON s.ReaderId = r.Id
                JOIN Books b ON s.BookId = b.Id";
                    using (var command = new SQLiteCommand(selectQuery, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                subscriptions.Add(new Subscription
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    ReaderId = reader.GetInt32(reader.GetOrdinal("ReaderId")),
                                    BookId = reader.GetInt32(reader.GetOrdinal("BookId")),
                                    IssueDate = DateTime.Parse(reader.GetString(reader.GetOrdinal("IssueDate"))),
                                    ReturnDate = DateTime.Parse(reader.GetString(reader.GetOrdinal("ReturnDate"))),
                                    IsActive = reader.GetInt32(reader.GetOrdinal("IsActive")) == 1,
                                    ReaderName = reader.GetString(reader.GetOrdinal("ReaderName")),
                                    BookTitle = reader.GetString(reader.GetOrdinal("BookTitle"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to load subscriptions: {ex.Message}");
                MessageBox.Show($"Ошибка при загрузке абонементов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return subscriptions;
        }

        public bool AuthenticateUser(string username, string password)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string selectQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password";
                    using (var command = new SQLiteCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);
                        long count = (long)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to authenticate user: {ex.Message}");
                throw;
            }
        }

        private void LogError(string message)
        {
            try
            {
                if (!Directory.Exists("Logs"))
                {
                    Directory.CreateDirectory("Logs");
                }
                File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}\n");
            }
            catch
            {
                // Игнорируем ошибки логирования
            }
        }
    }
}