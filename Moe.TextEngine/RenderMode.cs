using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moe.TextEngine;
public enum RenderMode
{
    /// <summary>
    /// one bit
    /// </summary>
    Mono,
    /// <summary>
    /// 8 bits gray
    /// </summary>
    Gray,
    /// <summary>
    /// sdf
    /// </summary>
    Sdf,
    /// <summary>
    /// RGBA
    /// </summary>
    BGRA,
}
