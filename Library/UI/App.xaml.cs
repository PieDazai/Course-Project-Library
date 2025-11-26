using Data.Interfaces;
using Domain;
using LibraryDB.Data.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Services;
using System.IO;
using System.Windows;

namespace UI
{
    public partial class App : Application
    {
        private IReaderRepository _readerRepository = null!;
        private IBookRepository _bookRepository = null!;
        private ILoanRepository _loanRepository = null!;
        private LibraryDbContext _dbContext = null!;
        private LoanService _loanService = null!;
        private StatisticsService _statisticsService = null!;


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Чтение конфигурации из файла
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.database.json")
            .Build();

            // 2. Создание DbContext через фабрику
            var factory = new LibraryDbContextFactory();
            _dbContext = factory.CreateDbContext(configuration);

            // 3. ВАЖНО: Применение миграций автоматически при запуске
            _dbContext.Database.Migrate();

            // 4. Создание репозиториев на основе DbContext
            _readerRepository = new LibraryDB.Data.SqlServer.ReaderRepository(_dbContext);
            _bookRepository = new LibraryDB.Data.SqlServer.BookRepository(_dbContext);
            _loanRepository = new LibraryDB.Data.SqlServer.LoanRepository(_dbContext);
            _loanService = new LoanService(_loanRepository, _bookRepository);
            _statisticsService = new StatisticsService(_loanRepository, _readerRepository);

            // 5. Заполнение тестовыми данными (только если БД пустая)
            SeedInitData();

            // 6. Запуск главного окна
            var mainWindow = new MainWindow(_bookRepository, _readerRepository, _loanRepository, _loanService, _statisticsService);
            mainWindow.Show();
        }

