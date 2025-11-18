using Data.Interfaces;
using Domain;
using System.Windows;

namespace UI
{
    public partial class EditBookWindow : Window
    {
        private readonly IBookRepository _bookRepository;

        private bool _isEditBook = false;

        private Book _editBook;


        public EditBookWindow(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
            _editBook = new Book();
            InitializeComponent();
            SetWindowTitle();
        }

        public EditBookWindow(IBookRepository bookRepository, Book book)
        {
            _isEditBook = true;
            _bookRepository = bookRepository;
            _editBook = book;
            InitializeComponent();
            LoadDataBook(book);
            SetWindowTitle();
        }

        private void SetWindowTitle()
        {
            if (_isEditBook)
            {
                Title = "Редактирование книги";
                WindowTitleText.Text = "Редактирование книги";
            }
            else
            {
                Title = "Добавление новой книги";
                WindowTitleText.Text = "Добавление новой книги";
            }
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void LoadDataBook(Book book)
        {
            TitleTextBox.Text = book.Title;
            AuthorTextBox.Text = book.Author;
            GenreTextBox.Text = book.Genre;
            YearTextBox.Text = book.PublishedYear.ToString();
            RentalCostTextBox.Text = book.RentalCost.ToString();
            DepositTextBox.Text = book.Deposit.ToString();
            TotalCopiesTextBox.Text = book.TotalCopies.ToString();
            AvailableCopiesTextBox.Text = book.AvailableCopies.ToString();
            RackNumberTextBox.Text = book.RackNumber.ToString();
        }

        private bool Validation()
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Введите название книги!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (string.IsNullOrWhiteSpace(AuthorTextBox.Text))
            {
                MessageBox.Show("Введите автора книги!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (string.IsNullOrWhiteSpace(GenreTextBox.Text))
            {
                MessageBox.Show("Введите жанр книги!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (!decimal.TryParse(RentalCostTextBox.Text, out decimal rentalCost) || rentalCost <= 0)
            {
                MessageBox.Show("Введите корректную стоимость проката (положительное число)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (!decimal.TryParse(DepositTextBox.Text, out decimal deposit) || deposit <= 0)
            {
                MessageBox.Show("Введите корректную залоговую стоимость (положительное число)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (!int.TryParse(YearTextBox.Text, out int publishedYear) || publishedYear < 1000 || publishedYear > DateTime.Now.Year)
            {
                MessageBox.Show("Введите корректный год издания (от 1000 до текущего года)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
             
            if (!int.TryParse(TotalCopiesTextBox.Text, out int totalCopies) || totalCopies <= 0)
            {
                MessageBox.Show("Введите корректное общее количество экземпляров (положительное число)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (!int.TryParse(AvailableCopiesTextBox.Text, out int availableCopiesTextBox) || availableCopiesTextBox < 0)
            {
                MessageBox.Show("Введите корректное количество доступных экземпляров!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (!int.TryParse(RackNumberTextBox.Text, out int rackNumber) || rackNumber <= 0)
            {
                MessageBox.Show("Введите корректный номер стеллажа (положительное число)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if(int.Parse(TotalCopiesTextBox.Text) < int.Parse(AvailableCopiesTextBox.Text))
            {
                MessageBox.Show("Число доступных экземпляров не может быть больше общего количества!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            return true;
        }

        private void ApplyChanges()
        {
            _editBook.Title = TitleTextBox.Text.Trim();
            _editBook.Author = AuthorTextBox.Text.Trim();
            _editBook.Genre = GenreTextBox.Text.Trim();
            _editBook.PublishedYear = int.Parse(YearTextBox.Text);
            _editBook.Deposit = int.Parse(DepositTextBox.Text);
            _editBook.RentalCost = int.Parse(RentalCostTextBox.Text);
            _editBook.AvailableCopies = int.Parse(AvailableCopiesTextBox.Text);
            _editBook.TotalCopies = int.Parse(TotalCopiesTextBox.Text);
            _editBook.RackNumber = int.Parse(RackNumberTextBox.Text);
        }

        private void CaveButtonClick(object sender, RoutedEventArgs e)
        {
            if (Validation())
            {
                ApplyChanges();

                if (!_isEditBook)
                {
                    if (_bookRepository.Add(_editBook))
                    {
                        MessageBox.Show("Книга добавлена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка добавления книги", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        DialogResult = false;
                        Close();
                    }
                }
                else
                {
                    if (_bookRepository.Update(_editBook))
                    {
                        MessageBox.Show("Книга обновлена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка обновления книги", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        DialogResult = false;
                        Close();
                    }
                }
            }
        }
    }
}
