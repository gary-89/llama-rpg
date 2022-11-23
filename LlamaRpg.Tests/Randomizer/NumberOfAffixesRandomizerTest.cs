using LlamaRpg.Services.Randomization;

namespace LlamaRpg.Tests.Randomizer;

public class NumberOfAffixesRandomizerTest
{
    private const bool OutputTests = false;

    [Theory]
    [Repeat(1000, 1, 1, 1)]
    public void GenerateNumberOfAffixesForMagicItem_WorksCorrectly(int maxPrefixes, int maxSuffixes, int min)
    {
        // Arrange
        var generator = new NumberOfAffixesGenerator();

        // Act
        var (numberOfPrefixes, numberOfSuffixes) =
            generator.Generate(new Models.Range(0, maxPrefixes), new Models.Range(0, maxSuffixes), min);

        // Assert
        Assert.True(numberOfPrefixes + numberOfSuffixes <= maxPrefixes + maxSuffixes);
        Assert.True(numberOfPrefixes + numberOfSuffixes >= min);
        Assert.True(numberOfPrefixes >= 0);
        Assert.True(numberOfPrefixes <= maxPrefixes);
        Assert.True(numberOfSuffixes >= 0);
        Assert.True(numberOfSuffixes <= maxSuffixes);

        if (OutputTests)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var output = Path.Combine(path, "affixes-magic-items.txt");
            File.AppendAllText(
                output,
                $"MAGIC Prefixes: {numberOfPrefixes}, Suffixes:{numberOfSuffixes}, Total:{numberOfPrefixes + numberOfSuffixes}{Environment.NewLine}");
        }
    }

    [Theory]
    [Repeat(1000, 2, 2, 3)]
    [Repeat(1000, 2, 5, 3)]
    public void GenerateNumberOfAffixesForRareItem_WorksCorrectly(int maxPrefixes, int maxSuffixes, int min)
    {
        // Arrange
        var generator = new NumberOfAffixesGenerator();

        // Act
        var (numberOfPrefixes, numberOfSuffixes) = generator.Generate(new Models.Range(1, maxPrefixes), new Models.Range(1, maxSuffixes), min);

        // Assert
        Assert.True(numberOfPrefixes + numberOfSuffixes <= maxPrefixes + maxSuffixes);
        Assert.True(numberOfPrefixes + numberOfSuffixes >= min);
        Assert.True(numberOfPrefixes >= 1);
        Assert.True(numberOfPrefixes <= maxPrefixes);
        Assert.True(numberOfSuffixes >= 1);
        Assert.True(numberOfSuffixes <= maxSuffixes);

        if (OutputTests)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var output = Path.Combine(path, "affixes-rare-items.txt");
            File.AppendAllText(
                output,
                $"RARE Prefixes: {numberOfPrefixes}, Suffixes:{numberOfSuffixes}, Total:{numberOfPrefixes + numberOfSuffixes}{Environment.NewLine}");
        }
    }

    [Theory]
    [Repeat(1000, 2, 2, 3)]
    public void GenerateNumberOfAffixesForEliteItem_WorksCorrectly(int maxPrefixes, int maxSuffixes, int min)
    {
        // Arrange
        var generator = new NumberOfAffixesGenerator();

        // Act
        var (numberOfPrefixes, numberOfSuffixes) = generator.GenerateForEliteItems(new Models.Range(min, maxPrefixes + maxSuffixes));

        // Assert
        Assert.True(numberOfPrefixes + numberOfSuffixes <= maxPrefixes + maxSuffixes);
        Assert.True(numberOfPrefixes + numberOfSuffixes >= min);

        Assert.True(numberOfPrefixes >= 0);
        Assert.True(numberOfPrefixes <= maxPrefixes + maxSuffixes);
        Assert.True(numberOfSuffixes >= 0);
        Assert.True(numberOfSuffixes <= maxPrefixes + maxSuffixes);

        if (OutputTests)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var output = Path.Combine(path, "affixes-elite-items.txt");
            File.AppendAllText(
                output,
                $"ELITE Prefixes: {numberOfPrefixes}, Suffixes:{numberOfSuffixes}, Total:{numberOfPrefixes + numberOfSuffixes}{Environment.NewLine}");
        }
    }
}
