using Microsoft.Extensions.DependencyInjection;

namespace Ethereum.MetaMask.Blazor;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="IMetaMaskService"/> to services collection.
    /// </summary>
    public static IServiceCollection AddMetaMaskBlazor(this IServiceCollection services)
    {
        services.AddScoped<IMetaMaskService, MetaMaskService>();
        return services;
    }
}