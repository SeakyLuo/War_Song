using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Newtonsoft.Json.Serialization;

[Serializable]
public class Location
{
    public int x;
    public int y;
    public Location()
    {
        x = 0;
        y = 0;
    }
    public Location(int X, int Y)
    {
        x = X;
        y = Y;
    }
    public Location(Location location)
    {
        x = location.x;
        y = location.y;
    }
    public Location(string location)
    {
        var parts = location.Split(new char[] { '(', ')', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        x = int.Parse(parts[0]);
        y = int.Parse(parts[1]);
    }
    public readonly static Location zero = new Location(0, 0);
    public readonly static Location NoLocation = new Location(-1, -1);
    public bool Between(Location a, Location b, string compare = "XY")
    {
        /// Compare can only be "XY" or "X" or "Y". a and b are exclusive.
        if (compare == "XY") return a < this && this < b;
        if (compare == "X") return y == a.y && y == b.y && a.x < x && x < b.x;
        if (compare == "Y") return x == a.x && x == b.x && a.y < y && y < b.y;
        else return false;
    }
    public static bool CorrectFormat(string str)
    {
        return str[0] == '(' && str.EndsWith(")") && str.Contains(", ");
    }

    public override string ToString()
    {
        return string.Format("({0}, {1})", x, y);
    }
    public override bool Equals(object obj)
    {
        var location = obj as Location;
        return this == location;
    }
    public override int GetHashCode()
    {
        var hashCode = 1502939027;
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        return hashCode;
    }
    public static Location operator +(Location a, Location b)
    {
        return new Location(a.x + b.x, a.y + b.y);
    }
    public static Location operator -(Location a, Location b)
    {
        return new Location(a.x - b.x, a.y - b.y);
    }
    public static Location operator *(Location a, int b)
    {
        return new Location(a.x * b, a.y * b);
    }
    public static Location operator *(Location a, Location b)
    {
        return new Location(a.x * b.x, a.y * b.y);
    }
    public static bool operator ==(Location a, Location b)
    {
        return a.x == b.x && a.y == b.y;
    }
    public static bool operator !=(Location a, Location b)
    {
        return !(a == b);
    }
    public static bool operator <(Location a, Location b)
    {
        return a.x < b.x && a.y < b.y;
    }
    public static bool operator >(Location a, Location b)
    {
        return a.x > b.x && a.y > b.y;
    }
    public static bool operator <=(Location a, Location b)
    {
        return a.x <= b.x && a.y <= b.y;
    }
    public static bool operator >=(Location a, Location b)
    {
        return a.x >= b.x && a.y >= b.y;
    }
}

public class DictionaryAsArrayResolver : DefaultContractResolver
{
    protected override JsonContract CreateContract(Type objectType)
    {
        if (objectType.GetInterfaces().Any(i => i == typeof(IDictionary) ||
            (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>))))
        {
            return base.CreateArrayContract(objectType);
        }

        return base.CreateContract(objectType);
    }
}
