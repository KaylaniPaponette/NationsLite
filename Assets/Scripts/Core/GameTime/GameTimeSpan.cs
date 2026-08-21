using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

[Serializable]
public struct GameTimeSpan
{
    public int totalMilliseconds;

    public int totalSeconds =>  totalMilliseconds / 1000; 
    public int totalMinutes =>  totalMilliseconds / 1000 / 60;
    public int totalHours =>  totalMilliseconds / 1000 / 60 / 60;
    public int totalDays => totalMilliseconds / 1000 / 60 / 60 / 24;

    public float totalSecondsF => (float)totalMilliseconds / 1000;
    public float totalMinutesF => (float)totalMilliseconds / 1000 / 60;
    public float totalHoursF => (float)totalMilliseconds / 1000 / 60 / 60;
    public float totalDaysF => (float)totalMilliseconds / 1000 / 60 / 60 / 24;

    public int millisecondsPart => Mathf.Abs(totalMilliseconds) % 1000;
    public int secondsPart => Mathf.Abs(totalSeconds) % 60;
    public int minutesPart => Mathf.Abs(totalMinutes) % 60;
    public int hoursPart => Mathf.Abs(totalHours) % 24;
    public int daysPart => Mathf.Abs(totalDays);

    public bool isNegative => totalMilliseconds < 0;
    public static readonly GameTimeSpan zero = default;

    public GameTimeSpan(int milliseconds)
    {
        totalMilliseconds = milliseconds;
    }

