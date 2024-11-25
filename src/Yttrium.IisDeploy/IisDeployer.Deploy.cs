using CliWrap;
using Microsoft.Extensions.Logging;

namespace Yttrium.IisDeploy;

/// <summary />
public partial class IisDeployer : IIisDeployer
{
    /// <inheritdoc />
    public async Task Deploy( IisDefinition definition, DeploymentConfig config )
    {
        /*
         * 
         */
        const string Robocopy = @"c:\windows\system32\robocopy.exe";

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

            if ( Directory.Exists( from ) == false )
                throw new InvalidOperationException( $"Source directory '{from}' does not exist" );

            _logger.LogInformation( "--- {Site} ----------------------------------------------------------", s.Name );
            _logger.LogInformation( "Mirror {Site}: {From} >> {To}", s.Name, from, to );

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


        /*
         * Virtual directories
         */
        foreach ( var s in definition.Sites )
        {
            if ( s.VirtualDirectories == null )
                continue;

            foreach ( var vd in s.VirtualDirectories )
            {
                //if ( config.Source.ContainsKey( vd.Name ) == false )
                //    throw new InvalidOperationException( $"No source for vdir '{vd.Name}'" );

                //var from = config.Source[ vd.Name ];
                //var to = vd.PhysicalPath;

                //if ( Directory.Exists( from ) == false )
                //    throw new InvalidOperationException( $"Source directory '{from}' does not exist" );

                //_logger.LogInformation( "--- {Vdir} ----------------------------------------------------------", vd.Name );
                //_logger.LogInformation( "Mirror {Vdir}: {From} >> {To}", vd.Name, from, to );

                //await Cli.Wrap( Robocopy )
                //    .WithArguments( args => args
                //        .Add( "/mir" )
                //        .Add( from )
                //        .Add( to )
                //    )
                //    .WithStandardOutputPipe( PipeTarget.ToDelegate( s =>
                //    {
                //        _logger.LogDebug( s );
                //    } ) )
                //    .WithValidation( CommandResultValidation.None )
                //    .ExecuteAsync();
            }
        }
    }
}
