using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Yttrium.IisDeploy;

/// <summary />
public class JsonFileLoader : IFileLoader
{
    private readonly ILogger<JsonFileLoader> _logger;


    /// <summary />
    public JsonFileLoader( ILogger<JsonFileLoader> logger )
    {
        _logger = logger;
    }


    /// <inheritdoc />
    public DeploymentConfig LoadMap( Stream stream )
    {
        var obj = JsonSerializer.Deserialize<DeploymentConfig>( stream );

        if ( obj == null )
            throw new InvalidOperationException( "Invalid configuration file, yielded null object." );


        /*
         * Resolve paths
         */
        if ( obj.RootSource != null )
        {
            foreach ( var key in obj.Source.Keys.ToArray() )
            {
                obj.Source[ key ] = Path.Combine( obj.RootSource, obj.Source[ key ] );
            }
        }

        return obj;
    }


    /// <inheritdoc />
    public DeploymentDefinition LoadDefinition( Stream stream )
    {
        var obj = JsonSerializer.Deserialize<DeploymentDefinition>( stream );

        if ( obj == null )
            throw new InvalidOperationException( "Invalid configuration file, yielded null object." );

        /*
         * 
         */
        foreach ( var s in obj.Sites )
        {
            foreach ( var app in s.Applications )
                LoadWalk( obj, app );
        }

        return obj;
    }


    /// <summary />
    private static void LoadWalk( DeploymentDefinition defn, ApplicationDefinition app )
    {
        if ( app.ApplicationPool != null )
        {
            if ( defn.ApplicationPools.Exists( x => x.Name == app.ApplicationPool.Name ) == false )
            {
                defn.ApplicationPools.Add( app.ApplicationPool );
            }
        }

        app.PhysicalPath = Path.Combine( defn.RootPhysicalPath ?? ".", app.PhysicalPath );

        if ( app.VirtualDirectories != null )
        {
            foreach ( var vdir in app.VirtualDirectories )
            {
                vdir.PhysicalPath = Path.Combine( defn.RootPhysicalPath ?? ".", vdir.PhysicalPath );
            }
        }
    }
}