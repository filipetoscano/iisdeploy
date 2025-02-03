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
    public Task Normalize( DeploymentDefinition defn )
    {
        foreach ( var site in defn.Sites )
        {
            foreach ( var app in site.Applications )
            {
                if ( app.ApplicationPool != null && defn.ApplicationPools.Any( x => x.Name == app.ApplicationPool.Name ) == false )
                {
                    defn.ApplicationPools.Add( app.ApplicationPool );
                    app.ApplicationPool = null;
                }

                if ( app.VirtualDirectories == null )
                    continue;

                foreach ( var vdir in app.VirtualDirectories )
                {
                    if ( defn.RootPhysicalPath != null )
                        vdir.PhysicalPath = Path.Combine( defn.RootPhysicalPath, vdir.PhysicalPath );
                }
            }
        }

        return Task.CompletedTask;
    }


    /// <inheritdoc />
    public Task Normalize( DeploymentMap map )
    {
        if ( map.RootSource != null )
        {
            foreach ( var key in map.Source.Keys.ToArray() )
            {
                map.Source[ key ] = Path.Combine( map.RootSource, map.Source[ key ] );
            }
        }

        return Task.CompletedTask;
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