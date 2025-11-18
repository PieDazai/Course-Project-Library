using Data.Interfaces;
using Domain.Statistics;

namespace Services
{
    public class StatisticsService
    {

        private readonly ILoanRepository _loanRepository;
        private readonly IReaderRepository _readerRepository;
        public StatisticsService(ILoanRepository loanRepository, IReaderRepository readerRepository) 
        {
            _loanRepository = loanRepository;
            _readerRepository = readerRepository;
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
                })
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
                .ToList();
        }

        public List<ReaderCategoryStatisticItem> GetReadersByAgeGroupBy(ReaderFilter filter)
        {
            var readers = _readerRepository.GetAll(filter);
            var totalReaders = readers.Count();

            return readers
                .GroupBy(r => GetAgeCategoryKey(CalculateAge(r.BirthDate)))
                .Select(g => new ReaderCategoryStatisticItem
                {
                    AgeStart = g.Key.AgeStart,
                    AgeEnd = g.Key.AgeEnd,
                    Count = g.Count(),
                    Percent = totalReaders > 0 ? Math.Round((double)g.Count() / totalReaders * 100, 1) : 0
                })
                .OrderBy(s => s.AgeStart)
                .ToList();
        }

        private int CalculateAge(DateOnly birthDate)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - birthDate.Year;

            if (birthDate > today.AddYears(-age))
                age--;

            return age;
        }

        private (int AgeStart, int AgeEnd) GetAgeCategoryKey(int age)
        {
            return age switch
            {
                <= 10 => (0, 10),
                <= 16 => (11, 16),
                <= 20 => (17, 20),
                <= 30 => (21, 30),
                <= 45 => (31, 45),
                <= 60 => (46, 60),
                _ => (61, 999)
            };
        }

    }
}
