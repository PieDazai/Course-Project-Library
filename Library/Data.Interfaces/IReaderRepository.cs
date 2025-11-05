using Domain;

namespace Data.Interfaces
{
    public interface IReaderRepository
    {
        Reader GetById(int id);
        List<Reader> GetAll(ReaderFilter filter);
        bool Delete(Reader reader);
        bool Update(Reader reader);
        bool Add(Reader reader);
        bool ContainsReader(Reader reader);
        string FormatNumber(string number);
    }
}
