using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Weather.Data;

namespace Weather
{
    internal class StatisticsCollector
    {
        private NotesDbContext dbContext;
        public StatisticsCollector(string connectionString)
        {
            dbContext = new NotesDbContext(new DbContextOptionsBuilder<NotesDbContext>().UseMySQL(connectionString).Options);
        }
        public double CalculateAverageMonthlyTemperature(int month_id)
        {
            if (dbContext.LoadNotes(out List<Notes> notes))
            {
                var temperatures = notes.Where(n => n.month_id == month_id).Select(n => n.air_temperature);
                if (temperatures.Any())
                {
                    return Math.Round(temperatures.Average(),4);
                }
            }
            return 0;
        }

        public double CalculateAverageMonthlyPressure(int month_id)
        {
            if (dbContext.LoadNotes(out List<Notes> notes))
            {
                var pressures = notes.Where(n => n.month_id == month_id).Select(n => n.pressure);
                if (pressures.Any())
                {
                    return Math.Round(pressures.Average(),4);
                }
            }
            return 0;
        }
        public bool GetDatesWithNegativeTemperatureAndPrecipitation(out List<string> dates)
        {
            dates = new List<string>();
            try
            {
                if (dbContext.LoadNotes(out List<Notes> notes))
                {
                    dates = notes
                        .Where(n => n.air_temperature < 0 && n.presence_of_precipitation)
                        .Select(n => $"{n.day.ToString("00")}/{n.month_id.ToString("00")}")
                        .ToList();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
