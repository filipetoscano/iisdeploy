using CliWrap;
using Microsoft.Extensions.Logging;

namespace Yttrium.IisDeploy;

/// <summary />
public partial class IisDeployer : IIisDeployer
{
    private const string Robocopy = @"c:\windows\system32\robocopy.exe";


    /// <inheritdoc />
    public async Task<DeploymentState> Deploy( DeploymentDefinition defn, DeploymentMap map )
    {
        /*
         * 
         */
        var state = LoadState( defn );

        NormalizeDefinition( defn );
        NormalizeMap( map );

        if ( defn.HasBlueGreen == true )
            MutateDefinition( defn, state.NextColor() );


        /*
         * 
         */
        if ( File.Exists( Robocopy ) == false )
            throw new InvalidOperationException( "Robocopy not found" );


        /*
         * Websites
         */
        foreach ( var s in defn.Sites )
        {
            /*
             * 
             */
            if ( s.Applications == null )
                continue;

            foreach ( var app in s.Applications )
            {
                var appKey = KeyFor( s.Name, app.Path );

                if ( map.Source.ContainsKey( appKey ) == false )
                {
                    _logger.LogError( "App {SiteName}{AppPath}: No source map with {Key}", s.Name, app.Path, appKey );
                    continue;
                }

                var appFrom = map.Source[ s.Name + app.Path ];
                var appTo = app.PhysicalPath;

                if ( Directory.Exists( appFrom ) == false )
                {
                    _logger.LogError( "Map {Key}: Folder {Folder} does not exist", appKey, appFrom );
                }
                else
                {
                    _logger.LogInformation( "App {SiteName} {AppPath}", s.Name, app.Path );
                    await FromTo( appFrom, appTo );
                }


                /*
                 * 
                 */
                if ( app.VirtualDirectories == null )
                    continue;

                foreach ( var vdir in app.VirtualDirectories )
                {
                    var vdirKey = KeyFor( s.Name, app.Path, vdir.Path );

                    if ( map.Source.ContainsKey( vdirKey ) == false )
                    {
                        _logger.LogError( "Vdir {SiteName}{AppPath}{VdirPath}: No source map with {Key}", s.Name, app.Path, vdir.Path, vdirKey );
                        continue;
                    }

                    var vdirFrom = map.Source[ vdirKey ];
                    var vdirTo = vdir.PhysicalPath;

                    if ( Directory.Exists( vdirFrom ) == false )
                    {
                        _logger.LogError( "Map {Key}: Folder {Folder} does not exist", vdirKey, vdirFrom );
                    }
                    else
                    {
                        _logger.LogInformation( "Vdir {SiteName} {AppPath} {VdirPath}", s.Name, app.Path, vdir.Path );
                        await FromTo( vdirFrom, vdirTo );
                    }
                }
            }
        }


        /*
         * 
         */
        return state;
    }


    /// <summary />
    private static string KeyFor( string site, string appPath )
    {
        return site + appPath;
    }


    /// <summary />
    private static string KeyFor( string site, string appPath, string vdirPath )
    {
        if ( appPath == "/" )
            return site + vdirPath;

        return site + appPath + vdirPath;
    }


    /// <summary />
    private async Task FromTo( string from, string to )
    {
        _logger.LogInformation( "Mirror {From} >> {To}", from, to );

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