using System.Windows;

namespace UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Catalog_Click(object sender, RoutedEventArgs e)
        {
            BookWindow bookWindow = new BookWindow();
            bookWindow.Show();
        }

        private void User_Click(object sender, RoutedEventArgs e)
        {
            ReaderWindow readerWindow = new ReaderWindow();
            readerWindow.Show();

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();

        }
    }
}