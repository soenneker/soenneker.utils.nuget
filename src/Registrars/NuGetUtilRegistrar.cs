using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.NuGet.Client.Registrars;
using Soenneker.Utils.NuGet.Abstract;

namespace Soenneker.Utils.NuGet.Registrars;

/// <summary>
/// A utility library for various NuGet related operations
/// </summary>
public static class NuGetUtilRegistrar
{
    /// <summary>
    /// Adds <see cref="INuGetUtil"/> as a singleton service. <para/>
    /// </summary>
    public static IServiceCollection AddNuGetUtilAsSingleton(this IServiceCollection services)
    {
        services.TryAddSingleton<INuGetUtil, NuGetUtil>();
        services.AddNuGetClientAsSingleton();
        return services;
    }

    /// <summary>
    /// Adds <see cref="INuGetUtil"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddNuGetUtilAsScoped(this IServiceCollection services)
    {
        services.TryAddScoped<INuGetUtil, NuGetUtil>();
        services.AddNuGetClientAsSingleton();
        return services;
    }
}