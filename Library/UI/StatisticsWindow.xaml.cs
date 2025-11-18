using Data.Interfaces;
using Domain.Statistics;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using Services;
using System.Windows;

namespace UI
{
    public partial class StatisticsWindow : Window
    {
        private readonly StatisticsService _statisticsService;

        public StatisticsWindow(StatisticsService statisticsService)
        {
            InitializeComponent();
            _statisticsService = statisticsService;

            Loaded += StatisticsWindow_Loaded;
            StartDatePicker.SelectedDate = DateTime.Now.AddYears(-1);
            EndDatePicker.SelectedDate = DateTime.Now;
        }

        private void StatisticsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAgeStatistics();
            LoadRevenueStatistics();
        }

        private void LoadAgeStatistics()
        {
            try
            {
                var ageStats = _statisticsService.GetReadersByAgeGroupBy(new ReaderFilter());
                AgeStatsListView.ItemsSource = ageStats;
                UpdateSummaryInfo(ageStats);
                LoadAgeChart(ageStats);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки статистики по возрастам: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRevenueStatistics()
        {
            try
            {
                var filter = new LoanFilter
                {
                    StartDate = StartDatePicker.SelectedDate.HasValue
                        ? DateOnly.FromDateTime(StartDatePicker.SelectedDate.Value)
                        : null,
                    EndDate = EndDatePicker.SelectedDate.HasValue
                        ? DateOnly.FromDateTime(EndDatePicker.SelectedDate.Value)
                        : null
                };

                var revenueStats = _statisticsService.GetLoansByMonth(filter);
                RevenueDataGrid.ItemsSource = revenueStats;
                LoadRevenueChart(revenueStats);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки статистики по доходам: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void LoadAgeChart(List<ReaderCategoryStatisticItem> data)
        {
            var plotModel = new PlotModel { Title = "Распределение читателей по возрастам" };

            var pieSeries = new PieSeries
            {
                StrokeThickness = 2.0,
                InsideLabelPosition = 0.8,
                AngleSpan = 360,
                StartAngle = 0
            };

            foreach (var item in data)
            {
                var label = $"{item.AgeStart}-{item.AgeEnd} лет";
                pieSeries.Slices.Add(new PieSlice(label, item.Count)
                {
                    IsExploded = false
                });
            }

            plotModel.Series.Add(pieSeries);
            AgePlotView.Model = plotModel; // Напрямую присваиваем модель
        }

        private void LoadRevenueChart(List<MonthlyRevenueStatisticItem> data)
        {
            var plotModel = new PlotModel { Title = "Доходы по месяцам" };

            if (data == null || !data.Any())
            {
                RevenuePlotView.Model = plotModel;
                return;
            }

            // Используем серию прямоугольников для вертикальных столбцов
            var series = new StemSeries
            {
                Title = "Доход",
                Color = OxyColors.SteelBlue,
                MarkerSize = 10,
                MarkerFill = OxyColors.SteelBlue,
                LineStyle = LineStyle.Solid,
                StrokeThickness = 20
            };

            // Находим максимальное значение для правильного позиционирования подписей
            double maxValue = data.Max(item => (double)item.Total);

            // Добавляем точки данных и аннотации
            for (int i = 0; i < data.Count; i++)
            {
                double value = (double)data[i].Total;
                series.Points.Add(new DataPoint(i, value));

                // Добавляем подпись над столбцом
                var annotation = new TextAnnotation
                {
                    Text = value.ToString("C0"), // Формат валюты
                    TextPosition = new DataPoint(i, value + maxValue * 0.03), // Над столбцом (3% от максимума)
                    TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Center, // Указываем полное имя
                    TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom, // Указываем полное имя
                    FontSize = 10,
                    FontWeight = 700, // Используем числовое значение вместо FontWeights.Bold
                    TextColor = OxyColors.DarkBlue,
                    Background = OxyColor.FromArgb(180, 255, 255, 255), // Полупрозрачный белый фон
                    Stroke = OxyColors.LightGray,
                    StrokeThickness = 1
                };
                plotModel.Annotations.Add(annotation);
            }

            plotModel.Series.Add(series);

            // Ось категорий (месяцы) - снизу
            var categoryAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Месяцы"
            };

            // Добавляем подписи месяцев
            foreach (var item in data)
            {
                categoryAxis.Labels.Add(item.GetMonthName());
            }

            // Ось значений - слева
            var valueAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Доход (руб.)",
                Minimum = 0,
                MaximumPadding = 0.15 // Добавляем место для подписей
            };

            plotModel.Axes.Add(categoryAxis);
            plotModel.Axes.Add(valueAxis);

            RevenuePlotView.Model = plotModel;
        }

        private void UpdateSummaryInfo(List<ReaderCategoryStatisticItem> stats)
        {
            if (stats == null || !stats.Any()) return;

            var totalReaders = stats.Sum(s => s.Count);
            var largestCategory = stats.OrderByDescending(s => s.Count).First();
            var smallestCategory = stats.OrderBy(s => s.Count).First();

            TotalReadersText.Text = $"Всего читателей: {totalReaders}";
            LargestCategoryText.Text = $"Самая большая группа: {largestCategory.AgeStart}-{largestCategory.AgeEnd} лет ({largestCategory.Count} чел.)";
            SmallestCategoryText.Text = $"Самая маленькая группа: {smallestCategory.AgeStart}-{smallestCategory.AgeEnd} лет ({smallestCategory.Count} чел.)";
        }

        private void ApplyRevenueFilter_Click(object sender, RoutedEventArgs e)
        {
            LoadRevenueStatistics();
        }

        private void ResetRevenueFilter_Click(object sender, RoutedEventArgs e)
        {
            StartDatePicker.SelectedDate = DateTime.Now.AddYears(-1);
            EndDatePicker.SelectedDate = DateTime.Now;
            LoadRevenueStatistics();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

    }
}