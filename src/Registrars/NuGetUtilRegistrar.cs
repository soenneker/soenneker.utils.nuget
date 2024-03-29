using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
    public static void AddNuGetUtilAsSingleton(this IServiceCollection services)
    {
        services.TryAddSingleton<INuGetUtil, NuGetUtil>();
    }

    /// <summary>
    /// Adds <see cref="INuGetUtil"/> as a scoped service. <para/>
    /// </summary>
    public static void AddNuGetUtilAsScoped(this IServiceCollection services)
    {
        services.TryAddScoped<INuGetUtil, NuGetUtil>();
    }
}
