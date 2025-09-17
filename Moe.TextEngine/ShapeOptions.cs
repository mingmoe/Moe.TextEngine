using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moe.TextEngine;

public sealed class ShapeOptions
{
    public TextDirection Direction { get; set; } = TextDirection.LeftToRight;

    /// <summary>
    /// follow ISO 15924,
    /// e.g. from hb_script_from_iso15924_tag 
    /// </summary>
    public string Script { get; set; } = "Ital";

    /// <summary>
    /// follow BCP 47,
    /// e.g. from hb_language_from_string() 
    /// </summary>
    public string Region { get; set; } = "en";

    /// <summary>
    ///  follow ISO 639
    /// </summary>
    public string Language { get; set; } = "en";
}
