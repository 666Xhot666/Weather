using System;
using System.Collections.Generic;

namespace Weather.Data
{
    public class MonthStatistic
    {
        public double TemperatureMean { get; set; }
        public double PressureMean { get; set; }
        public List<string>? Dates { get; set; }
        
    }
}
