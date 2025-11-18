using Data.Interfaces;
using Domain;
using Services;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace UI
{
    public partial class PaymentWindow : Window
    {
        private bool _isReturn = false;
        private string ALL_DIGIT = @"^\d+$";
        private readonly ILoanRepository _loanRepository;
        private readonly LoanService _loanService;
        private Book _book;
        private Reader _reader;
        private Loan _loan;
        private int _fine;
        private int _finalPrice;

        public PaymentWindow(ILoanRepository loanRepository, LoanService loanService, Book book, Reader reader)
        {
            _book = book;
            _reader = reader;
            _loanRepository = loanRepository;
            _loanService = loanService;
            InitializeComponent();

            PaymentWindow_Loaded("Выдача книги", _book.Deposit);
        }
        public PaymentWindow(ILoanRepository loanRepository, LoanService loanService, Loan loan, int fine, int finalPrice)
        {
            _loanRepository = loanRepository;
            _loanService = loanService;
            InitializeComponent();
            _isReturn = true;
            _loan = loan;
            _fine = fine;
            _finalPrice = finalPrice;

            PaymentWindow_Loaded("Прием книги", _fine + _finalPrice);
        }

        private void PaymentWindow_Loaded(string text, int amount)
        {
            OperationTypeText.Text = text;
            AmountText.Text = $"{amount} руб.";
            UpdatePaymentUI();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsValid())
            {
                if (_isReturn)
                {
                    _loan.Fine = _fine;
                    _loan.FinalPrice = _finalPrice;

                    if (_loanService.ReturnBook(_loan))
                    {
                        MessageBox.Show("Прокат успешно завершен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка завершения проката", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        DialogResult = false;
                        Close();
                    }
                }
                else
                {
                    if (_loanRepository.Add(_loanService.IssueBook(_book, _reader)))
                    {
                        MessageBox.Show($"Книга выдана", "Успех",
                                      MessageBoxButton.OK, MessageBoxImage.Information);

                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при выдаче книги", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Warning);
                        DialogResult = false;
                        Close();
                    }
                }
            }
        }

        private bool IsValid()
        {
            if (PaymentMethodComboBox?.SelectedIndex == 0)
            {
                if (CardNumberTextBox.Text.Length != 16)
                {
                    MessageBox.Show("Номер карты должен содержать 16 цифр", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (ExpiryDateTextBox.Text.Length != 5)
                {
                    MessageBox.Show("Срок действия должен быть в формате MM/YY", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (!Regex.IsMatch(ExpiryDateTextBox.Text, @"^\d{2}\/\d{2}$"))
                {
                    MessageBox.Show("Срок действия должен быть в формате MM/YY", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (CvvTextBox.Text.Length != 3)
                {
                    MessageBox.Show("CVV код должен содержать 3 цифры", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(CardNumberTextBox.Text, ALL_DIGIT))
                {
                    MessageBox.Show("Номер карты должен содержать только цифры", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(CvvTextBox.Text, ALL_DIGIT))
                {
                    MessageBox.Show("CVV код должен содержать только цифры", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            return true;
        }

        private void PaymentMethodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePaymentUI();
        }

        private void UpdatePaymentUI()
        {
            if (CardPaymentPanel != null && CashMessageText != null && ValidationText != null)
            {
                if (PaymentMethodComboBox.SelectedIndex == 0)
                {
                    CardPaymentPanel.Visibility = Visibility.Visible;
                    CashMessageText.Visibility = Visibility.Collapsed;
                    ValidationText.Text = "Заполните все обязательные поля для оплаты картой";
                }
                else
                {
                    CardPaymentPanel.Visibility = Visibility.Collapsed;
                    CashMessageText.Visibility = Visibility.Visible;
                    ValidationText.Text = "Оплата наличными при получении";
                }
            }
        }
    }
}