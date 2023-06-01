using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using Weather.Data;

namespace Weather
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MonthStatistic? statistic;
        private readonly string? connectionString;
        private readonly DbContextOptions<NotesDbContext> options;

        public MainWindow()
        {
            try
            {
                this.statistic = new MonthStatistic();
                this.connectionString = AppConfiguration.Configuration.GetConnectionString("MySqlConnection");
                if (this.connectionString == null)
                {
                    throw new System.Exception("Empty connection String");
                }
                this.options = new DbContextOptionsBuilder<NotesDbContext>().UseMySQL(connectionString).Options;
                InitializeComponent();
            }
            catch (Exception ex)
            {
                // Display error message and offer to load a file
                MessageBox.Show($"Не вдалося підключитися до бази даних.{Environment.NewLine}{ex.Message}{Environment.NewLine}", "Помилка підключення", MessageBoxButton.OK, MessageBoxImage.Error);
                // Terminate the program
                Application.Current.Shutdown();
            }
        }
        private void NotesListDG_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var dbContext = new NotesDbContext(options))
                {
                    if (dbContext.LoadNotes(out var notes))
                    {
                        NotesListDG.ItemsSource = notes;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка програми.{Environment.NewLine}{ex.Message}{Environment.NewLine}", "Помилка програми", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void RefreshDataGrid()
        {
            try
            {
                using (var dbContext = new NotesDbContext(options))
                {
                    if (dbContext.LoadNotes(out var notes))
                    {
                        NotesListDG.ItemsSource = notes;
                    }
                }
                NotesListDG.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка програми.{Environment.NewLine}{ex.Message}{Environment.NewLine}", "Помилка програми", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void EditNote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (NotesListDG.SelectedItem == null)
                {
                    MessageBox.Show("Будь ласка, оберіть запис.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                // Отримуємо вибраний запис з DataGrid
                var selectedNote = NotesListDG.SelectedItem as Notes;

                // Перевіряємо, чи вибрано запис
                if (selectedNote != null)
                {

                    noteGroupBox.Tag = selectedNote.Id;
                    // Встановлюємо значення полів у GroupBox з вибраного запису
                    dayInputBox.Text = selectedNote.day.ToString();
                    dayInputBox.IsReadOnly = true;
                    monthInputBox.Text = selectedNote.month_id.ToString();
                    monthInputBox.IsReadOnly = true;
                    temperatureInputBox.Text = selectedNote.air_temperature.ToString();
                    pressureInputBox.Text = selectedNote.pressure.ToString();
                    // Для CheckBox залежно від значення selectedNote.presence_of_precipitation встановлюємо його стан
                    precipitationInputBox.IsChecked = selectedNote.presence_of_precipitation;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка програми.{Environment.NewLine}{ex.Message}{Environment.NewLine}", "Помилка програми", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void AddNote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Очистити поля форми GroupBox
                noteGroupBox.Tag = string.Empty;
                dayInputBox.Text = string.Empty;
                dayInputBox.IsReadOnly = false;
                monthInputBox.Text = string.Empty;
                monthInputBox.IsReadOnly = false;
                temperatureInputBox.Text = string.Empty;
                pressureInputBox.Text = string.Empty;
                precipitationInputBox.IsChecked = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка програми.{Environment.NewLine}{ex.Message}{Environment.NewLine}", "Помилка програми", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void SaveData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Отримуємо значення з полів у GroupBox
                int noteId;
                int day;
                int month;
                double temperature;
                double pressure;
                bool precipitation;
                if (!int.TryParse(dayInputBox.Text, out day) || day < 1 || day > 31)
                {
                    MessageBox.Show("Введіть коректний день.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(monthInputBox.Text, out month) || month < 1 || month > 12)
                {
                    MessageBox.Show("Введіть коректний місяць.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!double.TryParse(temperatureInputBox.Text, out temperature))
                {
                    MessageBox.Show("Введіть коректну температуру.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!double.TryParse(pressureInputBox.Text, out pressure))
                {
                    MessageBox.Show("Введіть коректний тиск.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                precipitation = precipitationInputBox.IsChecked ?? false;

                // Configure the DBContext with the connection string
                using (var dbContext = new NotesDbContext(options))
                {

                    //int noteId = (int)noteGroupBox.Tag;
                    if (int.TryParse(noteGroupBox.Tag.ToString(), out noteId))
                    {
                        // Знайдемо запис за його ID
                        var note = dbContext.Notes.FirstOrDefault(n => n.Id == noteId);
                        if (note != null)
                        {
                            // Оновимо значення полів запису
                            note.day = day;
                            note.month_id = month;
                            note.air_temperature = temperature;
                            note.pressure = pressure;
                            note.presence_of_precipitation = precipitation;

                            // Збережемо зміни в базі даних
                            dbContext.SaveChanges();

                            // Оновимо відображення DataGrid
                            RefreshDataGrid();
                            noteGroupBox.Tag = string.Empty;
                            dayInputBox.Text = string.Empty;
                            dayInputBox.IsReadOnly = false;
                            monthInputBox.Text = string.Empty;
                            monthInputBox.IsReadOnly = false;
                            temperatureInputBox.Text = string.Empty;
                            pressureInputBox.Text = string.Empty;
                            precipitationInputBox.IsChecked = false;

                            MessageBox.Show("Запис успішно оновлено.", "Повідомлення", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Запис не знайдено:", "Помилка оновлення запису", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        if (dbContext.Notes.FirstOrDefault(n => n.day == day && n.month_id == month) == null)
                        {
                            Notes note = new Notes
                            {
                                day = day,
                                month_id = month,
                                air_temperature = temperature,
                                pressure = pressure,
                                presence_of_precipitation = precipitation
                            };
                            if (dbContext.AddNote(note))
                            {
                                dbContext.SaveChanges();
                                RefreshDataGrid();
                                noteGroupBox.Tag = string.Empty;
                                dayInputBox.Text = string.Empty;
                                monthInputBox.Text = string.Empty;
                                temperatureInputBox.Text = string.Empty;
                                pressureInputBox.Text = string.Empty;
                                precipitationInputBox.IsChecked = false;
                                MessageBox.Show("Запис успішно Додано.", "Повідомлення", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Запис не можливо додати:", "Помилка додавання запису", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Запис вже існує:", "Помилка додавання запису", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка програми.{Environment.NewLine}{ex.Message}{Environment.NewLine}", "Помилка програми", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void ReportCalculation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(monthReportInputBox.Text, out int month_id) | month_id>12 | month_id<1)
                {
                    MessageBox.Show("Введіть коректний місяць.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                StatisticsCollector statisticsCollector = new StatisticsCollector(connectionString);
                this.statistic.TemperatureMean = statisticsCollector.CalculateAverageMonthlyTemperature(month_id);
                this.statistic.PressureMean = statisticsCollector.CalculateAverageMonthlyPressure(month_id);
                statisticsCollector.GetDatesWithNegativeTemperatureAndPrecipitation(out List<string> dates);
                this.statistic.Dates = dates;
                MessageBox.Show("Статистика успішно зібрана.","Повідомлення",MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка програми.{Environment.NewLine}{ex.Message}{Environment.NewLine}", "Помилка програми", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void ReportSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка програми.{Environment.NewLine}{ex.Message}{Environment.NewLine}", "Помилка програми", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
