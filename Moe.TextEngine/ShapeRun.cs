using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moe.TextEngine;

public class ShapeRun
{
    public FontOptions FontOptions { get; set; }

    /// <summary>
    /// null means let ShapeEngine guess the options.
    /// </summary>
    public ShapeOptions? ShapeOptions { get; set; }

    public ShapeRun()
    {
        FontOptions = new();
        ShapeOptions = new();
    }

    public ShapeRun(FontOptions fontOptions, ShapeOptions? options)
    {
        FontOptions = fontOptions;
        ShapeOptions = options;
    }

    public required string Text { get; set; }

    public int Offset { get; set; } = 0;

    /// <summary>
    /// null means reading to the end of the <see cref="Text"/>
    /// </summary>
    public int? Length { get; set; }

    public string UsedText => Text[0..(Length ?? Text.Length)];
}
