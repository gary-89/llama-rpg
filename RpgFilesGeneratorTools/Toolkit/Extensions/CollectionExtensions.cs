using System.Collections.Generic;

namespace RpgFilesGeneratorTools.Toolkit.Extensions;

internal static class CollectionExtensions
{
    public static void AddEach<T>(this ICollection<T> collection, IEnumerable<T> itemsToAdd)
    {
        foreach (var item in itemsToAdd)
        {
            collection.Add(item);
        }
    }
}
