using Data.Interfaces;
using Domain;
using System.Windows;
using System.Windows.Controls;

namespace UI
{
    public partial class BookWindow : Window
    {
        private readonly IBookRepository _bookRepository;
        private BookFilter _filter = new BookFilter();
        public BookWindow(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
            InitializeComponent();
            RefreshDataBookGrid(_filter);
        }

        private void RefreshDataBookGrid(BookFilter filter)
        {
            DataGridBooks.ItemsSource = _bookRepository.GetAll(filter);
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            EditBookWindow editBookWindow = new EditBookWindow(_bookRepository);
            editBookWindow.ShowDialog();
            RefreshDataBookGrid(_filter);
        }

        private void EditBook_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridBooks.SelectedItem is Book book)
            {
                EditBookWindow editBookWindow = new EditBookWindow(_bookRepository, book);
                editBookWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите книгу для редактирования", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
            }
            RefreshDataBookGrid(_filter);
        }

        private void DeleteBook_Click(object sender, RoutedEventArgs e)
        {

            if (DataGridBooks.SelectedItem is Book book)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить книгу:\n" +
                    $"Название: {book.Title}\n" +
                    $"Автор: {book.Author}",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    MessageBoxResult.No);

                if (result == MessageBoxResult.Yes)
                {
                    if (_bookRepository.Delete(book))
                    {
                        MessageBox.Show($"Книга удалена", "Успех",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении книги", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите книгу для удаления", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
            }

            RefreshDataBookGrid(_filter);
        }
  
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            txtGenreSearch.Text = string.Empty;
            txtAuthorSearch.Text = string.Empty;
            txtTitleSearch.Text = string.Empty;
            _filter = new BookFilter();
            RefreshDataBookGrid(_filter);
        }
        private void Books_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isEnabledBook = DataGridBooks.SelectedItem is Book;
            btnEditBook.IsEnabled = isEnabledBook;
            btnDeleteBook.IsEnabled = isEnabledBook;
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            _filter = new BookFilter
            {
                Author = txtAuthorSearch.Text,
                Title = txtTitleSearch.Text,
                Genre = txtGenreSearch.Text,
            };

            RefreshDataBookGrid( _filter );
        }


    }
}
