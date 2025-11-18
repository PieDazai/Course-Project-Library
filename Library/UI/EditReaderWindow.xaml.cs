using Data.Interfaces;
using System.Windows;
using Domain;
using Data.InMemory;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;


namespace UI
{    public partial class EditReaderWindow : Window
    {
        private static readonly string EMAIL_VALID = @"^[\w\.\-]+@(?!\.)(?!.*\.\.)[\w\.\-]+\.[a-zA-Z]{2,}$";

        private readonly IReaderRepository _readerRepository;
        private Reader _readerEdit;
        private bool _isEditReader = false;
        public EditReaderWindow(IReaderRepository readerRepository)
        {
            _readerEdit = new Reader();
            _readerRepository = readerRepository;
            InitializeComponent();
            SetWindowTitle();
        }

        public EditReaderWindow(IReaderRepository readerRepository, Reader reader)
        {
            _readerRepository = readerRepository;
            _readerEdit = reader;
            _isEditReader = true;
            InitializeComponent();
            LoadReaderData();
            SetWindowTitle();
        }

        private void SetWindowTitle()
        {
            if (_isEditReader)
            {
                Title = "Редактирование данных читателя";
                WindowTitleText.Text = "Редактирование данных читателя";
            }
            else
            {
                Title = "Добавление нового читателя";
                WindowTitleText.Text = "Добавление нового читателя";
            }
        }

        private void LoadReaderData()
        {
            FullNameTextBox.Text = _readerEdit.FullName;
            AddressTextBox.Text = _readerEdit.Address;
            PhoneTextBox.Text = _readerEdit.PhoneNumber;
            EmailTextBox.Text = _readerEdit.Email;
            BirthDateTextBox.Text = _readerEdit.BirthDate.ToString("dd.MM.yyyy");
        }

        private void SaveReader_Click(object sender, RoutedEventArgs e)
        {
            if (Validate())
            {
                ApplyChanges();

                if (!_isEditReader)
                {
                    if (_readerRepository.Add(_readerEdit))
                    {
                        MessageBox.Show("Читатель добавлен\n" +
                            $"Номер читательского билета: {_readerEdit.Id}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка добавления читателя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        DialogResult = false;
                        Close();
                    }
                }
                else
                {
                    if (_readerRepository.Update(_readerEdit))
                    {
                        MessageBox.Show("Данные читателя обновлены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        _isEditReader = false;
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка обновления данных читателя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        DialogResult = false;
                        Close();
                    }
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите закрыть окно? " +
                    "Все несохраненные изменения будут потеряны.",
                                           "Подтверждение закрытия",
                                           MessageBoxButton.YesNo,
                                           MessageBoxImage.Question,
                                           MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                DialogResult = false;
                Close();
            }
        }

        private bool Validate()
        {

            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
            {
                MessageBox.Show("Введите ФИО читателя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (string.IsNullOrWhiteSpace(AddressTextBox.Text))
            {
                MessageBox.Show("Введите адрес читателя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (string.IsNullOrWhiteSpace(PhoneTextBox.Text) || Regex.Replace(PhoneTextBox.Text, "\\D", "").Length != 11)
            {
                MessageBox.Show("Введите корректный номер телефона читателя(11 цифр)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (string.IsNullOrWhiteSpace(EmailTextBox.Text) || !Regex.IsMatch(EmailTextBox.Text, EMAIL_VALID))
            {
                MessageBox.Show("Введите корректный email читателя!\n" +
                    "example@email.ru", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (!DateOnly.TryParse(BirthDateTextBox.Text, out DateOnly year))
            {
                MessageBox.Show("Введите корректную дату рождения читателя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (year > DateOnly.FromDateTime(DateTime.Now))
            {
                MessageBox.Show("Дата рождения не может быть в будущем!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (year < DateOnly.FromDateTime(DateTime.Now.AddYears(-100)))
            {
                MessageBox.Show("Дата рождения не может быть более 100 лет назад!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            return true;
        }

        private void ApplyChanges()
        {
            _readerEdit.FullName = FullNameTextBox.Text.Trim();
            _readerEdit.Address = AddressTextBox.Text.Trim();
            _readerEdit.Email = EmailTextBox.Text.Trim();
            _readerEdit.BirthDate = DateOnly.Parse(BirthDateTextBox.Text);

            if (!PhoneTextBox.Text.Equals(_readerEdit.PhoneNumber))
            {
                _readerEdit.PhoneNumber = _readerRepository.FormatNumber(PhoneTextBox.Text);
            }
        }
    }
}
