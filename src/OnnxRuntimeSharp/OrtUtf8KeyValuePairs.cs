using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;

namespace OnnxRuntimeSharp;

unsafe ref struct OrtUtf8KeyValuePairs
{
    readonly sbyte** _keys;
    readonly sbyte** _values;
    int _count;

    public OrtUtf8KeyValuePairs(
        IReadOnlyDictionary<string, string>? options,
        sbyte** keys,
        sbyte** values,
        int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(capacity);
        _keys = keys;
        _values = values;
        _count = 0;
        try
        {
            if (options is not null)
            {
                foreach (var option in options)
                {
                    ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(_count, capacity, nameof(capacity));
                    ArgumentNullException.ThrowIfNull(option.Key);
                    _keys[_count] = (sbyte*)Utf8StringMarshaller.ConvertToUnmanaged(option.Key);
                    _values[_count] = null;
                    // Track the key before converting its value so partial pairs are freed on failure.
                    ++_count;
                    ArgumentNullException.ThrowIfNull(option.Value);
                    _values[_count - 1] = (sbyte*)Utf8StringMarshaller.ConvertToUnmanaged(option.Value);
                }
            }
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    public sbyte** Keys => _keys;
    public sbyte** Values => _values;
    public nuint Count => (nuint)_count;

    public void Dispose()
    {
        while (_count > 0)
        {
            var index = --_count;
            Utf8StringMarshaller.Free((byte*)_values[index]);
            Utf8StringMarshaller.Free((byte*)_keys[index]);
            _values[index] = null;
            _keys[index] = null;
        }
    }
}
