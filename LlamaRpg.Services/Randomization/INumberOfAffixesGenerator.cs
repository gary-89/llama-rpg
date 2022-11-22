namespace LlamaRpg.Services.Randomization;

public interface INumberOfAffixesGenerator
{
    (int NumberOfPrefixes, int NumberOfAffixes) Generate(Models.Range numberOfPrefixes, Models.Range numberOfSuffixes, int mandatoryAffixes);
}
