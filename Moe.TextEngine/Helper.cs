using FreeTypeSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Moe.TextEngine;

internal static class Helper
{

    public static void AssertFTError(FT_Error error)
    {
        if (error != FT_Error.FT_Err_Ok)
        {
            unsafe
            {
                var str = FT.FT_Error_String(error);

                var msg = Marshal.PtrToStringUTF8((nint)str);

                throw new FreeTypeException(error,
                    $"Get a freetype error(code {(long)error},{error}):{msg}");
            }
        }
    }
}
