namespace Yttrium.IisDeploy;

/// <summary />
public interface IIisDeployer
{
    /// <summary>
    /// Deploys the source files to the target locations.
    /// </summary>
    /// <param name="definition">
    /// Definition of IIS websites and virtual directories.
    /// </param>
    /// <param name="config">
    /// Deployment configuration settings.
    /// </param>
    Task Deploy( IisDefinition definition, DeploymentConfig config );


    /// <summary />
    Task<IisColor> ColorGet();


    /// <summary />
    Task ColorSet( IisColor color );


    /// <summary>
    /// Gets the current IIS definition.
    /// </summary>
    /// <returns>
    /// Entire IIS definition.
    /// </returns>
    Task<IisDefinition> Get();


    /// <summary>
    /// Configures IIS, upserting sites and virtual directories.
    /// </summary>
    /// <param name="definition">
    /// Definition of IIS websites and virtual directories.
    /// </param>
    Task Configure( IisDefinition definition );
}
