namespace BibliosphereLite.Models
{
    public class Reader
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public int RentedBooks { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}