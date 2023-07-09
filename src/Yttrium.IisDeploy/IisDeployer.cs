using CliWrap;
using Microsoft.Extensions.Logging;
using Microsoft.Web.Administration;

namespace Yttrium.IisDeploy;

/// <summary />
public class IisDeployer : IIisDeployer
{
    private readonly ILogger<IisDeployer> _logger;


    /// <summary />
    public IisDeployer( ILogger<IisDeployer> logger )
    {
        _logger = logger;
    }


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
                if ( config.Source.ContainsKey( vd.Name ) == false )
                    throw new InvalidOperationException( $"No source for vdir '{vd.Name}'" );

                var from = config.Source[ vd.Name ];
                var to = vd.PhysicalPath;

                if ( Directory.Exists( from ) == false )
                    throw new InvalidOperationException( $"Source directory '{from}' does not exist" );

                _logger.LogInformation( "--- {Vdir} ----------------------------------------------------------", vd.Name );
                _logger.LogInformation( "Mirror {Vdir}: {From} >> {To}", vd.Name, from, to );

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
    }


    /// <summary />
    public Task Configure( IisDefinition definition )
    {
        /*
         * 
         */
        var mgr = new ServerManager();


        /*
         * Add pools
         */
        foreach ( var pd in definition.Pools )
        {
            var p = mgr.ApplicationPools.SingleOrDefault( x => x.Name == pd.Name );

            if ( p == null )
                p = mgr.ApplicationPools.Add( pd.Name );


            /*
             * 
             */
            p.AutoStart = pd.AutoStart;


            /*
             * ProcessModel
             */
            p.ProcessModel.IdentityType = pd.ProcessModel.IdentityType;

            if ( pd.ProcessModel.IdentityType == ProcessModelIdentityType.SpecificUser )
            {
                p.ProcessModel.UserName = pd.ProcessModel.UserName;
                p.ProcessModel.Password = pd.ProcessModel.Password;
            }
            else
            {
                p.ProcessModel.UserName = null;
                p.ProcessModel.Password = null;
            }
        }


        /*
         * Add sites
         */
        foreach ( var sd in definition.Sites )
        {
            var s = mgr.Sites.SingleOrDefault( x => x.Name == sd.Name );

            if ( s == null )
                s = mgr.Sites.Add( sd.Name, sd.PhysicalPath, sd.Bindings.First().Port );


            /*
             * Path
             */
            var app = s.Applications.Single();
            app.Path = sd.PhysicalPath;
            app.ApplicationPoolName = sd.ApplicationPoolName;


            /*
             * Check binding
             * TODO: Rewrite without .Clear'ing
             */
            s.Bindings.Clear();

            foreach ( var b in sd.Bindings )
                b.AddTo( s.Bindings );

            s.ServerAutoStart = true;
        }


        /*
         * Remove unmanaged sites?
         */
        var usite = new Dictionary<string, Site>();

        foreach ( var site in mgr.Sites )
        {
            var attr = site.GetAttribute( "x_auto" );

            if ( attr == null )
            {
                usite.Add( site.Name, site );
                continue;
            }
        }

        if ( usite.Count > 0 && true )
        {
            if ( mgr.Sites.AllowsRemove == false )
            {
                _logger.LogError( "Removal of websites is not permitted" );
            }
            else
            {
                foreach ( var s in usite )
                {
                    _logger.LogWarning( "Removing website {SiteName}", s.Key );
                    mgr.Sites.Remove( s.Value );
                }
            }
        }


        /*
         * Remove unmanaged pools?
         */
        var upool = new Dictionary<string, ApplicationPool>();

        foreach ( var pool in mgr.ApplicationPools )
        {
            if ( pool.Name == ".NET v4.5" )
                continue;

            if ( pool.Name == ".NET v4.5 Classic" )
                continue;

            if ( pool.Name == "DefaultAppPool" )
                continue;

            var attr = pool.GetAttribute( "x_auto" );

            if ( attr == null )
            {
                upool.Add( pool.Name, pool );
                continue;
            }
        }

        if ( upool.Count > 0 && true )
        {
            if ( mgr.ApplicationPools.AllowsRemove == false )
            {
                _logger.LogError( "Removal of application pools is not permitted" );
            }
            else
            {
                foreach ( var s in upool )
                {
                    _logger.LogWarning( "Removing application pool {SiteName}", s.Key );
                    mgr.ApplicationPools.Remove( s.Value );
                }
            }
        }


        /*
         * 
         */
        mgr.CommitChanges();


        return Task.CompletedTask;
    }

    public Task Configure( object config )
    {
        throw new NotImplementedException();
    }
}
