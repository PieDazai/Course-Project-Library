using Data.Interfaces;
using Domain;
using Services;
using System.Windows;
using System.Windows.Controls;

namespace UI
{
    public partial class ReaderTicketWindow : Window
    {
        private Reader _reader;
        private LoanFilter _filter;
        private readonly ILoanRepository _loanRepository;
        private readonly IBookRepository _bookRepository;
        private readonly LoanService _loanService;

        public ReaderTicketWindow(Reader reader, IBookRepository bookRepository, ILoanRepository loanRepository, LoanService loanService)
        {
            _loanRepository = loanRepository;
            _loanService = loanService;
            _reader = reader;
            _bookRepository = bookRepository;
            _filter = new LoanFilter { ReaderId = _reader.Id };
            InitializeComponent();
            LoadDataByReader();
            RefreshDataGridByReader();
        }

        private void RefreshDataGridByReader()
        {
            LoansDataGrid.ItemsSource = _loanRepository.GetAll(_filter);
        }

        private void LoansDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReturnBookButton.IsEnabled = LoansDataGrid.SelectedItem is Loan loan;
        }

        private void IssueBookButton_Click(object sender, RoutedEventArgs e)
        {
            IssueBookWindow issueBookWindow = new IssueBookWindow(_bookRepository, _reader, _loanRepository, _loanService);
            issueBookWindow.ShowDialog();
            RefreshDataGridByReader();
        }

        private void ReturnBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoansDataGrid.SelectedItem is Loan loan)
            {
                if (loan.Status == "Завершен")
                {
                    MessageBox.Show("Выдача уже завершена!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    ReturnBookWindow returnBookWindow = new ReturnBookWindow(loan, _loanRepository, _loanService);
                    returnBookWindow.ShowDialog();
                    RefreshDataGridByReader();
                }
            }
            else
            {
                MessageBox.Show("Выберите выдачу", "Подсказка", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void LoadDataByReader()
        {
            ReaderNameText.Text = _reader.FullName;
            TicketTitleText.Text = $"Читательский билет № {_reader.Id}";
            EmailText.Text = _reader.Email;
            PhoneNumberText.Text = _reader.PhoneNumber;
            BirthdayText.Text = _reader.BirthDate.ToString("dd.MM.yyyy");
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            _filter = new LoanFilter { Status = "В прокате", ReaderId = _reader.Id};
            RefreshDataGridByReader();
        }

        private void UnCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            _filter = new LoanFilter { ReaderId = _reader.Id };
            RefreshDataGridByReader();
        }
    }
}