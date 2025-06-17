using FreeTypeSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moe.TextEngine;
public class FreeTypeException : Exception
{
    public FT_Error Error { get; init; }

    public FreeTypeException(FT_Error error, string msg) : base(msg)
    {
        Error = error;
    }
}
