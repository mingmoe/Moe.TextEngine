using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moe.TextEngine;

public class Font : IEquatable<Font>
{
    public IFontSource Source { get; init; }

    public int FaceIndex { get; init; }

    public Font(IFontSource source, int faceIndex)
    {
        Source = source;
        FaceIndex = faceIndex;
    }

    public bool Equals(Font? other)
    {
        if (other == null)
        {
            return false;
        }

        return Source.Equals(other.Source) && FaceIndex == other.FaceIndex;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }
        if (obj is Font other)
        {
            return Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Source, FaceIndex);
    }
}