        private void SeedInitData()
        {
            // Проверяем, есть ли уже данные в БД
            if (_bookRepository.GetAll(new BookFilter()).Any() ||
                _readerRepository.GetAll(new ReaderFilter()).Any() ||
                _loanRepository.GetAll(new LoanFilter()).Any())
            {
                return; // Данные уже есть
            }

            // Создание тестовых книг
            var book1 = new Book
            {
                Title = "Мастер и Маргарита",
                Author = "Михаил Булгаков",
                Genre = "Роман",
                RentalCost = 50,
                Deposit = 500,
                PublishedYear = 1967,
                AvailableCopies = 3,
                TotalCopies = 3,
                RackNumber = 1
            };
            _bookRepository.Add(book1);

            var book2 = new Book
            {
                Title = "Преступление и наказание",
                Author = "Фёдор Достоевский",
                Genre = "Роман",
                RentalCost = 45,
                Deposit = 450,
                PublishedYear = 1866,
                AvailableCopies = 2,
                TotalCopies = 2,
                RackNumber = 2
            };
            _bookRepository.Add(book2);

            var book3 = new Book
            {
                Title = "Война и мир",
                Author = "Лев Толстой",
                Genre = "Роман",
                RentalCost = 60,
                Deposit = 600,
                PublishedYear = 1869,
                AvailableCopies = 4,
                TotalCopies = 4,
                RackNumber = 3
            };
            _bookRepository.Add(book3);

            var book4 = new Book
            {
                Title = "1984",
                Author = "Джордж Оруэлл",
                Genre = "Антиутопия",
                RentalCost = 55,
                Deposit = 550,
                PublishedYear = 1949,
                AvailableCopies = 2,
                TotalCopies = 2,
                RackNumber = 4
            };
            _bookRepository.Add(book4);

            var book5 = new Book
            {
                Title = "Гарри Поттер и философский камень",
                Author = "Джоан Роулинг",
                Genre = "Фэнтези",
                RentalCost = 40,
                Deposit = 400,
                PublishedYear = 1997,
                AvailableCopies = 5,
                TotalCopies = 5,
                RackNumber = 5
            };
            _bookRepository.Add(book5);

            // Создание тестовых читателей
            var reader1 = new Reader
            {
                FullName = "Иванов Иван Иванович",
                Address = "ул. Ленина, д. 10, кв. 5",
                PhoneNumber = "+7 (912) 345-67-89",
                Email = "ivanov@mail.ru",
                BirthDate = DateOnly.FromDateTime(new DateTime(1990, 5, 15))
            };
            _readerRepository.Add(reader1);

            var reader2 = new Reader
            {
                FullName = "Петрова Мария Сергеевна",
                Address = "ул. Пушкина, д. 25, кв. 12",
                PhoneNumber = "+7 (923) 456-78-90",
                Email = "petrova@gmail.com",
                BirthDate = DateOnly.FromDateTime(new DateTime(1985, 8, 22))
            };
            _readerRepository.Add(reader2);

            var reader3 = new Reader
            {
                FullName = "Сидоров Алексей Владимирович",
                Address = "пр. Мира, д. 15, кв. 8",
                PhoneNumber = "+7 (934) 567-89-01",
                Email = "sidorov@yandex.ru",
                BirthDate = DateOnly.FromDateTime(new DateTime(1995, 3, 10))
            };
            _readerRepository.Add(reader3);

            // Сохраняем изменения, чтобы получить ID созданных книг и читателей
            _dbContext.SaveChanges();

            // Создание тестовых выдач книг
            var loan1 = new Loan
            {
                Book = book1,
                Reader = reader1,
                IssuanceDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-10)),
                ReturnDate = null,
                Fine = 0,
                Status = "В прокате",
                FinalPrice = 0
            };
            _loanRepository.Add(loan1);
            book1.AvailableCopies--;
            _bookRepository.Update(book1);

            var loan2 = new Loan
            {
                Book = book2,
                Reader = reader2,
                IssuanceDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-5)),
                ReturnDate = null,
                Fine = 0,
                Status = "В прокате",
                FinalPrice = 0
            };
            _loanRepository.Add(loan2);
            book2.AvailableCopies--;
            _bookRepository.Update(book2);

            // Исправленная выдача №3 - завершенный заем с просрочкой
            var loan3 = new Loan
            {
                Book = book3,
                Reader = reader3,
                IssuanceDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-35)),
                ReturnDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-5)), // Возвращена с опозданием
                Fine = 300, // Штраф за просрочку
                Status = "Завершен",
                FinalPrice = 1800 + 300 // Аренда + штраф
            };
            _loanRepository.Add(loan3);

            var loan4 = new Loan
            {
                Book = book4,
                Reader = reader1,
                IssuanceDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
                ReturnDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-10)),
                Fine = 0,
                Status = "Завершен",
                FinalPrice = 1100 // 20 дней аренды
            };
            _loanRepository.Add(loan4);

            var loan5 = new Loan
            {
                Book = book5,
                Reader = reader2,
                IssuanceDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-3)),
                ReturnDate = null,
                Fine = 0,
                Status = "В прокате",
                FinalPrice = 0
            };
            _loanRepository.Add(loan5);
            book5.AvailableCopies--;
            _bookRepository.Update(book5);

            // Дополнительные выдачи
            var loan6 = new Loan
            {
                Book = book1,
                Reader = reader3,
                IssuanceDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-15)),
                ReturnDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-5)),
                Fine = 0,
                Status = "Завершен",
                FinalPrice = 500 // 10 дней аренды
            };
            _loanRepository.Add(loan6);

            var loan7 = new Loan
            {
                Book = book3,
                Reader = reader1,
                IssuanceDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-7)),
                ReturnDate = null,
                Fine = 0,
                Status = "В прокате",
                FinalPrice = 0
            };
            _loanRepository.Add(loan7);
            book3.AvailableCopies--;
            _bookRepository.Update(book3);

            var loan8 = new Loan
            {
                Book = book4,
                Reader = reader2,
                IssuanceDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-20)),
                ReturnDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-15)),
                Fine = 0,
                Status = "Завершен",
                FinalPrice = 275 // 5 дней аренды
            };
            _loanRepository.Add(loan8);

            var loan9 = new Loan
            {
                Book = book2,
                Reader = reader3,
                IssuanceDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-25)),
                ReturnDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-20)),
                Fine = 150, // Штраф за просрочку
                Status = "Завершен",
                FinalPrice = 225 + 150 // Аренда + штраф
            };
            _loanRepository.Add(loan9);

            var loan10 = new Loan
            {
                Book = book5,
                Reader = reader1,
                IssuanceDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
                ReturnDate = null,
                Fine = 0,
                Status = "В прокате",
                FinalPrice = 0
            };
            _loanRepository.Add(loan10);
            book5.AvailableCopies--;
            _bookRepository.Update(book5);

            // Сохраняем все изменения в БД
            _dbContext.SaveChanges();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // ВАЖНО: Освобождаем ресурсы DbContext при закрытии приложения
            _dbContext?.Dispose();
            base.OnExit(e);
        }
    }

}
