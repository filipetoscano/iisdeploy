using Microsoft.Extensions.Logging;
using Microsoft.Web.Administration;

namespace Yttrium.IisDeploy;

/// <summary />
public partial class IisDeployer : IIisDeployer
{
    private readonly ILogger<IisDeployer> _logger;


    /// <summary />
    public IisDeployer( ILogger<IisDeployer> logger )
    {
        _logger = logger;
    }


    /// <summary />
    public Task Configure( IisDefinition definition )
    {
        /*
         * 
         */
        var mgr = new ServerManager();


        /*
         * #1. Ensure pools
         */
        _logger.LogDebug( "Ensure application pools" );

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

        mgr.CommitChanges();


        /*
         * #2. Ensure websites
         * A website always has an application running in the root, which
         * always corresponds to a virtual directory!
         */
        _logger.LogDebug( "Ensure web sites / root apps" );

        foreach ( var sd in definition.Sites )
        {
            var s = mgr.Sites.SingleOrDefault( x => x.Name == sd.Name );

            if ( s == null )
                s = mgr.Sites.Add( sd.Name, sd.PhysicalPath, sd.Bindings.First().Port );


            /*
             * Check binding
             * TODO: Rewrite without .Clear'ing
             */
            s.Bindings.Clear();

            foreach ( var b in sd.Bindings )
                b.AddTo( s.Bindings );

            s.ServerAutoStart = true;


            /*
             * Root application
             */
            var app = s.Applications[ "/" ];
            app.VirtualDirectories[ "/" ].PhysicalPath = sd.PhysicalPath;
            app.ApplicationPoolName = sd.ApplicationPoolName;
        }


        /*
         * #3. Ensure virtual directories
         */
        _logger.LogDebug( "Ensure vdir apps" );

        foreach ( var sd in definition.Sites )
        {
            if ( sd.VirtualDirectories == null )
                continue;

            var s = mgr.Sites.Single( x => x.Name == sd.Name );

            foreach ( var vdd in sd.VirtualDirectories )
            {
                var app = s.Applications.SingleOrDefault( x => x.Path == vdd.Path );

                if ( app == null )
                {
                    app = s.Applications.Add( vdd.Path, vdd.PhysicalPath );
                }

                app.VirtualDirectories[ "/" ].PhysicalPath = vdd.PhysicalPath;
                app.ApplicationPoolName = vdd.ApplicationPoolName;
            }
        }


        /*
         * #4. Remove unmanaged sites
         * (This will remove all other associated objects, such as vdirs)
         */
        var usite = new Dictionary<string, Site>();

        foreach ( var site in mgr.Sites )
        {
            if ( definition.Sites.Any( x => x.Name == site.Name ) == true )
                continue;

            usite.Add( site.Name, site );
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
         * Remove unmanaged vdir apps
         */


        /*
         * Remove unmanaged apps
         */


        /*
         * Remove unmanaged pools
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

            if ( definition.Pools.Any( x => x.Name == pool.Name ) == true )
                continue;

            upool.Add( pool.Name, pool );
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
                    _logger.LogWarning( "Removing application pool {PoolName}", s.Key );
                    mgr.ApplicationPools.Remove( s.Value );
                }
            }
        }


        /*
         * Save changes to IIS configuration
         */
        mgr.CommitChanges();


        /*
         * Recycle pools
         */
        foreach ( var p in mgr.ApplicationPools )
        {
            _logger.LogInformation( "Recycling {PoolName}", p.Name );
            p.Recycle();
        }


        /*
         * 
         */
        return Task.CompletedTask;
    }
}
