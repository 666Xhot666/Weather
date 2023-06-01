using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Windows;
using Weather.Data;

namespace Weather
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppConfiguration.Configuration["CurrentDirectory"] = Environment.CurrentDirectory.ToString(); ;
            base.OnStartup(e);
            try
            {
                // Get the connection string from appsettings.json
                string? connectionString = AppConfiguration.Configuration.GetConnectionString("MySqlConnection");
                // Configure the DBContext with the connection string
                var optionsBuilder = new DbContextOptionsBuilder<NotesDbContext>();
                if (connectionString == null)
                {
                    throw new ArgumentNullException(nameof(connectionString));
                }
                    optionsBuilder.UseMySQL(connectionString);
                    using (var dbContext = new NotesDbContext(optionsBuilder.Options))
                    {
                        // create and fill DB table Notes if that does not exists
                        dbContext.CreateNotesTableIfNotExists();
                    }
            }
            catch (Exception ex)
            {
                // Display error message and offer to load a file
                MessageBoxResult result = MessageBox.Show($"Не вдалося підключитися до бази даних.{Environment.NewLine}{ex.Message}{Environment.NewLine}", "Помилка підключення", MessageBoxButton.OK, MessageBoxImage.Error);
                // Terminate the program
                Current.Shutdown();
            }
            
        }
    }
}
