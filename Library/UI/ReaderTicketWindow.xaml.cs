using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UI
{
    /// <summary>
    /// Логика взаимодействия для ReaderTicketWindow.xaml
    /// </summary>
    public partial class ReaderTicketWindow : Window
    {
        public ReaderTicketWindow()
        {
            InitializeComponent();
        }

        private void LoansDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void IssueBookButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ReturnBookButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
