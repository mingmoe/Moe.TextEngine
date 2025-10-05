using Moe.ResourcesManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moe.TextEngine;

public interface IFontSource : IDisposable, IEquatable<IFontSource>
{
    ResourceID ResourceId { get; }

    nint GetRawData();

    int GetDataSize();

    bool IEquatable<IFontSource>.Equals(IFontSource? other)
    {
        if (other == null) return false;

        return other.ResourceId.Equals(ResourceId);
    }
}
