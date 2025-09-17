using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moe.TextEngine
{
    public record class FontRequest(FontOptions Options, int GlyphIndex)
    {
        public override int GetHashCode()
        {
            return HashCode.Combine(Options, GlyphIndex);
        }
    }

}
