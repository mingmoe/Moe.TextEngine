using Moe.ResourcesManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Moe.TextEngine;

public class FileFontSource : IFontSource
{
    private readonly nint _buffer;

    private readonly int _length;

    private bool _disposed = false;

    public ResourceID ResourceID { get; init; }

    public FileFontSource(string file)
    {
        ResourceID = new ResourceID(file);
        FileInfo fileInfo = new FileInfo(file);
        checked
        {
            _length = (int)fileInfo.Length;
            _buffer = Marshal.AllocHGlobal(_length);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    ~FileFontSource()
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

        Marshal.FreeHGlobal(_buffer);
    }

    public int GetDataSize()
    {
        return _length;
    }

    public nint GetRawData()
    {
        return _buffer;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }
        if (obj is IFontSource other)
        {
            return ((IEquatable<IFontSource>)this).Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return ResourceID.GetHashCode();
    }
}
