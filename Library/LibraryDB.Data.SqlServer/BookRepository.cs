using Data.Interfaces;
using Domain;

namespace LibraryDB.Data.SqlServer
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryDbContext _dbContext;

        public BookRepository(LibraryDbContext context)
        {
            _dbContext = context;
        }
        public bool Add(Book book)
        {
            _dbContext.Books.Add(book);
            _dbContext.SaveChanges();
            return true;
        }

        public bool Delete(Book book)
        {
            if (book == null) return false; 

            _dbContext.Books.Remove(book);
            _dbContext.SaveChanges();
            return true;

        }

        public List<Book> GetAll(BookFilter filter)
        {
            var query = _dbContext.Books.AsQueryable();

            if (filter.AvailableOnly == true) query = query.Where(b => b.AvailableCopies > 0);

            if (!string.IsNullOrWhiteSpace(filter.Genre)) query = query
                    .Where(b => b.Genre.Contains(filter.Genre, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(filter.Author)) query = query
        .Where(b => b.Author.Contains(filter.Author, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(filter.Title)) query = query
        .Where(b => b.Title.Contains(filter.Title, StringComparison.OrdinalIgnoreCase));

            return query.ToList();
        }

        public Book GetById(int id)
        {
            return _dbContext.Books.Find(id);
        }

        public bool Update(Book book)
        {
            var temp = GetById(book.Id);
            if (temp == null) return false;

            _dbContext.Entry(temp).CurrentValues.SetValues(book);
            _dbContext.SaveChanges();
            return true;
        }
    }
}
