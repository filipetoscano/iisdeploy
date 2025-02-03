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
    Task Deploy( DeploymentDefinition definition, DeploymentConfig config );


    /// <summary />
    Task<DeploymentColor> ColorGet();


    /// <summary />
    Task ColorSet( DeploymentColor color );


    /// <summary>
    /// Gets the current IIS definition.
    /// </summary>
    /// <returns>
    /// Entire IIS definition.
    /// </returns>
    Task<DeploymentDefinition> Get();


    /// <summary>
    /// Configures IIS, upserting sites and virtual directories.
    /// </summary>
    /// <param name="definition">
    /// Definition of IIS websites and virtual directories.
    /// </param>
    Task Configure( DeploymentDefinition definition );
}