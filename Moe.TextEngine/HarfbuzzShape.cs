using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moe.TextEngine;

public class HarfbuzzShape
{
    public HarfbuzzShape(HarfbuzzShapeEngine engine, Font font)
    {
        Engine = engine;
        Font = font;
        _blob = new(
            Font.Source.GetRawData(),
            Font.Source.GetDataSize(),
            HarfBuzzSharp.MemoryMode.ReadOnly);
        _buffer = new();
        _face = new(_blob, font.FaceIndex);
        _font = new(_face);
    }

    private HarfBuzzSharp.Blob _blob;

    private HarfBuzzSharp.Face _face;

    private HarfBuzzSharp.Font _font;

    private HarfBuzzSharp.Buffer _buffer;

    public HarfbuzzShapeEngine Engine { get; init; }

    public Font Font { get; init; }

    public IEnumerable<ShapedCharacter> Shape(ShapeRun run)
    {
        List<ShapedCharacter> shapedCharacters = new(run.Text.Length);

        _buffer.Reset();

        _font.SetScale(run.FontOptions.PixelWidth, run.FontOptions.PixelHeight);

        _buffer.AddUtf16(run.Text, run.Offset, run.Length ?? run.Text.Length - run.Offset);

        if (run.ShapeOptions != null)
        {
            _buffer.Script = HarfBuzzSharp.Script.Parse(run.ShapeOptions.Script);
            _buffer.Language = new HarfBuzzSharp.Language(run.ShapeOptions.Language);
            switch (run.ShapeOptions.Direction)
            {
                case TextDirection.LeftToRight:
                    _buffer.Direction = HarfBuzzSharp.Direction.LeftToRight;
                    break;
                case TextDirection.RightToLeft:
                    _buffer.Direction = HarfBuzzSharp.Direction.RightToLeft;
                    break;
                case TextDirection.TopToBottom:
                    _buffer.Direction = HarfBuzzSharp.Direction.TopToBottom;
                    break;
                case TextDirection.BottomToTop:
                    _buffer.Direction = HarfBuzzSharp.Direction.BottomToTop;
                    break;
            }
        }
        else
        {
            _buffer.GuessSegmentProperties();
        }

        _font.Shape(_buffer, []);

        var glyphs = _buffer.GetGlyphPositionSpan();
        var infos = _buffer.GetGlyphInfoSpan();

        Debug.Assert(glyphs.Length == infos.Length);

        int index = 0;
        while (index != glyphs.Length)
        {
            var glyph = glyphs[index];
            var info = infos[index];

            shapedCharacters.Add(new(
                info.Codepoint,
                new Vector2D(glyph.XAdvance, glyph.YAdvance),
                new Vector2D(glyph.XOffset, glyph.YOffset)));

            index++;
        }

        return shapedCharacters;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _buffer.Dispose();
        _font.Dispose();
        _face.Dispose();
        _blob.Dispose();
    }
}
