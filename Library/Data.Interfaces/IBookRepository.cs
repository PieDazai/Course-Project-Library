using Domain;

namespace Data.Interfaces
{
    public interface IBookRepository
    {
        List<Book> GetAll(BookFilter filter);
        Book GetById(int id);
        bool Add(Book book);
        bool Update(Book book);
        bool Delete(Book book);
        bool ContainsBook(Book book);

    }
}
