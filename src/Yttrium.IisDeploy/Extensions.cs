using Microsoft.Extensions.DependencyInjection;

namespace Yttrium.IisDeploy;

/// <summary />
public static class Extensions
{
    /// <summary />
    public static IServiceCollection AddIisDeployer( this IServiceCollection services )
    {
        services.AddScoped<IisDeployer, IisDeployer>();

        return services;
    }
}
