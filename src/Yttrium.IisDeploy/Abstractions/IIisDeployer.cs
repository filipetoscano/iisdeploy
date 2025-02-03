using Yttrium.IisDeploy.Model;

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
    /// <param name="map">
    /// Deployment map.
    /// </param>
    Task Deploy( DeploymentDefinition definition, DeploymentMap map );


    /// <summary />
    Task<DeploymentColor> ColorGet( string deploymentName );


    /// <summary />
    Task Mutate( DeploymentDefinition definition, DeploymentColor color );


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
    /// Options when applying the definition.
    /// </param>
    Task Apply( DeploymentDefinition definition, DefinitionApplyOptions options );
}