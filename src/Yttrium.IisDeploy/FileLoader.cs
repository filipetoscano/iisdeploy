using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Yttrium.IisDeploy;

/// <summary />
public class FileLoader : IFileLoader
{
    private readonly ILogger<FileLoader> _logger;


    /// <summary />
    public FileLoader( ILogger<FileLoader> logger )
    {
        _logger = logger;
    }


    /// <inheritdoc />
    public DeploymentConfig LoadConfigJson( Stream stream )
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
    public IisDefinition LoadDefinitionJson( Stream stream )
    {
        var obj = JsonSerializer.Deserialize<IisDefinition>( stream );

        if ( obj == null )
            throw new InvalidOperationException( "Invalid configuration file, yielded null object." );


        /*
         * 
         */
        foreach ( var s in obj.Sites )
            LoadWalk( obj, s );

        return obj;
    }


    /// <summary />
    private static void LoadWalk( IisDefinition defn, ApplicationDefinition app )
    {
        if ( app.ApplicationPool != null )
        {
            if ( defn.ApplicationPools.Exists( x => x.Name == app.ApplicationPool.Name ) == false )
            {
                defn.ApplicationPools.Add( app.ApplicationPool );
            }
        }

        app.PhysicalPath = Path.Combine( defn.RootPhysicalPath, app.PhysicalPath );


        /*
         * 
         */
        if ( app.Applications != null )
        {
            foreach ( var sapp in app.Applications )
            {
                LoadWalk( defn, sapp );
            }
        }


        /*
         * 
         */
        if ( app.VirtualDirectories != null )
        {
            foreach ( var vdir in app.VirtualDirectories )
            {
                vdir.PhysicalPath = Path.Combine( defn.RootPhysicalPath, vdir.PhysicalPath );
            }
        }
    }


    /// <inheritdoc />
    public DeploymentConfig LoadConfigXml( Stream stream )
    {
        throw new NotImplementedException();
    }


    /// <inheritdoc />
    public IisDefinition LoadDefinitionXml( Stream stream )
    {
        throw new NotImplementedException();
    }
}
