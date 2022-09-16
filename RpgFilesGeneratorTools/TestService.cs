using System;

namespace RpgFilesGeneratorTools
{
    internal class TestService
    {
        private static readonly Random Random = new(65);

        public int Test()
        {
            return Random.Next(0, 100);
        }
    }
}
