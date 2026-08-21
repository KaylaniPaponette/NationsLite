using System;
using System.Text;
using System.Text.RegularExpressions;

public struct GameTime : IComparable<GameTime>
{
    public int totalMilliseconds;

    public GameTime(int milliseconds)
    {
        totalMilliseconds = milliseconds;
    }

    public int totalSeconds => totalMilliseconds / 1000;
    public int totalMinutes => totalMilliseconds / 1000 / 60;
    public int totalHours => totalMilliseconds / 1000 / 60 / 60;
    public int totalDays => totalMilliseconds / 1000 / 60 / 60 / 24;

    public float totalSecondsF => (float)totalMilliseconds / 1000;
    public float totalMinutesF => (float)totalMilliseconds / 1000 / 60;
    public float totalHoursF => (float)totalMilliseconds / 1000 / 60 / 60;
    public float totalDaysF => (float)totalMilliseconds / 1000 / 60 / 60 / 24;

    public int millisecondsPart => totalMilliseconds % 1000;
    public int secondsPart => (totalMilliseconds / 1000) % 60;
    public int minutesPart => (totalMilliseconds / 1000 / 60) % 60;

    public int hours24Part => (totalMilliseconds / 1000 / 60 / 60) % 24;
    public int hours12Part => (totalHours % 12) != 0 ? (totalHours % 12) : 12;

    public bool isAm => (totalSeconds % (24 * 60 * 60)) < 12 * 60 * 60;
    public bool isPm => (totalSeconds % (24 * 60 * 60)) >= 12 * 60 * 60;
    public bool isValid => totalMilliseconds >= 0 && totalMilliseconds < kMillisecondsInDay; 

    public static GameTime minValue => new GameTime(0);
    public static GameTime maxValue => GameTime.FromTimeParts(23, 59, 59, 999);
    public const int kMillisecondsInDay = 86_400_000;

static Regex timePattern = new Regex(@"
        ^\s* (?<hour>[0-9]|1[0-2])
        ( \: (?<minute>0[0-9]|[1-5][0-9]) 
            ( \: (?<second>0[0-9]|[1-5][0-9]) 
                ( \. (?<ms>\d\d\d) )?
            )?
        )? \s* (?<ampm>[ap]m)? \s*$", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

    public static bool TryParse(string s, out GameTime time)
    {
        time = new GameTime();
        var match = timePattern.Match(s);
        if (!match.Success)
            return false;
        
        int hoursPart = int.Parse(match.Groups["hour"].Value);
        int minutesPart = match.Groups["minute"].Success ? int.Parse(match.Groups["minute"].Value) : 0;
        int secondsPart = match.Groups["second"].Success ? int.Parse(match.Groups["second"].Value) : 0;
        int msPart = match.Groups["ms"].Success ? int.Parse(match.Groups["ms"].Value) : 0;

        if (!match.Groups["ampm"].Success)
            time = GameTime.FromTimeParts(hours: hoursPart, minutes: minutesPart, seconds: secondsPart, milliseconds: msPart);
        else 
        {
            bool isPm = match.Groups["ampm"].Value.ToLowerInvariant() == "pm";
            time = GameTime.FromTimePartsAmPm(hours: hoursPart, minutes: minutesPart, seconds: secondsPart, milliseconds: msPart, isPm: isPm);
        }
        return true;
    }

    public static GameTime Parse(string s)
    {
        if (!TryParse(s, out var time))
            throw new System.ArgumentException(s, nameof(s));
        return time;
    }

    public static GameTime FromTimeParts(int hours = 0, int minutes = 0, int seconds = 0, int milliseconds = 0)
    {
        return new GameTime((hours % 24) * 60 * 60 * 1000 + minutes * 60 * 1000 + seconds * 1000 + milliseconds);
    }

    public static GameTime FromTimePartsAmPm(int hours = 0, int minutes = 0, int seconds = 0, int milliseconds = 0, bool isPm = false)
    {
        if (hours == 12)
            hours -= 12;
        if (isPm)
            hours += 12;
        return GameTime.FromTimeParts(hours, minutes, seconds, milliseconds);
    }

    public static GameTime Min(GameTime a, GameTime b)
    {
        return new GameTime(Math.Min(a.totalMilliseconds, b.totalMilliseconds));
    }

    public static GameTime Max(GameTime a, GameTime b)
    {
        return new GameTime(Math.Max(a.totalMilliseconds, b.totalMilliseconds));
    }

    public static GameTime operator +(GameTime a, GameTime b)
    {
        return new GameTime(a.totalMilliseconds + b.totalMilliseconds);
    }

    public static GameTime operator -(GameTime a, GameTime b)
    {
        return a + -b;
    }

    public static GameTime operator -(GameTime a)
    {
        return new GameTime(-a.totalMilliseconds);
    }

    public static bool operator ==(GameTime a, GameTime b) => a.Equals(b);
    public static bool operator !=(GameTime a, GameTime b) => !a.Equals(b);

    public static bool operator >(GameTime a, GameTime b) => a.CompareTo(b) > 0;
    public static bool operator <(GameTime a, GameTime b) => a.CompareTo(b) < 0;

    public static bool operator >=(GameTime a, GameTime b) => a.CompareTo(b) >= 0;
    public static bool operator <=(GameTime a, GameTime b) => a.CompareTo(b) <= 0;

    public int CompareTo(GameTime other)
    {
        return totalMilliseconds.CompareTo(other.totalMilliseconds);
    }

    public override bool Equals(object obj)
    {
        if (obj != null && obj is GameTime gt)
            return gt.totalMilliseconds == totalMilliseconds;

        return false;
    }

    public override int GetHashCode()
    {
        return totalMilliseconds.GetHashCode();
    }

    public string ToString24Hour()
    {
        var sb = new StringBuilder();
        if (totalMilliseconds < 0)
            sb.Append("-");

        sb.Append($"{hours24Part:D2}:{minutesPart:D2}");

        if (secondsPart > 0 || millisecondsPart > 0)
        {
            sb.Append($":{secondsPart:D2}");
            if (millisecondsPart > 0)
                sb.Append($".{millisecondsPart:D3}");
        }

        return sb.ToString();
    }

    public string ToString12Hour()
    {
        var sb = new StringBuilder();
        if (totalMilliseconds < 0)
            sb.Append("-");

        sb.Append($"{hours12Part}:{minutesPart:D2}");

        if (secondsPart > 0 || millisecondsPart > 0)
        {
            sb.Append($":{secondsPart:D2}");
            if (millisecondsPart > 0)
                sb.Append($".{millisecondsPart:D3}");
        }

        sb.Append(isAm ? " AM" : " PM");
        return sb.ToString();
    }

    public string ToString12HourMinutes()
    {
        var sb = new StringBuilder();
        if (totalMilliseconds < 0)
            sb.Append("-");
        
        sb.Append($"{hours12Part}:{minutesPart:D2}");
        sb.Append(isAm ? " AM" : " PM");
        return sb.ToString();
    }

    public override string ToString()
    {
        return ToString12Hour();
    }
}
