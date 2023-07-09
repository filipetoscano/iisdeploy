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
    public DeploymentConfig LoadConfigXml( Stream stream )
    {
        throw new NotImplementedException();
    }


    /// <inheritdoc />
    public IisDefinition LoadDefinitionJson( Stream stream )
    {
        var obj = JsonSerializer.Deserialize<IisDefinition>( stream );

        if ( obj == null )
            throw new InvalidOperationException( "Invalid configuration file, yielded null object." );

        foreach ( var s in obj.Sites )
        {
            s.PhysicalPath = Path.Combine( obj.RootPhysicalPath, s.PhysicalPath );

            if ( s.VirtualDirectories == null )
                continue;

            foreach ( var vd in s.VirtualDirectories )
                vd.PhysicalPath = Path.Combine( obj.RootPhysicalPath, vd.PhysicalPath );
        }

        return obj;
    }


    /// <inheritdoc />
    public IisDefinition LoadDefinitionXml( Stream stream )
    {
        throw new NotImplementedException();
    }
}
