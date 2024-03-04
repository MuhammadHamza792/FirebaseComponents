using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.ThirdPartyNotifications
{
    [Serializable]
    public class DateTimeData : IComparable<DateTimeData>
    {
        public int Years;
        public int Months;
        public int Days;
        public int Hours;
        public int Minutes;
        public int Seconds;
        public int Milliseconds;

        public int CompareTo(DateTimeData other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var yearsComparison = Years.CompareTo(other.Years);
            if (yearsComparison != 0) return yearsComparison;
            var monthsComparison = Months.CompareTo(other.Months);
            if (monthsComparison != 0) return monthsComparison;
            var daysComparison = Days.CompareTo(other.Days);
            if (daysComparison != 0) return daysComparison;
            var hoursComparison = Hours.CompareTo(other.Hours);
            if (hoursComparison != 0) return hoursComparison;
            var minutesComparison = Minutes.CompareTo(other.Minutes);
            if (minutesComparison != 0) return minutesComparison;
            var secondsComparison = Seconds.CompareTo(other.Seconds);
            if (secondsComparison != 0) return secondsComparison;
            return Milliseconds.CompareTo(other.Milliseconds);
        }
    }
}
