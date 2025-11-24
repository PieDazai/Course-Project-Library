using Data.Interfaces;
using Domain;

namespace LibraryDB.Data.SqlServer
{
    public class LoanRepository : ILoanRepository
    {
        private readonly LibraryDbContext _dbContext;
        public LoanRepository(LibraryDbContext context) 
        {
            _dbContext = context;
        }

        public bool Add(Loan loan)
        {
            _dbContext.Loans.Add(loan);
            _dbContext.SaveChanges();
            return true;
        }

        public List<Loan>? GetAll(LoanFilter filter)
        {
            var query = _dbContext.Loans.AsQueryable();

            if (filter.StartDate.HasValue)
                query = query.Where(l => l.IssuanceDate >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(l => l.IssuanceDate <= filter.EndDate.Value);

            if (filter.ReaderId.HasValue)
                query = query.Where(l => l.Reader.Id == filter.ReaderId.Value);

            if (filter.BookId.HasValue)
                query = query.Where(l => l.Book.Id == filter.BookId.Value);

            if (!string.IsNullOrEmpty(filter.Status))
                query = query.Where(l => l.Status == filter.Status);

            return query.ToList();
        }

        public Loan GetById(int id)
        {
            return _dbContext.Loans.Find(id);
        }

        public bool Update(Loan loan)
        {
            var temp = GetById(loan.Id);
            if (temp == null) return false;

            _dbContext.Entry(temp).CurrentValues.SetValues(loan);
            _dbContext.SaveChanges();
            return true;
        }
    }
}
