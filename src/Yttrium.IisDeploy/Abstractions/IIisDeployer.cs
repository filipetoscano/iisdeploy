namespace Yttrium.IisDeploy;

/// <summary />
public interface IIisDeployer
{
    /// <summary />
    Task<DeploymentState> State( DeploymentDefinition defn );


    /// <summary>
    /// Deploys the source files to the target locations.
    /// </summary>
    /// <param name="definition">
    /// Definition of IIS websites and virtual directories.
    /// </param>
    /// <param name="map">
    /// Deployment map.
    /// </param>
    /// <param name="options">
    /// Deployment options.
    /// </param>
    Task<DeploymentState> Deploy( DeploymentDefinition definition, DeploymentMap map, DeployOptions options );


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
    /// <param name="options">
    /// Apply options.
    /// </param>
    Task<DeploymentState> Apply( DeploymentDefinition definition, ApplyOptions options );
}