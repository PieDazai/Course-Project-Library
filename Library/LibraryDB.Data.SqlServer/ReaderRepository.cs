using Data.Interfaces;
using Domain;
using System.Text.RegularExpressions;

namespace LibraryDB.Data.SqlServer
{
    public class ReaderRepository : IReaderRepository
    {
        private readonly LibraryDbContext _dbContext;
        private static readonly string PHONE_NUMBER_FORMAT = @"(\d{1})(\d{3})(\d{3})(\d{2})(\d{2})";

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

        public string FormatNumber(string number)
        {
            string result = Regex.Replace(number, @"[^\d]", "");
            return Regex.Replace(result, PHONE_NUMBER_FORMAT, "$1 ($2) $3-$4-$5");
        }

        public List<Reader> GetAll(ReaderFilter filter)
        {
            var query = _dbContext.Readers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.FullName)) query = query
                    .Where(r => r.FullName.Contains(filter.FullName));

            if (!string.IsNullOrWhiteSpace(filter.PhoneNumber)) query = query
                    .Where(r => Regex.Replace(r.PhoneNumber, @"\D", "").Contains(filter.PhoneNumber));

            if (filter.TicketNumber.HasValue) query = query
                    .Where(r => r.Id == filter.TicketNumber);



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
