using System;
using UnityEngine;

public class InfoLoader : MonoBehaviour {

    public static UserInfo user;
    public static int playerID;
    public static Database database;
    public static string switchSceneCaller = "Main";
    private static bool called = false;

    private void Awake()
    {
        if (!called)
        {
            database = new Database();
            user = new CheatAccount(); // This line should be
                                       // user = new UserInfo();
                                       // download Json
                                       // user.JsonToClass();
            playerID = user.playerID;
            called = true;
        }
    }

    public static string Vec2ToString(Vector2Int vec) { return vec.x.ToString() + vec.y.ToString(); }
    public static Vector2Int StringToVec2(string loc) { return new Vector2Int((int)Char.GetNumericValue(loc[0]), (int)Char.GetNumericValue(loc[1])); }
}