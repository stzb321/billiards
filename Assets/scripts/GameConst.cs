using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GameConst
{
    public enum GameState
    {
        None,
        FindPlace,
        Aim,
        LoadForce,
        Rolling,
        GameOver,
    }

    public class BallsTag
    {
        public static string WhiteBall = "WhiteBall";
    }
}
