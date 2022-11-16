namespace LlamaRpg.Models.Monsters;

public sealed record Monster(string Name, string Type, MonsterAreaType monsterArea, int Level);
