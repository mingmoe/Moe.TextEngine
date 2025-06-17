using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moe.TextEngine;

public readonly struct ShapedCharacter
{
    public readonly uint GlyphIndex;

    public readonly Vector2D Advance;

    public readonly Vector2D Offset;

    public ShapedCharacter() { }

    public ShapedCharacter(uint codePoint, Vector2D advance, Vector2D offset)
    {
        GlyphIndex = codePoint;
        Advance = advance;
        Offset = offset;
    }
}
