using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moe.TextEngine;

public class HarfbuzzShapeEngine
{
    public static readonly Lazy<string> EngineName = new(() =>
    {
        return $"harfbuzz";
    },
        true);

    public string Name { get; } = EngineName.Value;

    public HarfbuzzShape Create(Font font)
    {
        return new HarfbuzzShape(
            this,
                font
            );
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
