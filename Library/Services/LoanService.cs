using Data.Interfaces;
using Domain;

namespace Services
{
    public class LoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IBookRepository _bookRepository;

        private int CurrentId = 0;

        public LoanService(ILoanRepository loanRepository, IBookRepository bookRepository)
        {
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
        }

        public Loan? IssueBook(Book book, Reader reader)
        {
            if (book != null && reader != null)
            {
                Loan loan = new Loan();
                loan.Id = ++CurrentId;
                loan.IssuanceDate = DateOnly.FromDateTime(DateTime.Now);
                loan.Status = "В прокате";
                loan.Reader = reader;
                loan.Book = book;
                book.AvailableCopies--;
                _bookRepository.Update(book);

                return loan;
            }
            return null;
        }

        public bool ReturnBook(Loan loan)
        {
            if(loan != null)
            {
                loan.ReturnDate = DateOnly.FromDateTime(DateTime.Now);
                loan.Book.AvailableCopies++;
                loan.Status = "Завершен";
                _bookRepository.Update(loan.Book);
                _loanRepository.Update(loan);
                return true;
            }
            return false;
        }

    }
}
