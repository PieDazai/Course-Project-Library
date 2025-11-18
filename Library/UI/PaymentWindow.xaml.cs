using Data.Interfaces;
using Domain;
using Services;
using System.Windows;
using System.Windows.Controls;

namespace UI
{
    public partial class PaymentWindow : Window
    {

        private readonly ILoanRepository _loanRepository;
        private readonly LoanService _loanService;
        private Book _book;
        private Reader _reader;
        public PaymentWindow(ILoanRepository loanRepository, LoanService loanService, Book book, Reader reader)
        {
            _book = book;
            _reader = reader;
            _loanRepository = loanRepository;
            _loanService = loanService;
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            if (isValid())
            {
                if (_loanRepository.Add(_loanService.IssueBook(_book, _reader)))
                {
                    MessageBox.Show($"Книга выдана", "Успех",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Ошибка при выдаче книги", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            DialogResult = true;
            Close();
        }

        private bool isValid()
        {
            return true;
        }

        private void CardHolderTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void CvvTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ExpiryDateTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ExpiryDateTextBox_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void ExpiryDateTextBox_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void CardNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void PaymentMethodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
       
        }

    }
}
