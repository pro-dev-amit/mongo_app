using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Core.FrameworkCore
{   
    /// <summary>
    /// purpose of this class is to get timings of executions of certain code. Need to have this for recording purposes in the furture.
    /// </summary>
    public class MXTiming
    {
        Stopwatch sw;
        MXTimeUnit _timeUnit;

        public MXTiming()
        {
            _timeUnit = MXTimeUnit.Millisecond;

            sw = new Stopwatch();
            sw.Start();
        }

        public MXTiming(MXTimeUnit timeUnit)
        {
            _timeUnit = timeUnit;

            sw = new Stopwatch();
            sw.Start();
        }

        public string Finish() 
        {
            sw.Stop();

            if (_timeUnit == MXTimeUnit.Millisecond)
                return sw.ElapsedMilliseconds.ToString("0.##") + " ms";
            else if (_timeUnit == MXTimeUnit.Second)
                return sw.Elapsed.TotalSeconds.ToString("0.##") + " s";
            else if (_timeUnit == MXTimeUnit.Minute)
                return sw.Elapsed.TotalMinutes.ToString("0.##") + " mins";
            else if (_timeUnit == MXTimeUnit.Hour)
                return sw.Elapsed.TotalHours.ToString("0.##") + " hrs";
            else if (_timeUnit == MXTimeUnit.Day)
                return sw.Elapsed.TotalDays.ToString("0.##") + " days";
            else
                return "NOT SUPPORTED";
            
        }
    }

    public enum MXTimeUnit
    {
        Millisecond,
        Second,
        Minute,
        Hour,
        Day
    }
}
