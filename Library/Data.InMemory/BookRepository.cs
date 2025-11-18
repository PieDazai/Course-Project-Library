using Data.Interfaces;
using Domain;

namespace Data.InMemory
{
    public class BookRepository : IBookRepository
    {
        private static List<Book> BookList { get; set; }
            = new List<Book>();

        private int CurrentId = 0;

        public bool Add(Book book)
        {
            if(book == null) return false;

            book.Id = ++CurrentId;
            BookList.Add(book);
            return true;
        }

        public bool Delete(Book book)
        {
            if(book == null) return false;

            BookList.Remove(book);
            return true;
        }

        public List<Book> GetAll(BookFilter filter)
        {
            var result = BookList.AsEnumerable();

            if(filter.AvailableOnly == true) result = result.Where(b => b.AvailableCopies > 0);

            if (!string.IsNullOrWhiteSpace(filter.Genre)) result = result
                    .Where(b => b.Genre.Contains(filter.Genre, StringComparison.OrdinalIgnoreCase));  

            if (!string.IsNullOrWhiteSpace(filter.Author)) result = result
        .Where(b => b.Author.Contains(filter.Author, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(filter.Title)) result = result
        .Where(b => b.Title.Contains(filter.Title, StringComparison.OrdinalIgnoreCase));

            return result.ToList();
        }
        
        public Book GetById(int id) => BookList.FirstOrDefault(b => b.Id == id);

        public bool Update(Book book)
        {
            int id = BookList.IndexOf(GetById(book.Id));

            if (id != -1)
            {
                BookList[id] = book;
                return true;
            }

            return false;
        }
}
    }
