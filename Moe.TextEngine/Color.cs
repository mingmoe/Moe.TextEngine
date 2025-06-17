using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moe.TextEngine;

public readonly struct Color
{
    public readonly byte Red;
    public readonly byte Green;
    public readonly byte Blue;
    public readonly byte Alpha;

    public Color()
    {
    }

    public Color(byte red, byte green, byte blue, byte alpha)
    {
        Red = red;
        Green = green;
        Blue = blue;
        Alpha = alpha;
    }

    public readonly uint ToAGRB()
    {
        return BitConverter.ToUInt32([Alpha, Green, Red, Blue]);
    }

    public static Color FromAGRB(uint agrb)
    {
        var data = BitConverter.GetBytes(agrb);

        return new Color(data[2], data[1], data[3], data[0]);
    }
}
