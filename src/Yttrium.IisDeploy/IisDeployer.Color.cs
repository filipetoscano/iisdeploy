using Microsoft.Web.Administration;

namespace Yttrium.IisDeploy;

public partial class IisDeployer
{
    /// <inheritdoc />
    public async Task<IisColor> ColorGet()
    {
        await Task.Yield();

        var mgr = new ServerManager();

        string? meta = (string?) mgr.GetMetadata( "env:color" );

        if ( meta == null )
            return IisColor.Blue;

        var color = (IisColor) Enum.Parse( typeof( IisColor ), meta );

        return color;
    }


    /// <inheritdoc />
    public async Task ColorSet( IisColor color )
    {
        await Task.Yield();

        var mgr = new ServerManager();

        var value = color.ToString();

        mgr.SetMetadata( "env:color", value );
    }
}
