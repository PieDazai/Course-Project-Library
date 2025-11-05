using Data.Interfaces;
using Domain;
using Domain.Statistics;

namespace Services
{
    public class StatisticsService
    {

        private readonly ILoanRepository _loanRepository;
        private readonly IBookRepository _bookRepository;
        public StatisticsService(ILoanRepository loanRepository, IBookRepository bookRepository) 
        {
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
        }  

        public List<MonthlyRevenueStatisticItem> GetLoansByMonth(LoanFilter filter)
        {
            var loans = _loanRepository.GetAll(filter);
            return loans
                .GroupBy(l => new { l.IssuanceDate.Year, l.IssuanceDate.Month })
                .Select(g => new MonthlyRevenueStatisticItem
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Total = g.Sum(l => l.FinalPrice),
                    RentalsCount = g.Count()
                })
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
                .ToList();
        }



    }
}
