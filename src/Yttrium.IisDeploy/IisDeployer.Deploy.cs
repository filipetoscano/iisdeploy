using CliWrap;
using Microsoft.Extensions.Logging;
namespace Yttrium.IisDeploy;

/// <summary />
public partial class IisDeployer : IIisDeployer
{
    private const string Robocopy = @"c:\windows\system32\robocopy.exe";


    /// <inheritdoc />
    public async Task Deploy( DeploymentDefinition definition, DeploymentConfig config )
    {
        /*
         * 
         */
        if ( File.Exists( Robocopy ) == false )
            throw new InvalidOperationException( "Robocopy not found" );


        /*
         * Websites
         */
        foreach ( var s in definition.Sites )
        {
            /*
             * 
             */
            if ( s.Applications == null )
                continue;

            foreach ( var app in s.Applications )
            {
                var appFrom = config.Source[ s.Name + app.Path ];
                var appTo = app.PhysicalPath;

                await FromTo( "App", app.Path, appFrom, appTo );


                /*
                 * 
                 */
                if ( app.VirtualDirectories == null )
                    continue;

                foreach ( var vdir in app.VirtualDirectories )
                {
                    var vdirFrom = config.Source[ s.Name + app.Path + vdir ];
                    var vdirTo = vdir.PhysicalPath;

                    await FromTo( "VirtualDirectory", app.Path + vdir, vdirFrom, vdirTo );
                }
            }
        }
    }


    /// <summary />
    private async Task FromTo( string type, string name, string from, string to )
    {
        if ( Directory.Exists( from ) == false )
            throw new InvalidOperationException( $"Source directory '{from}' does not exist" );

        _logger.LogInformation( "--- {Type}: {Name}", type, name );
        _logger.LogInformation( "Mirror {Name}: {From} >> {To}", name, from, to );

        await Cli.Wrap( Robocopy )
            .WithArguments( args => args
                .Add( "/mir" )
                .Add( from )
                .Add( to )
            )
            .WithStandardOutputPipe( PipeTarget.ToDelegate( s =>
            {
                _logger.LogDebug( s );
            } ) )
            .WithValidation( CommandResultValidation.None )
            .ExecuteAsync();
    }
}