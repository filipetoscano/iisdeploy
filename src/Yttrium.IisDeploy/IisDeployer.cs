using Microsoft.Extensions.Logging;
using System.Text.Json;

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


    /// <summary />
    private DeploymentState LoadState( DeploymentDefinition defn )
    {
        if ( defn.Name == null )
            throw new ApplicationException( $"$.Name is required" );

        if ( defn.RootPhysicalPath == null )
            throw new ApplicationException( $"$.RootPhysicalPath is required" );


        /*
         * 
         */
        var fname = Path.Combine( defn.RootPhysicalPath, $"d-{defn.Name}.json" );

        if ( File.Exists( fname ) == false )
            return new DeploymentState();


        /*
         * 
         */
        var json = File.ReadAllText( fname );

        return JsonSerializer.Deserialize<DeploymentState>( json )!;
    }


    /// <summary />
    private void SaveState( DeploymentDefinition defn, DeploymentState state )
    {
        if ( defn.Name == null )
            throw new ApplicationException( $"$.Name is required" );

        if ( defn.RootPhysicalPath == null )
            throw new ApplicationException( $"$.RootPhysicalPath is required" );


        /*
         * 
         */
        var jso = new JsonSerializerOptions() { WriteIndented = true };
        var json = JsonSerializer.Serialize( state, jso );

        var fname = Path.Combine( defn.RootPhysicalPath, $"d-{defn.Name}.json" );

        File.WriteAllText( fname, json );
    }


    /// <summary />
    private void NormalizeDefinition( DeploymentDefinition defn )
    {
        /*
         * 
         */
        if ( defn.Sites == null )
            defn.Sites = new List<SiteDefinition>();

        if ( defn.ApplicationPools == null )
            defn.ApplicationPools = new List<ApplicationPoolDefinition>();


        /*
         * 
         */
        foreach ( var site in defn.Sites )
        {
            if ( site.Applications == null )
                site.Applications = new List<ApplicationDefinition>();

            if ( site.Applications.Count( x => x.Path == "/" ) != 1 )
                throw new ApplicationException( $"Site {site.Name} must have only one app with path /" );

            foreach ( var app in site.Applications )
            {
                if ( defn.RootPhysicalPath != null )
                    app.PhysicalPath = Path.Combine( defn.RootPhysicalPath, app.PhysicalPath );

                if ( app.ApplicationPool != null && defn.ApplicationPools.Any( x => x.Name == app.ApplicationPool.Name ) == false )
                {
                    defn.ApplicationPools.Add( app.ApplicationPool );

                    app.ApplicationPoolName = app.ApplicationPool.Name;
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
    }


    /// <summary />
    private void NormalizeMap( DeploymentMap map )
    {
        if ( map.Source == null )
            map.Source = new Dictionary<string, string>();

        if ( map.RootSource != null )
        {
            foreach ( var key in map.Source.Keys.ToArray() )
            {
                map.Source[ key ] = Path.Combine( map.RootSource, map.Source[ key ] );
            }
        }
    }


    /// <summary />
    private void MutateDefinition( DeploymentDefinition definition, DeploymentColor color )
    {
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