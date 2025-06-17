using FreeTypeSharp;
using static FreeTypeSharp.FT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moe.TextEngine;

public class FreeTypeEngine
{
    private bool _disposed = false;

    public string Name { get; init; }

    public readonly unsafe FT_LibraryRec_* Library;

    public FreeTypeEngine()
    {
        unsafe
        {
            FT_LibraryRec_* lib;
            var error = FT_Init_FreeType(&lib);
            Helper.AssertFTError(error);

            Library = lib;

            int major = 0;
            int minor = 0;
            int patch = 0;

            FT_Library_Version(lib, &major, &minor, &patch);

            Name = $"freetype version {major}.{minor}.{patch}";
        }
    }

    public FreeTypeFontRasterizer Create(Font font)
    {
        return new FreeTypeFontRasterizer(this, font);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Dispose(true);
    }

    ~FreeTypeEngine()
    {
        Dispose(false);
    }

    protected void Dispose(bool disposing)
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
            FT_Done_Library(Library);
        }
    }
}
