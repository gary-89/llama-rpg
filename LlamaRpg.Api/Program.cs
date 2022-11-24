using LlamaRpg.Models.Items;
using LlamaRpg.Models.Randomizer;
using LlamaRpg.Services;
using LlamaRpg.Services.Randomization;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLlamaRpgServices();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.WriteIndented = true;
});

var app = builder.Build();

app.MapGet("/items/{monsterLevel:int}", async (int monsterLevel, IRandomizedItemProvider provider)
    => await GetRandomizedItemsAsync(monsterLevel, provider).ConfigureAwait(false));

app.MapGet("/items", async (IRandomizedItemProvider provider)
    => await GetRandomizedItemsAsync(10, provider).ConfigureAwait(false));

app.Run();

async Task<List<RandomizedItem>> GetRandomizedItemsAsync(int monsterLevel, IRandomizedItemProvider randomizedItemProvider)
{
    var items = new List<RandomizedItem>();

    var settings = new ItemRandomizerSettings(100, monsterLevel, DropRateSettings.Default(), NumberOfAffixesSettings.Default(), ItemTypeWeightDrop.Default());

    await foreach (var item in randomizedItemProvider.GenerateItemsAsync(settings, CancellationToken.None))
    {
        items.Add(item);
    }

    return items;
}
