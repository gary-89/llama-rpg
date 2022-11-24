using LlamaRpg.Services.Randomization;
using LlamaRpg.Services.Readers;
using LlamaRpg.Services.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace LlamaRpg.Services;

public static class ServicesConfiguration
{
    public static IServiceCollection AddLlamaRpgServices(this IServiceCollection services)
    {
        services.AddSingleton<AppServicesConfig>();
        services.AddSingleton<IRandomizerAffixValidator, RandomizerAffixValidator>();
        services.AddSingleton<INumberOfAffixesGenerator, NumberOfAffixesGenerator>();
        services.AddSingleton<IAffixProvider, AffixProvider>();
        services.AddSingleton<IItemProvider, ItemProvider>();
        services.AddSingleton<IMonsterProvider, MonsterProvider>();
        services.AddSingleton<ItemValidator>();
        services.AddSingleton<IRandomizedAffixProvider, RandomizedAffixProvider>();
        services.AddSingleton<IRandomizedItemProvider, RandomizedItemProvider>();
        services.AddSingleton<IRandomizedMonsterProvider, RandomizedMonsterProvider>();
        return services;
    }
}
