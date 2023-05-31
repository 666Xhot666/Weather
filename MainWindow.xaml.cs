using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Weather.Data;

namespace Weather
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void NotesListDG_Loaded(object sender, RoutedEventArgs e)
        {
            string? connectionString = AppConfiguration.Configuration.GetConnectionString("MySqlConnection");
            // Configure the DBContext with the connection string
            var optionsBuilder = new DbContextOptionsBuilder<NotesDbContext>();
            if (connectionString != null)
            {
                // Load the formatted notes from the database
                optionsBuilder.UseMySQL(connectionString);
                using (var dbContext = new NotesDbContext(optionsBuilder.Options))
                {
                    if (dbContext.LoadNotes(out var notes))
                    {
                        NotesListDG.ItemsSource = notes;
                    } 
                }
            }
        }
        private void RefreshDataGrid()
        {
            string? connectionString = AppConfiguration.Configuration.GetConnectionString("MySqlConnection");
            // Configure the DBContext with the connection string
            var optionsBuilder = new DbContextOptionsBuilder<NotesDbContext>();
            if (connectionString != null)
            {
                // Load the notes from the database
                optionsBuilder.UseMySQL(connectionString);
                using (var dbContext = new NotesDbContext(optionsBuilder.Options))
                {
                    if (dbContext.LoadNotes(out var notes))
                    {
                        NotesListDG.ItemsSource = notes;
                    }
                }
                NotesListDG.Items.Refresh();
            }
        }
        public void EditNote_Click(object sender, RoutedEventArgs e)
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
        public void AddNote_Click(object sender, RoutedEventArgs e)
        {

        }
        public void SaveSata_Click(object sender, RoutedEventArgs e)
        {
            // Отримуємо значення з полів у GroupBox
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

            // Отримуємо ID вибраного запису
            int noteId = (int)noteGroupBox.Tag;

            string? connectionString = AppConfiguration.Configuration.GetConnectionString("MySqlConnection");
            // Configure the DBContext with the connection string
            var optionsBuilder = new DbContextOptionsBuilder<NotesDbContext>();
            if (connectionString != null)
            {
                optionsBuilder.UseMySQL(connectionString);
                using (var dbContext = new NotesDbContext(optionsBuilder.Options))
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

                        // Очистимо форму
                        dayInputBox.Text = string.Empty;
                        monthInputBox.Text = string.Empty;
                        temperatureInputBox.Text = string.Empty;
                        pressureInputBox.Text = string.Empty;
                        precipitationInputBox.IsChecked = false;

                        MessageBox.Show("Запис успішно оновлено.", "Повідомлення", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Не вдалося знайти вибраний запис.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
