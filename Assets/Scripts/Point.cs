using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Point {

    int x;
    int y;


    public int X
    {
        get
        {
            return x;
        }

       private set
        {
            x = value;
        }
    }

    public int Y
    {
        get
        {
            return y;
        }

      private  set
        {
            y = value;
        }
    }


    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static bool operator ==(Point x, Point y)
    {
        return x.X == y.X && y.Y == y.Y;
    }

    public static bool operator !=(Point x, Point y)
    {
        return x.X != y.X && y.Y != y.Y;
    }
}
