using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moe.ResourcesManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Moe.TextEngine;

public sealed class RenderEngine
{
    public GraphicsDevice Device { get; init; }

    public required FreeTypeEngine RasterizerEngine { get; init; }

    public required HarfbuzzShapeEngine ShapeEngine { get; init; }

    /// <summary>
    /// this decides the sort of the font,making some font as backup and not be used first.
    /// </summary>
    public List<Font> Fonts { get; init; } = [];

    public Dictionary<Font, AtlasManager<FontRequest>> Cache { get; init; } = [];

    public Dictionary<Font, HarfbuzzShape> Shapes { get; init; } = [];

    public Dictionary<Font, Dictionary<FontRequest,GlyphInfo>> GlyphCache { get; init; } = [];

    public Dictionary<Font, FreeTypeFontRasterizer> Rasterizers { get; init; } = [];

    public RenderEngine(GraphicsDevice device)
    {
        Device = device;
    }

    public void AddFont(Font font)
    {
        if (Cache.ContainsKey(font))
        {
            return;
        }

        Fonts.Add(font);
        Cache.Add(font, new(Device, font.Source.ResourceId));
        Shapes.Add(font, ShapeEngine.Create(font));
        Rasterizers.Add(font, RasterizerEngine.Create(font));
    }

    private Font? FindFont(Rune rune)
    {
        foreach (var font in ListHelper.FastReverse(Fonts))
        {
            var rasterizers = Rasterizers[font];

            var index = rasterizers.CovertToCharIndex(rune);

            if (index != 0)
            {
                return font;
            }
        }
        return Fonts.Last();
    }

    private List<(ShapeRun, Font)> SplitRun(IList<Font> fonts, IList<Rune> rune, ShapeRun parentRun)
    {
        List<(ShapeRun, Font)> runs = [];
        Font? currentFont = null;

        int index = 0;
        int utf16Index = 0;
        int utf16StartIndex = 0;

        while (index < fonts.Count)
        {
            var output = fonts[index];
            var codePoint = rune[index];

            var advance = codePoint.IsBmp ? 1 : 2;

            if (currentFont == null)
            {
                currentFont = output!;
            }
            else if (!object.ReferenceEquals(currentFont, output))
            {
                {
                    // add new run
                    runs.Add((new ShapeRun(parentRun.FontOptions, parentRun.ShapeOptions)
                    {
                        Text = parentRun.Text,
                        Length = utf16Index - utf16StartIndex,
                        Offset = parentRun.Offset + utf16StartIndex,
                    }, currentFont!));

                    currentFont = output;
                    utf16StartIndex = utf16Index;
                }
            }

            index++;
            utf16Index += advance;
        }

        if (utf16StartIndex < parentRun.Text.Length)
        {
            runs.Add((new ShapeRun(parentRun.FontOptions, parentRun.ShapeOptions)
            {
                Text = parentRun.Text,
                Offset = utf16StartIndex + parentRun.Offset,
                Length = utf16Index - utf16StartIndex,
            },
            currentFont!));
        }

        return runs;
    }

    private List<(IEnumerable<ShapedCharacter>, Font)> ShapeRuns(List<(ShapeRun, Font)> run)
    {
        List<(IEnumerable<ShapedCharacter>, Font)> shaped = [];
        foreach (var oneRun in run)
        {
            var runOptions = oneRun.Item1;
            var font = oneRun.Item2;

            shaped.Add((Shapes[font].Shape(runOptions), font));
        }

        return shaped;
    }

    public void DrawString(ShapeRun run, Point place,SpriteBatch batch)
    {
        DrawString(
            run,
            (point, texture, rect) =>
            {
                var target = place + point;
                batch.Draw(texture,
                    new Rectangle(target.X, target.Y, rect.Width,rect.Height),
                    rect, 
                    Microsoft.Xna.Framework.Color.White);
                return true;
            });
    }

    public Point MeasureSize(ShapeRun run)
    {
        int xMax = 0;
        int yMax = 0;

        DrawString(run,
            (pen, _, rect) =>
            {
                xMax = int.Max(xMax, pen.X + rect.Width);
                yMax = int.Max(yMax, pen.Y + rect.Height);
                return true;
            });

        return new(xMax, yMax);
    }

    public void DrawString(ShapeRun run, Func<Point, Texture2D, Rectangle,bool> draw)
    {
        var fontOptions = run.FontOptions;
        var text = run.UsedText;

        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        List<Rune> codePoints = [.. text.EnumerateRunes()];
        var outputs = codePoints.Select(FindFont).ToList();
        outputs.RemoveAll((v) => v == null);

        List<(ShapeRun, Font)> runs = SplitRun(outputs!, codePoints, run);

        // shaped
        List<(IEnumerable<ShapedCharacter>, Font)> shaped = ShapeRuns(runs);

        // shape
        Point pen = new(0, 0);

        foreach (var shape in shaped)
        {
            var shapedCharacters = shape.Item1;
            var font = shape.Item2;
            var cache = Cache[font];
            var rasterizers = Rasterizers[font];

            foreach (var shapedCharacter in shapedCharacters)
            {
                var request = new FontRequest(run.FontOptions,
                    BitConverter.ToInt32(BitConverter.GetBytes(shapedCharacter.GlyphIndex)));

                if (!cache.TryGet(request, out var bitmap))
                {
                    var size = rasterizers.LoadChar(shapedCharacter.GlyphIndex, fontOptions);
                    var allocated = cache.Alloc(request, new(size.Width, size.Rows));
                    rasterizers.WriteTo(allocated.Item1, allocated.Item2);
                    bitmap = allocated;

                    if (!GlyphCache.ContainsKey(font))
                    {
                        GlyphCache.TryAdd(font, []);
                    }

                    GlyphCache[font][request] = size;
                }

                GlyphInfo glyphInfo = GlyphCache[font][request];

                // write
                if (!draw.Invoke(
                        new(pen.X + shapedCharacter.Offset.X,
                            pen.Y + shapedCharacter.Offset.Y + fontOptions.PixelHeight - glyphInfo.BearingY),
                        bitmap.Value.Item1,
                        bitmap.Value.Item2))
                {
                    return;
                }

                pen.X += shapedCharacter.Advance.X;
                pen.Y += shapedCharacter.Advance.Y;
            }
        }
    }


}
