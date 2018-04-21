﻿using System;
using UnityEngine;

public class Stats {

    public int gamesPlayed = 0;
    public int win = 0, lose = 0, draw = 0;
    public double percentage = 0;
    public Stats(int winCount, int loseCount, int drawCount)
    {
        win = winCount;
        lose = loseCount;
        draw = drawCount;
        gamesPlayed = winCount + loseCount + drawCount;
        SetPercentage();
    }

    public void Win()
    {
        win++;
        SetPercentage();
    }

    public void Lost()
    {
        lose++;
        SetPercentage();
    }

    public void Draw()
    {
        draw++;
        SetPercentage();
    }

    public void SetPercentage()
    {
        if (gamesPlayed == 0) percentage = 0;
        else percentage = Math.Round(((double)win / gamesPlayed * 100), 2);
    }
}
