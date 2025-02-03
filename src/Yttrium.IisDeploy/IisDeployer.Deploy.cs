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
            if ( config.Source.ContainsKey( s.Name ) == false )
                throw new InvalidOperationException( $"No source for website '{s.Name}'" );

            var from = config.Source[ s.Name ];
            var to = s.PhysicalPath;

            await FromTo( "Site", s.Name, from, to );


            /*
             * TODO: Doesn't work with nested
             */
            if ( s.Applications != null )
            {
                foreach ( var a in s.Applications )
                {
                    var name = s.Name + a.Path;
                    var f = config.Source[ s.Name ];
                    var t = a.PhysicalPath;

                    await FromTo( "App", name, f, t );
                }
            }

            if ( s.VirtualDirectories != null )
            {
                foreach ( var v in s.VirtualDirectories )
                {
                    var name = s.Name + v.Path;
                    var f = config.Source[ s.Name ];
                    var t = v.PhysicalPath;

                    await FromTo( "Vdir", name, f, t );
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
