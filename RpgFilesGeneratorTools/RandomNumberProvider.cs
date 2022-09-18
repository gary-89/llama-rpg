using System;

namespace RpgFilesGeneratorTools;

internal class RandomNumberProvider
{
    private static readonly Random Random = new(65);

    public int GetRandomNumber()
    {
        return Random.Next(0, 100);
    }
}