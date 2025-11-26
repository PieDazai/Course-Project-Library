using Data.InMemory;
using Data.Interfaces;
using Services;
using System.Windows;

namespace UI
{

    public partial class MainWindow : Window
    {
        private IBookRepository _bookRepository;
        private IReaderRepository _readerRepository;
        private ILoanRepository _loanRepository;
        private LoanService _loanService;
        private StatisticsService _statisticsService;
        public MainWindow(IBookRepository bookRepository, IReaderRepository readerRepository, ILoanRepository loanRepository, 
            LoanService loanService, StatisticsService statisticsService)
        {
            _bookRepository = bookRepository;
            _readerRepository = readerRepository;
            _loanRepository = loanRepository;
            InitializeComponent();
            _loanService = loanService;
            _statisticsService = statisticsService;
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