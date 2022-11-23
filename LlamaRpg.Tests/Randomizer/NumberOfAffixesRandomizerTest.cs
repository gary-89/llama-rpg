using System.Diagnostics.CodeAnalysis;
using LlamaRpg.Services.Randomization;

namespace LlamaRpg.Tests.Randomizer;

[SuppressMessage("Usage", "xUnit1006:Theory methods should have parameters", Justification = "Repeat does not provide any parameters.")]
public class NumberOfAffixesRandomizerTest
{
    private const bool OutputTests = false;

    [Theory]
    [Repeat(1000)]
    public void GenerateNumberOfAffixesForMagicItem_WorksCorrectly()
    {
        // Arrange
        var generator = new NumberOfAffixesGenerator();

        // Act
        var (numberOfPrefixes, numberOfSuffixes) =
            generator.Generate(new Models.Range(0, 1), new Models.Range(0, 1), 1);

        // Assert
        Assert.True(numberOfPrefixes + numberOfSuffixes <= 2);
        Assert.True(numberOfPrefixes + numberOfSuffixes >= 1);
        Assert.True(numberOfPrefixes >= 0);
        Assert.True(numberOfPrefixes <= 1);
        Assert.True(numberOfSuffixes >= 0);
        Assert.True(numberOfSuffixes <= 1);

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
    [Repeat(1000)]
    public void GenerateNumberOfAffixesForRareItem_WorksCorrectly()
    {
        // Arrange
        var generator = new NumberOfAffixesGenerator();

        // Act
        var (numberOfPrefixes, numberOfSuffixes) = generator.Generate(new Models.Range(1, 2), new Models.Range(1, 2), 3);

        // Assert
        Assert.True(numberOfPrefixes + numberOfSuffixes <= 4);
        Assert.True(numberOfPrefixes + numberOfSuffixes >= 3);
        Assert.True(numberOfPrefixes >= 1);
        Assert.True(numberOfPrefixes <= 2);
        Assert.True(numberOfSuffixes >= 1);
        Assert.True(numberOfSuffixes <= 2);

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
    [Repeat(1000)]
    public void GenerateNumberOfAffixesForEliteItem_WorksCorrectly()
    {
        // Arrange
        var generator = new NumberOfAffixesGenerator();

        // Act
        var (numberOfPrefixes, numberOfSuffixes) = generator.GenerateForEliteItems(new Models.Range(3, 4));

        // Assert
        Assert.True(numberOfPrefixes + numberOfSuffixes <= 4);
        Assert.True(numberOfPrefixes + numberOfSuffixes >= 3);
        Assert.True(numberOfPrefixes >= 0);
        Assert.True(numberOfPrefixes <= 4);
        Assert.True(numberOfSuffixes >= 0);
        Assert.True(numberOfSuffixes <= 4);

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
