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
        private StatisticsService _statisticsService = null!;
        private LibraryDbContext _dbContext = null!;


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

            // 5. Заполнение тестовыми данными (только если БД пустая)
            SeedInitData();

            // 6. Запуск главного окна
            var mainWindow = new MainWindow(_bookRepository, _readerRepository, _loanRepository);
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
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // ВАЖНО: Освобождаем ресурсы DbContext при закрытии приложения
            _dbContext?.Dispose();
            base.OnExit(e);
        }
    }

}
