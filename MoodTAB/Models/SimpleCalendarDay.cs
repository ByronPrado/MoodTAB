using System;
using System.Collections.Generic;

namespace MoodTAB.Models.Calendar
{
    public class SimpleCalendarDay
    {
        public DateTime DateTime { get; set; }
        public bool IsCurrentMonth { get; set; }
        public bool IsToday { get; set; }
        public IEnumerable<object> Events { get; set; } = Array.Empty<object>();
    }
}
