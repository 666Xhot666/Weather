using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Weather.Data
{
    public class NotesDbContext: DbContext
    {
        public NotesDbContext(DbContextOptions<NotesDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity mappings and relationships
            modelBuilder.Entity<Notes>(entity =>
            {
                entity.ToTable("Notes");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.day).HasColumnName("day").IsRequired();
                entity.Property(e => e.month_id).HasColumnName("month_id").IsRequired();
                entity.Property(e => e.air_temperature).HasColumnName("air_temperature");
                entity.Property(e => e.presence_of_precipitation).HasColumnName("presence_of_precipitation");
                entity.Property(e => e.pressure).HasColumnName("pressure");
                entity.HasKey(e => e.Id).HasName("PK_Notes");
            });
        }
        private void SeedData()
        {
            // Check if the table exists
            bool tableExists = IsTableExists("Notes");

            if (tableExists)
            {
                // Generate 10 sample notes
                var notes = new List<Notes>();
                var random = new Random();

                for (int i = 1; i <= 10; i++)
                {
                    var note = new Notes
                    {
                        Id = i,
                        day = random.Next(1, 31),
                        month_id = random.Next(1, 12),
                        air_temperature = Math.Round(random.NextDouble() * 100, 2),
                        presence_of_precipitation = random.Next(0, 2) == 0 ? false : true,
                        pressure = Math.Round(random.NextDouble() * 100, 2)
                    };

                    notes.Add(note);
                }

                // Add the notes to the database
                Notes.AddRange(notes);
                SaveChanges();
            }
        }
        public bool IsConnectionAlive()
        {
            try
            {
                return Database.CanConnect();
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool IsTableExists(string tableName)
        {
            var connection = Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            try
            {
                var schema = connection.GetSchema("Tables");
                return schema.Rows.OfType<DataRow>().Any(row => row["TABLE_NAME"].ToString() == tableName);
            }
            finally
            {
                connection.Close();
            }
        }
        public void CreateNotesTableIfNotExists()
        {
            bool tableExists = IsTableExists("Notes");

            if (!tableExists)
            {
                // Create the table
                Database.ExecuteSqlRaw(@"
                    CREATE TABLE Notes (
                        id INT unsigned NOT NULL AUTO_INCREMENT,
                        day INT(2) NOT NULL,
                        month_id INT(2) NOT NULL,
                        air_temperature DOUBLE(4,2),
                        presence_of_precipitation BOOL,
                        pressure DOUBLE(4,2),
                        PRIMARY KEY (id)
                    );");
                SeedData();
            }
        }
        public bool UpdateNote(Notes note)
        {
            try
            {
                Notes? existingNote = Notes.FirstOrDefault(n => n.Id == note.Id);

                if (existingNote == null)
                {
                    throw new Exception("Note does not Exists.");
                }
                // Update the existing record with new values
                existingNote.day = note.day;
                existingNote.month_id = note.month_id;
                existingNote.air_temperature = note.air_temperature;
                existingNote.presence_of_precipitation = note.presence_of_precipitation;
                existingNote.pressure = note.pressure;

                SaveChanges(); // Save the changes to the database
                return true; // Update successful
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log the error, display an error message, etc.)
                Console.WriteLine("An error occurred while updating the note: " + ex.Message);
                return false; // Update failed
            }
        }
        public bool AddNote(Notes note)
        {
            try
            {
                Notes.Add(note);
                SaveChanges(); // Save the changes to the database
                return true; // Add operation successful
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log the error, display an error message, etc.)
                Console.WriteLine("An error occurred while adding the note: " + ex.Message);
                return false; // Add operation failed
            }
        }
        public bool LoadNotes(out List<Notes> notes)
        {
            notes = new List<Notes>();
            try
            {
                notes = Notes.ToList();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured while loading notes:" + ex.Message);
                return false;
            }
        }

        public DbSet<Notes> Notes { get; set; }
    }
}
