using Data.Interfaces;
using Domain;
using Services;
using System;
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
                }
            }
        }

        private void LostAmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateDataLoad();
            LoadDataForReturn();
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
            CalculateDataLoad();
            LoadDataForReturn();
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

                int overdueFine = (_daysOverdue > 0) ? (int)(_loan.Book.RentalCost * _daysOverdue * 0.5) : 0;
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

                int overdueFine = (int)(_loan.Book.RentalCost * _daysOverdue);

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
    }
}