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

        }

        private void IssueBook_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ReturnBook_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}