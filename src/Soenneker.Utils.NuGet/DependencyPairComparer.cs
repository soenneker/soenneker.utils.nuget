using System;
using System.Collections.Generic;

namespace Soenneker.Utils.NuGet;

internal sealed class DependencyPairComparer : IEqualityComparer<KeyValuePair<string, string>>
{
    internal static readonly DependencyPairComparer Instance = new();

    public bool Equals(KeyValuePair<string, string> x, KeyValuePair<string, string> y) =>
        StringComparer.OrdinalIgnoreCase.Equals(x.Key, y.Key) && StringComparer.OrdinalIgnoreCase.Equals(x.Value, y.Value);

    public int GetHashCode(KeyValuePair<string, string> obj)
    {
        unchecked
        {
            int h = StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Key);
            h = (h * 397) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Value);
            return h;
        }
    }
}
