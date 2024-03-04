using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.ThirdPartyNotifications
{
    [Serializable]
    public class UTimeSpan : IComparable<UTimeSpan>
    {
        public UTimeSpan() { }

        public UTimeSpan(long ticks)
        {
            _ticks = ticks;
        }

        [SerializeField]
        public long _ticks;

        private TimeSpan timeSpan;

        public static implicit operator TimeSpan(UTimeSpan uts)
            => uts.timeSpan;

        public static implicit operator UTimeSpan(TimeSpan ts)
            => new UTimeSpan(ts.Ticks);

        public int CompareTo(UTimeSpan other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var ticksComparison = _ticks.CompareTo(other._ticks);
            if (ticksComparison != 0) return ticksComparison;
            return timeSpan.CompareTo(other.timeSpan);
        }
    }

    [Serializable]
    public class UDateTime : IComparable<UDateTime>
    {
        [SerializeField] 
        private long _ticks;

        private bool initialized;
        private DateTime m_dateTime;
        public DateTime DateTime
        {
            get
            {
                if (!initialized)
                {
                    m_dateTime = new DateTime(_ticks);
                    initialized = true;
                }

                return m_dateTime;
            }
        }

        public UDateTime(DateTime dateTime)
        {
            _ticks = dateTime.Ticks;
            m_dateTime = dateTime;
            initialized = true;
        }

        public int CompareTo(UDateTime other)
        {
            if (other == null)
            {
                return 1;
            }
            return _ticks.CompareTo(other._ticks);
        }
    }
}