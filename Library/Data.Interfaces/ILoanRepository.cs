using Domain;

namespace Data.Interfaces
{
    public interface ILoanRepository
    {
        List<Loan>? GetAll(LoanFilter filter);
        bool Update(Loan loan);
        bool Add(Loan loan);
        Loan GetById(int id);
    }
}
