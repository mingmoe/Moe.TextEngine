using FreeTypeSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FreeTypeSharp.FT;

namespace Moe.TextEngine;

public class FreeTypeFontRasterizer
{
    private bool _disposed = false;

    public readonly unsafe FT_FaceRec_* Face;

    public FreeTypeFontRasterizer(FreeTypeEngine engine, Font font)
    {
        FreeType = engine;
        Font = font;

        unsafe
        {
            FT_FaceRec_* face;
            FT_New_Memory_Face(engine.Library, (byte*)font.Source.GetRawData(), font.Source.GetDataSize(), font.FaceIndex, &face);
            Face = face;

            FT_Select_Charmap(Face, FT_Encoding_.FT_ENCODING_UNICODE);
            ApplyFontOptions();
        }
    }

    private void ApplyFontOptions()
    {
        unsafe
        {
            FT_Select_Charmap(Face, FT_Encoding_.FT_ENCODING_UNICODE);

            FT_Set_Pixel_Sizes(Face, (uint)Options.PixelWidth, (uint)Options.PixelHeight);
        }
    }

    public FreeTypeEngine FreeType { get; init; }

    public FreeTypeEngine Engine => FreeType;

    public Font Font { get; init; }

    public FontOptions Options { get; set; } = new();

    public long CovertToCharIndex(Rune rune)
    {
        unsafe
        {
            var i = FT_Get_Char_Index(Face,
                BitConverter.ToUInt32(BitConverter.GetBytes(rune.Value)));

            if (i == 0)
            {
                throw new FreeTypeException(FT_Error.FT_Err_Ok, "get an undefined character code");
            }

            return i;
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Dispose(true);
    }

    ~FreeTypeFontRasterizer()
    {
        Dispose(false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (disposing)
        {

        }

        unsafe
        {
            FT_Done_Face(Face);
        }
    }

    /// <summary>
    /// TODO:SUPPORT SVG?
    /// </summary>
    /// <param name="charIndex"></param>
    public GlyphInfo LoadChar(long charIndex)
    {
        uint index;
        checked
        {
            index = (uint)charIndex;
        }
        unsafe
        {
            var error = FT_Load_Glyph(Face, index, FT_LOAD.FT_LOAD_COLOR | FT_LOAD.FT_LOAD_NO_SVG);

            Helper.AssertFTError(error);

            error = FT_Render_Glyph(Face->glyph, FT_Render_Mode_.FT_RENDER_MODE_NORMAL);

            Helper.AssertFTError(error);

            return new GlyphInfo()
            {
                BearingX = Face->glyph->bitmap_left,
                BearingY = Face->glyph->bitmap_top,
                Width = (int)Face->glyph->bitmap.width,
                Rows = (int)Face->glyph->bitmap.rows
            };
        }
    }

    private void WriteGray(Rectangle dstRange, uint[] buffer)
    {
        unsafe
        {
            var width = Face->glyph->bitmap.width;
            var rows = Face->glyph->bitmap.rows;
            var pitch = Face->glyph->bitmap.pitch;
            var buf = Face->glyph->bitmap.buffer;
            var maxGray = Face->glyph->bitmap.num_grays;

            if (dstRange.Width != width || dstRange.Height != rows)
            {
                throw new ArgumentOutOfRangeException(nameof(dstRange), "dstRange.Width != width || dstRange.Height != rows");
            }

            int pointer = 0;

            for (int yIndex = 0; yIndex < rows; yIndex++)
            {
                for (int xIndex = 0; xIndex < width; xIndex++)
                {
                    byte value = buf[pointer + xIndex];

                    buffer[(yIndex * width) + xIndex] =
                        new Color(0, 0, 0, (byte)Math.Floor((double)value / maxGray * byte.MaxValue)).ToAGRB();
                }

                pointer += pitch;
            }
        }
    }

    private void WriteRGBA(Rectangle dstRange, uint[] buffer)
    {
        unsafe
        {
            var width = Face->glyph->bitmap.width;
            var rows = Face->glyph->bitmap.rows;
            var pitch = Face->glyph->bitmap.pitch;
            var buf = Face->glyph->bitmap.buffer;

            if (dstRange.Width != width || dstRange.Height != rows)
            {
                throw new ArgumentOutOfRangeException(nameof(dstRange),
                    "dstRange.Width != width || dstRange.Height != rows");
            }

            int pointer = 0;

            for (int yIndex = 0; yIndex < rows; yIndex++)
            {
                for (int xIndex = 0; xIndex < width; xIndex += 4)
                {
                    byte r = buf[pointer + xIndex];
                    byte g = buf[pointer + xIndex + 1];
                    byte b = buf[pointer + xIndex + 2];
                    byte a = buf[pointer + xIndex + 3];

                    buffer[(yIndex * width) + xIndex] = new Color(r, g, b, a).ToAGRB();
                }
                pointer += pitch;
            }
        }
    }

    public void WriteTo(Texture2D dst, Rectangle dstRange)
    {
        unsafe
        {
            uint[]? buffer = null;
            try
            {
                buffer = ArrayPool<uint>.Shared.Rent(dstRange.Width * dstRange.Height);

                if (Face->glyph->bitmap.pixel_mode == FT_Pixel_Mode_.FT_PIXEL_MODE_GRAY)
                {
                    WriteGray(dstRange, buffer);
                }
                else if (Face->glyph->bitmap.pixel_mode == FT_Pixel_Mode_.FT_PIXEL_MODE_BGRA)
                {
                    WriteRGBA(dstRange, buffer);
                }
                else
                {
                    throw new FreeTypeException(FT_Error.FT_Err_Ok, $"no supported pixel mode:{Face->glyph->bitmap.pixel_mode}");
                }

                dst.SetData(0, dstRange, buffer, 0, buffer.Length);
            }
            finally
            {
                if (buffer is not null)
                {
                    ArrayPool<uint>.Shared.Return(buffer);
                }
            }
        }
    }
}
