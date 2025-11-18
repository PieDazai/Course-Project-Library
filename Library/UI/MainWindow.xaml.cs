using Data.InMemory;
using Data.Interfaces;
using Services;
using System.Windows;

namespace UI
{
    public partial class MainWindow : Window
    {
        private IBookRepository _bookRepository = new BookRepository();
        private IReaderRepository _readerRepository = new ReaderRepository();
        private ILoanRepository _loanRepository = new LoanRepository();
        private LoanService _loanService;
        private StatisticsService _statisticsService;
        public MainWindow()
        {
            InitializeComponent();
            _loanService = new LoanService(_loanRepository, _bookRepository);
            _statisticsService = new StatisticsService(_loanRepository, _readerRepository);
        }

        private void Catalog_Click(object sender, RoutedEventArgs e)
        {
            BookWindow bookWindow = new BookWindow(_bookRepository);
            bookWindow.ShowDialog();
        }

        private void User_Click(object sender, RoutedEventArgs e)
        {
            ReaderWindow readerWindow = new ReaderWindow(_readerRepository, _bookRepository, _loanRepository, _loanService);
            readerWindow.ShowDialog();

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Statistic_Click(object sender, RoutedEventArgs e)
        {
            StatisticsWindow statisticsWindow = new StatisticsWindow(_statisticsService);
            statisticsWindow.ShowDialog();
        }
    }
}