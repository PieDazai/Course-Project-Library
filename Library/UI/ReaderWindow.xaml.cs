using Data.Interfaces;
using System.Windows;
using Domain;
using System.Text.RegularExpressions;
using Services;

namespace UI
{
    public partial class ReaderWindow : Window
    {
        private readonly IReaderRepository _readerRepository;
        private readonly IBookRepository _bookRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly LoanService _loanService;

        private ReaderFilter _filter = new ReaderFilter();
        public ReaderWindow(IReaderRepository readerRepository, IBookRepository bookRepository, ILoanRepository loanRepository, LoanService loanServise)
        {           
            _bookRepository = bookRepository;
            _readerRepository = readerRepository;
            _loanRepository = loanRepository;
            _loanService = loanServise;
            InitializeComponent();
            RefreshDataGrid(_filter);

        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _filter = new ReaderFilter
                {
                    FullName = string.IsNullOrWhiteSpace(SearchFullName.Text) ?
                              null : SearchFullName.Text.Trim(),

                    PhoneNumber = string.IsNullOrWhiteSpace(SearchPhoneNumber.Text) ?
                                null : Regex.Replace(SearchPhoneNumber.Text, @"\D", ""),

                    TicketNumber = string.IsNullOrWhiteSpace(SearchTicketNumber.Text) ?
                                 null : int.TryParse(SearchTicketNumber.Text.Trim(), out int ticketNum) ? ticketNum : null
                };

                RefreshDataGrid(_filter);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            SearchFullName.Text = string.Empty;
            SearchPhoneNumber.Text = string.Empty;
            SearchTicketNumber.Text = string.Empty;
            _filter = new ReaderFilter();
            RefreshDataGrid(_filter);
        }

        private void AddReaderButton_Click(object sender, RoutedEventArgs e)
        {
            EditReaderWindow editReaderWindow = new EditReaderWindow(_readerRepository);
            editReaderWindow.ShowDialog();
            RefreshDataGrid(_filter);

        }

        private void EditReaderButton_Click(object sender, RoutedEventArgs e)
        {
            if(ReadersDataGrid.SelectedItem is Reader reader)
            {
                EditReaderWindow editReaderWindow = new EditReaderWindow(_readerRepository, reader);
                editReaderWindow.ShowDialog();
                RefreshDataGrid(_filter);
            }
        }

        private void DeleteReaderButton_Click(object sender, RoutedEventArgs e)
        {

            if (ReadersDataGrid.SelectedItem is Reader reader)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить читателя:\n" +
                    $"ФИО: {reader.FullName}\n" +
                    $"Номер телефона: {reader.PhoneNumber}",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    MessageBoxResult.No);

                if (result == MessageBoxResult.Yes)
                {
                    if (_readerRepository.Delete(reader))
                    {
                        MessageBox.Show($"Читатель удален", "Успех",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении читателя", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите читателя для удаления", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
            }

            RefreshDataGrid(_filter);

        }

        private void ReaderTicketButton_Click(object sender, RoutedEventArgs e)
        {
            if(ReadersDataGrid.SelectedItem is Reader reader)
            {
                ReaderTicketWindow readerTicketWindow = new ReaderTicketWindow(reader, _bookRepository, _loanRepository, _loanService);
                readerTicketWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите читателя для открытия билета", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }

            RefreshDataGrid(_filter);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void RefreshDataGrid(ReaderFilter filter)
        {
            ReadersDataGrid.ItemsSource = _readerRepository.GetAll(filter);

        }

        private void Reader_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            bool isReader = ReadersDataGrid.SelectedItem is Reader reader;
            DeleteReaderButton.IsEnabled = isReader;
            ReaderTicketButton.IsEnabled = isReader;
            EditReaderButton.IsEnabled = isReader;
        }
    }
}
