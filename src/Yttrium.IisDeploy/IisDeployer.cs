using Microsoft.Extensions.Logging;

namespace Yttrium.IisDeploy;

/// <summary />
public partial class IisDeployer : IIisDeployer
{
    private readonly ILogger<IisDeployer> _logger;


    /// <summary />
    public IisDeployer( ILogger<IisDeployer> logger )
    {
        _logger = logger;
    }


    /// <inheritdoc />
    public async Task Mutate( DeploymentDefinition definition, DeploymentColor color )
    {
        await Task.Yield();

        foreach ( var sd in definition.Sites )
        {
            foreach ( var ad in sd.Applications )
            {
                ad.PhysicalPath = Mix( ad.PhysicalPath, color );


                /*
                 * Virtual Directories
                 */
                if ( ad.VirtualDirectories == null )
                    continue;

                foreach ( var vd in ad.VirtualDirectories )
                    vd.PhysicalPath = Mix( vd.PhysicalPath, color );
            }
        }
    }


    /// <summary />
    private string Mix( string path, DeploymentColor color )
    {
        return Path.Combine( path, $"dc-{color}" );
    }
}