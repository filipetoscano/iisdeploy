namespace Yttrium.IisDeploy;

public partial class IisDeployer
{
    /// <inheritdoc />
    public Task<DeploymentState> State( DeploymentDefinition defn )
    {
        var state = LoadState( defn );

        return Task.FromResult( state );
    }
}