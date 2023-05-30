using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
            base.OnStartup(e);
            // Get the connection string from appsettings.json
            string? connectionString = AppConfiguration.Configuration.GetConnectionString("MySqlConnection");
            // Configure the DBContext with the connection string
            var optionsBuilder = new DbContextOptionsBuilder<NotesDbContext>();
            if (connectionString != null)
            {
                optionsBuilder.UseMySQL(connectionString);
                using (var dbContext = new NotesDbContext(optionsBuilder.Options))
                {
                    // create and fill DB table Notes if that does not exists
                    dbContext.CreateNotesTableIfNotExists();
                }
            }
        }
    }
}
