using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moe.TextEngine;

public class FontOptions : IEquatable<FontOptions>
{
    public int PixelWidth { get; set; } = 12;

    public int PixelHeight { get; set; } = 12;

    public bool Equals(FontOptions? other)
    {
        if (other == null)
        {
            return false;
        }

        return PixelWidth == other.PixelWidth && PixelHeight == other.PixelHeight;
    }

    public override bool Equals(object? obj)
    {
        return ((IEquatable<FontOptions>)this).Equals(obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PixelWidth, PixelHeight);
    }
}
