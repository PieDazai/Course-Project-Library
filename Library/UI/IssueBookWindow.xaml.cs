using Data.Interfaces;
using Domain;
using Services;
using System.Windows;
using System.Windows.Controls;

namespace UI
{
    public partial class IssueBookWindow : Window
    {
        private BookFilter _filter = new BookFilter();
        private readonly IBookRepository _bookRepository;
        private readonly Reader _reader;
        private readonly ILoanRepository _loanRepository;
        private readonly LoanService _loanService;
        public IssueBookWindow(IBookRepository bookRepository, Reader reader, ILoanRepository loanRepository, LoanService loanService)
        {
            _reader = reader;
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
            _loanService = loanService;
            InitializeComponent();
            LoadDataByUser();
            RefreshDataGrid(_filter);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void IssueBookButton_Click(object sender, RoutedEventArgs e)
        {

            if (BooksDataGrid.SelectedItem is Book book)
            {

                if (book.AvailableCopies < 1)
                {
                    MessageBox.Show("Нет свободного экземпляра этой книги!", "Ошибка",
                                 MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBoxResult result = MessageBox.Show(
                    $"Выдать книгу:\n" +
                    $"Название: {book.Title}\n" +
                    $"Автор: {book.Author}",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    MessageBoxResult.No);

                if (result == MessageBoxResult.Yes)
                {
                    PaymentWindow paymentWindow = new PaymentWindow(_loanRepository, _loanService, book, _reader);
                    paymentWindow.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Выберите книгу для выдачи", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
            }
            RefreshDataGrid(_filter);
            DialogResult = true;
            Close();
        }

        private void BooksDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(BooksDataGrid.SelectedItem is Book book)
            {
                IssueBookButton.IsEnabled = true;
                SelectedBookDetails.Text = $"Название: {book.Title}\nАвтор: {book.Author}";
                DepositAmountText.Text = book.Deposit.ToString() + " руб";
                TotalAmountText.Text = DepositAmountText.Text;
            }
            else
            {
                IssueBookButton.IsEnabled = false;
                DepositAmountText.Text = DepositAmountText.Text;
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            TitleSearchTextBox.Text = string.Empty;
            AuthorSearchTextBox.Text = string.Empty;
            GenreTextBox.Text = string.Empty;
            RefreshDataGrid(new BookFilter());
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            _filter = new BookFilter
            {
                Author = AuthorSearchTextBox.Text.Trim(),
                Title = TitleSearchTextBox.Text.Trim(),
                Genre = GenreTextBox.Text.Trim()
            };
            RefreshDataGrid(_filter);
        }
        private void LoadDataByUser()
        {
            ReaderNameText.Text = _reader.FullName;
            EmailText.Text = _reader.Email;
            PhoneNumberText.Text = _reader.PhoneNumber;
            BirthdayText.Text = _reader.BirthDate.ToString("dd.MM.yyyy");
        }
        private void RefreshDataGrid(BookFilter filter)
        {
            BooksDataGrid.ItemsSource = _bookRepository.GetAll(filter);
        }
    }
}
