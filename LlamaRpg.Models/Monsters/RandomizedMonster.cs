using LlamaRpg.Models.Items;

namespace LlamaRpg.Models.Monsters;

public sealed record RandomizedMonster(
    int Index,
    string Name,
    string Type,
    UniqueMonsterType UniqueMonsterType,
    MonsterAreaType MonsterArea,
    int Level,
    IList<RandomizedItem> Items);
