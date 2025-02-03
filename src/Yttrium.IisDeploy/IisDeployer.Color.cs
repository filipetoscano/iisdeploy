namespace Yttrium.IisDeploy;

public partial class IisDeployer
{
    /// <inheritdoc />
    public async Task<DeploymentColor> ColorGet( string deploymentName )
    {
        await Task.Yield();

        var filename = $"deploy-{deploymentName}.txt";

        if ( File.Exists( filename ) == false )
            return DeploymentColor.Green;

        var text = File.ReadAllText( "env.txt" );

        DeploymentColor color = (DeploymentColor) Enum.Parse( typeof( DeploymentColor ), text );

        return color;
    }
}