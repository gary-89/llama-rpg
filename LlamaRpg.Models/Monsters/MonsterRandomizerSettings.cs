namespace LlamaRpg.Models.Monsters;

public sealed record MonsterRandomizerSettings(MonsterAreaType? MonsterAreaType, Range MonsterLevel, int NumberOfMonstersToGenerate = 100);