    /// Parse a timespan, valid forms are:
    ///     0:00            (hours:minutes)
    ///     0:00:00         (hours:minutes:seconds)
    ///     0:00:00.000     (hours:minutes:seconds.milliseconds)
    ///     0d              (days)
    ///     0d 0:00         (days hours:minutes)
    ///     0d 0:00:00      (days hours:minutes:seconds)
    ///     0d 0:00:00.000  (days hours:minutes:seconds.milliseconds)
    ///
    /// Negative timespans may also be parsed if the string leads with a "-",
    /// for example, "-3d 2:00" is equivalent to -GameTimeSpan.FromDaysHoursMinutes(3, 2, 0)
    static Regex timeSpanPattern = new Regex(@"
        ^\s*(?<negate>-)?(((?<days>\d) d)\s*)?
        (
            (\b(?<hours>[0-9]|1[0-9]|2[0-3])\b\:
            \b(?<minutes>0[0-9]|[1-5][0-9])\b)
            (
                (\: (?<seconds>0[0-9]|[1-5][0-9])) 
                ((\:|\.)(?<milliseconds>\d{3})?\b )?
            )?
        )?\s*$", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

    public static bool TryParse(string s, out GameTimeSpan timeSpan)
    {
        timeSpan = new GameTimeSpan();
        var match = timeSpanPattern.Match(s);
        if (!match.Success)
            return false;
        
        int daysPart = match.Groups["days"].Success ? int.Parse(match.Groups["days"].Value) : 0;
        int hoursPart = match.Groups["hours"].Success ? int.Parse(match.Groups["hours"].Value) : 0;
        int minutesPart = match.Groups["minutes"].Success ? int.Parse(match.Groups["minutes"].Value) : 0;
        int secondsPart = match.Groups["seconds"].Success ? int.Parse(match.Groups["seconds"].Value) : 0;
        int millisecondsPart = match.Groups["milliseconds"].Success ? int.Parse(match.Groups["milliseconds"].Value) : 0;

        timeSpan = FromTimeParts(days: daysPart, hours: hoursPart, minutes: minutesPart, seconds: secondsPart, milliseconds: millisecondsPart);
        if (match.Groups["negate"].Success)
            timeSpan = -timeSpan;
        
        return true;
    }

    public static GameTimeSpan Parse(string s)
    {
        if (!TryParse(s, out var timeSpan))
            throw new System.ArgumentException(nameof(s), "Invalid GameTimeSpan string");
        return timeSpan;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        if(totalMilliseconds < 0)
            sb.Append("-");

        if (daysPart >= 1)
            sb.Append($"{daysPart}d");

        if (daysPart == 0 || hoursPart > 0 || minutesPart > 0 || secondsPart > 0 || millisecondsPart > 0)
        {
            if (daysPart > 0)
                sb.Append(' ');
            sb.Append($"{hoursPart}:{minutesPart:D2}");
            
            if (secondsPart > 0 || millisecondsPart > 0)
            {
                sb.Append($":{secondsPart:D2}");
                if (millisecondsPart > 0)
                    sb.Append($".{millisecondsPart:D3}");
            }
        }

        return sb.ToString();
    }

    public string ToShortString(bool showSeconds = false)
    {
        var sb = new StringBuilder();
        if(totalMilliseconds < 0)
            sb.Append("-");

        if (daysPart >= 1)
            sb.Append($"{daysPart}d");

        if (daysPart == 0 || hoursPart > 0 || minutesPart > 0 || secondsPart > 0 || millisecondsPart > 0)
        {
            if (daysPart > 0)
                sb.Append(' ');
            sb.Append($"{hoursPart}:{minutesPart:D2}");
            
            if (secondsPart > 0 && showSeconds)
                sb.Append($":{secondsPart:D2}");
        }

        return sb.ToString();
    }

    public string ToDisplayString()
    {
        var sb = new StringBuilder();
        if (daysPart >= 1)
            sb.Append($"{daysPart} d");

        if (hoursPart > 0)
        {
            if (daysPart > 0)
                sb.Append(" ");
            sb.Append($"{hoursPart} h");
        }

        if (minutesPart > 0)
        {
            if (hoursPart > 0)
                sb.Append(" ");
            sb.Append($"{minutesPart} m");
        }

        if (secondsPart > 0)
        {
            if (minutesPart > 0)
                sb.Append(" ");
            sb.Append($"{secondsPart} s");
        }

        return sb.ToString();
    }

    public static GameTimeSpan FromTimeParts(int days = 0, int hours = 0, int minutes = 0, int seconds = 0, int milliseconds = 0)
    {
        return new GameTimeSpan
        (
            days * 1000 * 60 * 60 * 24 +
            hours * 1000 * 60 * 60 +
            minutes * 1000 * 60 +
            seconds * 1000 +
            milliseconds

        );
    }

    public static GameTimeSpan Min(GameTimeSpan a, GameTimeSpan b)
    {
        return a < b ? a : b;
    }

    public static GameTimeSpan Max(GameTimeSpan a, GameTimeSpan b)
    {
        return a > b ? a : b;
    }

    public static GameTimeSpan operator +(GameTimeSpan a, GameTimeSpan b)
    {
        return new GameTimeSpan(a.totalMilliseconds + b.totalMilliseconds);
    }

    public static GameTimeSpan operator -(GameTimeSpan a, GameTimeSpan b)
    {
        return a + -b;
    }

    public static GameTimeSpan operator -(GameTimeSpan a)
    {
        return new GameTimeSpan(-a.totalMilliseconds);
    }

    public static GameTimeSpan operator *(GameTimeSpan a, int i)
    {
        return new GameTimeSpan(a.totalMilliseconds * i);
    }

    public static GameTimeSpan operator /(GameTimeSpan a, int i)
    {
        if (i == 0)
            throw new DivideByZeroException();
        return new GameTimeSpan(a.totalMilliseconds / i);
    }

    public static bool operator ==(GameTimeSpan a, GameTimeSpan b) => a.Equals(b);
    public static bool operator !=(GameTimeSpan a, GameTimeSpan b) => !a.Equals(b);

    public static bool operator >(GameTimeSpan a, GameTimeSpan b) => a.CompareTo(b) > 0;
    public static bool operator <(GameTimeSpan a, GameTimeSpan b) => a.CompareTo(b) < 0;

    public static bool operator >=(GameTimeSpan a, GameTimeSpan b) => a.CompareTo(b) >= 0;
    public static bool operator <=(GameTimeSpan a, GameTimeSpan b) => a.CompareTo(b) <= 0;

    public int CompareTo(GameTimeSpan other)
    {
        return totalMilliseconds.CompareTo(other.totalMilliseconds);
    }

    public override bool Equals(object obj)
    {
        if (obj is GameTimeSpan other)
            return totalMilliseconds == other.totalMilliseconds;
        return false;
    }

    public override int GetHashCode()
    {
        return totalMilliseconds.GetHashCode();
    }

}
