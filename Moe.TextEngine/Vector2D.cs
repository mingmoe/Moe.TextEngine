using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moe.TextEngine;

public readonly struct Vector2D
{
    public Vector2D()
    {

    }

    public Vector2D(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Vector2D WithOffset(Vector2D current, Vector2D offset)
    {
        return new Vector2D(current.X + offset.X, current.Y + offset.Y);
    }

    public readonly int X;
    public readonly int Y;
}
