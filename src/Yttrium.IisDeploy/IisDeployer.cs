using Microsoft.Extensions.Logging;
using Microsoft.Web.Administration;
using System.Net;

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
    public Task Configure( DeploymentDefinition definition )
    {
        /*
         * 
         */
        var mgr = new ServerManager();


        /*
         * #1. Ensure pools
         */
        _logger.LogDebug( "Ensure application pools" );

        foreach ( var pd in definition.ApplicationPools )
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
                p.ProcessModel.UserName = "";
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
            {
                var appd = sd.Applications.Single( x => x.PhysicalPath == ApplicationDefinition.RootPath );

                _logger.LogInformation( "Site {SiteName}: Add", sd.Name );
                s = mgr.Sites.Add( sd.Name, appd.PhysicalPath, sd.Bindings.First().Port );
            }


            /*
             * Check binding
             * TODO: Rewrite without .Clear'ing
             */
            s.Bindings.Clear();

            foreach ( var b in sd.Bindings )
                b.AddTo( s.Bindings );

            s.ServerAutoStart = true;


            /*
             * Applications
             */
            foreach ( var ad in sd.Applications )
            {
                var app = s.Applications[ ad.Path ];

                if ( app == null )
                {
                    _logger.LogInformation( "App {SiteName}{AppPath}: Add", sd.Name, ad.Path );
                    app = s.Applications.Add( ad.Path, ad.PhysicalPath );
                }

                app.VirtualDirectories[ "/" ].PhysicalPath = ad.PhysicalPath;
                app.ApplicationPoolName = ad.ApplicationPoolName;


                /*
                 * Virtual directories
                 */
                if ( ad.VirtualDirectories == null )
                    continue;

                foreach ( var vd in ad.VirtualDirectories )
                {
                    var vdir = app.VirtualDirectories[ vd.Path ];

                    if ( vdir == null )
                    {
                        _logger.LogInformation( "Vdir {SiteName}{AppPath}{VdirPath}: Add", sd.Name, ad.Path, vd.Path );
                        vdir = app.VirtualDirectories.Add( vd.Path, vd.PhysicalPath );
                    }

                    vdir.PhysicalPath = vd.PhysicalPath;
                }
            }
        }

        mgr.CommitChanges();


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
                    _logger.LogWarning( "Site {SiteName}", s.Key );
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

            if ( definition.ApplicationPools.Any( x => x.Name == pool.Name ) == true )
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
