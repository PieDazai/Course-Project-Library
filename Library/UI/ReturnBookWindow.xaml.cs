using Data.Interfaces;
using Domain;
using Services;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace UI
{
    public partial class ReturnBookWindow : Window
    {
        private readonly ILoanRepository _loanRepository;
        private readonly LoanService _loanService;
        private Loan _loan;

        private int _totalDays;
        private int _daysOverdue;
        private int _finalPrice;
        private int _fine;

        public ReturnBookWindow(Loan loan, ILoanRepository loanRepository, LoanService loanService)
        {
            _loanRepository = loanRepository;
            _loanService = loanService;
            _loan = loan;
            InitializeComponent();
            CalculateDataLoad();
            LoadDataForReturn();
            UpdatePaymentButton(true);
        }

        private void UpdatePaymentButton(bool flag)
        {
            if(ReturnBookButton != null) ReturnBookButton.IsEnabled = flag;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ReturnBookButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                $"Принять книгу:\n" +
                $"Название: {_loan.Book.Title}\n" +
                $"Автор: {_loan.Book.Author}",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                PaymentWindow paymentWindow = new PaymentWindow(_loanRepository, _loanService, _loan, _fine, _finalPrice);

                if (paymentWindow.ShowDialog() == true)
                {
                    DialogResult = true;
                    Close();
                }
            }
        }


        private void DamageCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            DamageAmountTextBox.IsEnabled = true;
        }

        private void DamageUnCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            DamageAmountTextBox.IsEnabled = false;
            DamageAmountTextBox.Text = "0";
            CalculateDataLoad();
            LoadDataForReturn();
        }

        private void DamageAmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(CorrectFineValues(DamageAmountTextBox.Text) == false) 
            {
                MessageBox.Show("Введите корректный штраф!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                UpdatePaymentButton(false);
                return;
            }
            CalculateDataLoad();
            LoadDataForReturn();
            UpdatePaymentButton(true);
        }

        private void LostAmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CorrectFineValues(LostAmountTextBox.Text) == false)
            {
                MessageBox.Show("Введите корректный штраф!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                UpdatePaymentButton(false);
                return;
            }
            CalculateDataLoad();
            LoadDataForReturn();
            UpdatePaymentButton(true); ;
        }
        private void LostCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            LostAmountTextBox.IsEnabled = LostCheckBox.IsChecked == true;
            if (LostCheckBox.IsChecked != true)
            {
                LostAmountTextBox.Text = "0";
            }
            CalculateDataLoad();
            LoadDataForReturn();
        }

        private void LoadDataForReturn()
        {
            try
            {
                ReaderPhoneText.Text = _loan.Reader.PhoneNumber;
                ReaderNameText.Text = _loan.Reader.FullName;

                BookTitleText.Text = _loan.Book.Title;
                IssueDateText.Text = _loan.IssuanceDate.ToString("dd.MM.yyyy");
                BookAuthorText.Text = _loan.Book.Author;

                RentalCostText.Text = _loan.Book.RentalCost.ToString() + " руб/день";

                ActualRentalPeriodText.Text = _totalDays.ToString() + " дней";

                OverdueText.Text = (_daysOverdue > 0) ? _daysOverdue.ToString() + " дней" : "0 дней";

                int overdueFine = (_daysOverdue > 0) ? _loan.Book.RentalCost * _daysOverdue : 0;
                OverdueFineText.Text = overdueFine.ToString() + " руб";

                TotalCostText.Text = _finalPrice.ToString() + " руб";

                if (AdditionalFinesText != null)
                {
                    AdditionalFinesText.Text = "Доп. штрафы: " + _fine.ToString() + " руб";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CalculateDataLoad()
        {
            try
            {
                _totalDays = (DateOnly.FromDateTime(DateTime.Now).DayNumber - _loan.IssuanceDate.DayNumber);
                _totalDays = Math.Max(1, _totalDays); 

                _daysOverdue = Math.Max(0, _totalDays - 14);

                _fine = 0;

                if (DamageAmountTextBox != null)
                {
                    if (int.TryParse(DamageAmountTextBox.Text, out int damage))
                    {
                        _fine += Math.Max(0, damage);
                    }
                }

                if (LostAmountTextBox != null)
                {
                    if (int.TryParse(LostAmountTextBox.Text, out int lost))
                    {
                        _fine += Math.Max(0, lost);
                    }
                }

                int overdueFine = _loan.Book.RentalCost * _daysOverdue;

                _finalPrice = _loan.Book.RentalCost * _totalDays + overdueFine + _fine;

                if (_finalPrice <= 0)
                    _finalPrice = _loan.Book.RentalCost;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка расчета стоимости: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CorrectFineValues(string text)
        {
            Regex regex = new Regex("^[0-9]+$");
            return regex.IsMatch(text) || string.IsNullOrEmpty(text);
        }
    }
}