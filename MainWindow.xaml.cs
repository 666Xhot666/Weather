using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Windows;
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
            var formattedNotes = new List<string>();
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
        public void EditNote_Click(object sender, RoutedEventArgs e)
        {

        }
        public void AddNote_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
