using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSettings
{
    public static bool gameOver;
    public static int moveCount;

    public const int minRings = 3, maxRings = 7;
    public static int ringCount = minRings;
}