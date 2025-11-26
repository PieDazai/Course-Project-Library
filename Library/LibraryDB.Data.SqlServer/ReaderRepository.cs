using Data.Interfaces;
using Domain;

namespace LibraryDB.Data.SqlServer
{
    public class ReaderRepository : IReaderRepository
    {
        private readonly LibraryDbContext _dbContext;

        public ReaderRepository(LibraryDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public bool Add(Reader reader)
        {
            _dbContext.Readers.Add(reader);
            _dbContext.SaveChanges();
            return true;
        }

        public bool Delete(Reader reader)
        {
            if (reader == null) return false;

            _dbContext.Readers.Remove(reader);
            _dbContext.SaveChanges();
            return true;
        }

        public List<Reader> GetAll(ReaderFilter filter)
        {
            var query = _dbContext.Readers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.FullName))
                query = query.Where(r => r.FullName.ToLower().Contains(filter.FullName.ToLower()));

            if (!string.IsNullOrWhiteSpace(filter.PhoneNumber))
            {
                // Убираем все нецифровые символы из поискового запроса
                string digitsOnly = new string(filter.PhoneNumber.Where(char.IsDigit).ToArray());
                query = query.Where(r => r.PhoneNumber.Contains(digitsOnly));
            }

            if (filter.TicketNumber.HasValue)
                query = query.Where(r => r.Id == filter.TicketNumber.Value);

            return query.ToList();
        }

        public Reader GetById(int id)
        {
            return _dbContext.Readers.Find(id);
        }

        public bool Update(Reader reader)
        {
            var temp = GetById(reader.Id);
            if(temp == null) return false;

            _dbContext.Entry(temp).CurrentValues.SetValues(reader);
            _dbContext.SaveChanges();
            return true;
        }
    }
}
