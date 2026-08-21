using System;
using System.Text.RegularExpressions;
using UnityEngine;

public struct GameDateTime : IComparable<GameDateTime>
{
    public int day;
    public GameTime time;

    public GameDateTime(int day, GameTime gameTime)
    {
        this.day = day;
        time = gameTime;
    }

    public static GameDateTime Min(GameDateTime a, GameDateTime b)
    {
        if (a.day != b.day)
            return a.day < b.day ? a : b;
        else
            return a.time < b.time ? a : b;
    }

    public static GameDateTime Max(GameDateTime a, GameDateTime b)
    {
        if (a.day != b.day)
            return a.day > b.day ? a : b;
        else
            return a.time > b.time ? a : b;
    }

    public static bool operator ==(GameDateTime a, GameDateTime b) => a.Equals(b);
    public static bool operator !=(GameDateTime a, GameDateTime b) => !a.Equals(b);

    public static bool operator >(GameDateTime a, GameDateTime b) => a.CompareTo(b) > 0;
    public static bool operator <(GameDateTime a, GameDateTime b) => a.CompareTo(b) < 0;

    public static bool operator >=(GameDateTime a, GameDateTime b) => a.CompareTo(b) >= 0;
    public static bool operator <=(GameDateTime a, GameDateTime b) => a.CompareTo(b) <= 0;


    public static GameDateTime operator +(GameDateTime a, GameTimeSpan b)
    {
        long dateTimeInMilliseconds = (long)a.day * GameTime.kMillisecondsInDay + a.time.totalMilliseconds;
        long sum = dateTimeInMilliseconds + b.totalMilliseconds;

        int day = (int)(sum / GameTime.kMillisecondsInDay);
        long time = sum % GameTime.kMillisecondsInDay;
        
        return new GameDateTime(day, new GameTime((int)time));
    }
    
    public static GameTimeSpan operator -(GameDateTime a, GameDateTime b)
    {
        return new GameTimeSpan((a.day - b.day) * GameTime.kMillisecondsInDay + 
            (a.time.totalMilliseconds - b.time.totalMilliseconds));
    }

    public static GameDateTime operator -(GameDateTime a, GameTimeSpan b)
    {
        return a + -b;
    }

    public override bool Equals(object other)
    {
        if (other is GameDateTime dt)
            return CompareTo(dt) == 0;

        return false;
    }

    public int CompareTo(GameDateTime other)
    {
        if (day != other.day)
            return day.CompareTo(other.day);
        
        return time.CompareTo(other.time);
    }

    public override int GetHashCode()
    {
        return day.GetHashCode() + time.GetHashCode();   
    }

    public override string ToString()
    {
        return $"(D{day} {time.ToString12HourMinutes()})";
    }

    static Regex dateTimePattern = new Regex(@"^\s*D(\d+)\s*(\d.*)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static bool TryParse(string s, out GameDateTime result)
    {
        result = default;
        var m = dateTimePattern.Match(s);
        if (!m.Success)
            return false;

        if (!int.TryParse(m.Groups[1].Value, out int day))
            return false;

        GameTime time = default;
        if (m.Groups[2].Success && !GameTime.TryParse(m.Groups[2].Value, out time))
            return false;

        result = new GameDateTime(day, time);
        return true;
    }

    public static GameDateTime Parse(string s)
    {
        if (!TryParse(s, out var result))
            throw new ArgumentException(s, nameof(s));
        return result;
    }
}