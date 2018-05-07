using System.Collections.Generic;

[System.Serializable]
public class Range {

    public int lower;
    public int upper;

    public static Dictionary<Range, string> titles = new Dictionary<Range, string>()
    {
        { new Range(0,300), "Novice" }, {  new Range(5000,100000), "Mars"}
    };

    public Range(int Lower, int Upper)
    {
        lower = Lower;
        upper = Upper;
    }

    public bool InRange(int number)
    {
        return lower <= number && number < upper;
    }

    public static string FindTitle(int rank)
    {
        foreach(KeyValuePair<Range,string> pair in titles)
        {
            if (pair.Key.InRange(rank))
                return pair.Value;
        }
        return "";
    }
}

