namespace Yttrium.IisDeploy;

public partial class IisDeployer
{
    /// <inheritdoc />
    public async Task<DeploymentColor> ColorGet()
    {
        await Task.Yield();

        if ( File.Exists( "env.txt" ) == false )
            return DeploymentColor.Green;

        var text = File.ReadAllText( "env.txt" );

        DeploymentColor color = (DeploymentColor) Enum.Parse( typeof( DeploymentColor ), text );

        return color;
    }


    /// <inheritdoc />
    public async Task ColorSet( DeploymentColor color )
    {
        await Task.Yield();

        File.WriteAllText( "env.txt", color.ToString() );
    }
}