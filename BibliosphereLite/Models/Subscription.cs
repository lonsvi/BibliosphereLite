using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BibliosphereLite.Models
{
    public class Subscription : INotifyPropertyChanged
    {
        private int id;
        private int readerId;
        private int bookId;
        private DateTime issueDate;
        private DateTime returnDate;
        private bool isActive;
        private string readerName;
        private string bookTitle;

        public int Id
        {
            get => id;
            set { id = value; OnPropertyChanged(); }
        }

        public int ReaderId
        {
            get => readerId;
            set { readerId = value; OnPropertyChanged(); }
        }

        public int BookId
        {
            get => bookId;
            set { bookId = value; OnPropertyChanged(); }
        }

        public DateTime IssueDate
        {
            get => issueDate;
            set { issueDate = value; OnPropertyChanged(); }
        }

        public DateTime ReturnDate
        {
            get => returnDate;
            set { returnDate = value; OnPropertyChanged(); }
        }

        public bool IsActive
        {
            get => isActive;
            set { isActive = value; OnPropertyChanged(); }
        }

        public string ReaderName
        {
            get => readerName;
            set { readerName = value; OnPropertyChanged(); }
        }

        public string BookTitle
        {
            get => bookTitle;
            set { bookTitle = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}