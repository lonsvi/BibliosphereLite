namespace BibliosphereLite.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public string Genre { get; set; }
        public string Publisher { get; set; }
        public string ISBN { get; set; }
        public string Description { get; set; }
        public int PageCount { get; set; }
        public bool IsRented { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}