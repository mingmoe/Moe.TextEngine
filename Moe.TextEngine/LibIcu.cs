using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moe.TextEngine
{
    public class LibIcu
    {
        private static readonly Lazy<object?> InitiateIcu = new(() =>
        {
            Icu.Wrapper.Init();
            return null;
        }, true);

        public LibIcu()
        {
            _ = InitiateIcu.Value;
        }

    }
}
