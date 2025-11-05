using Data.Interfaces;
using Domain;

namespace Data.InMemory
{
    public class LoanRepository : ILoanRepository
    {
        private List<Loan> LoansList = new List<Loan>();
        private int CurrentId = 0;

        public bool Add(Loan loan)
        {
            if(loan != null)
            {
                LoansList.Add(loan);
                return true;
            }
            return false;
        }


        public List<Loan> GetAll(LoanFilter filter)
        {
            var result = LoansList.AsEnumerable();

            if (filter.StartDate.HasValue)
                result = result.Where(l => l.IssuanceDate >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                result = result.Where(l => l.IssuanceDate <= filter.EndDate.Value);

            if (filter.ReaderId.HasValue)
                result = result.Where(l => l.Reader.Id == filter.ReaderId.Value);

            if (filter.BookId.HasValue)
                result = result.Where(l => l.Book.Id == filter.BookId.Value);

            if (!string.IsNullOrEmpty(filter.Status))
                result = result.Where(l => l.Status == filter.Status);

            return result.ToList();
        }

        public Loan GetById(int id)
        {
            return LoansList.FirstOrDefault(l => l.Id == id);
        }

        public bool Update(Loan loan)
        {
            int index = LoansList.IndexOf(GetById(loan.Id));
            if(index != -1)
            {
                LoansList[index] = loan;
                return true;
            }
            return false;
        }
    }
}
