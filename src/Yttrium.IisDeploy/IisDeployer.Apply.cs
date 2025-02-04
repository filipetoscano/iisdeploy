using Microsoft.Extensions.Logging;
using Microsoft.Web.Administration;

namespace Yttrium.IisDeploy;

/// <summary />
public partial class IisDeployer
{
    /// <summary />
    public Task<DeploymentState> Apply( DeploymentDefinition defn, DefinitionApplyOptions options )
    {
        /*
         * 
         */
        var state = LoadState( defn );

        NormalizeDefinition( defn );

        if ( defn.HasBlueGreen == true )
        {
            state.Current = state.NextColor();
            MutateDefinition( defn, state.Current.Value );
        }


        /*
         * 
         */
        var mgr = new ServerManager();


        /*
         * #1. Ensure pools
         */
        _logger.LogDebug( "Ensure application pools" );

        foreach ( var pd in defn.ApplicationPools )
        {
            var p = mgr.ApplicationPools.SingleOrDefault( x => x.Name == pd.Name );

            if ( p == null )
            {
                _logger.LogInformation( "Pool {PoolName}: Adding", pd.Name );
                p = mgr.ApplicationPools.Add( pd.Name );
            }


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

        foreach ( var sd in defn.Sites )
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
        if ( options.RemoveUnmanagedSites == true )
        {
            var usite = new Dictionary<string, Site>();

            foreach ( var site in mgr.Sites )
            {
                if ( defn.Sites.Any( x => x.Name == site.Name ) == true )
                    continue;

                usite.Add( site.Name, site );
            }

            if ( usite.Count > 0 )
            {
                foreach ( var s in usite )
                {
                    _logger.LogWarning( "Site {SiteName}", s.Key );
                    mgr.Sites.Remove( s.Value );
                }

                mgr.CommitChanges();
            }
        }


        /*
         * #5. Remove unmanaged apps/vdir apps
         */
        foreach ( var site in mgr.Sites )
        {
            var sd = defn.Sites.SingleOrDefault( x => x.Name == site.Name );

            // Only happens when site wasn't removed, even though it isn't
            // in the definition. Good feature when the server is shared by
            // multiple systems/deployments.
            if ( sd == null )
                continue;


            var uapp = new Dictionary<string, Application>();

            foreach ( var app in site.Applications )
            {
                // Root app belongs to the same. No need to check :)
                if ( app.Path == "/" )
                    continue;

                var ad = sd.Applications.SingleOrDefault( x => x.Path == app.Path );

                if ( ad == null )
                {
                    uapp.Add( app.Path, app );
                    continue;
                }


                var uvdir = new Dictionary<string, VirtualDirectory>();

                foreach ( var vdir in app.VirtualDirectories )
                {
                    var vd = ad.VirtualDirectories?.SingleOrDefault( x => x.Path == vdir.Path );

                    if ( vd == null )
                    {
                        uvdir.Add( vdir.Path, vdir );
                        continue;
                    }
                }

                // Remove vdirs which are unmanaged
                if ( uvdir.Count > 0 && options.RemoveUnmanagedVdirs == true )
                {
                    foreach ( var v in uvdir )
                    {
                        _logger.LogWarning( "Vdir {SiteName}{AppPath}{VdirPath}: Removing", site.Name, app.Path, v.Value.Path );
                        app.VirtualDirectories.Remove( v.Value );
                    }
                }
            }

            // Remove apps which are unmanaged
            if ( uapp.Count > 0 && options.RemoveUnmanagedApplications == true )
            {
                foreach ( var a in uapp )
                {
                    _logger.LogWarning( "Vdir {SiteName}{AppPath}: Removing", site.Name, a.Value.Path );
                    site.Applications.Remove( a.Value );
                }
            }
        }

        mgr.CommitChanges();


        /*
         * #6. Remove unused pools
         */
        if ( options.RemoveUnusedApplicationPools == true )
        {
            var usedPool = new List<string>();
            var upool = new Dictionary<string, ApplicationPool>();

            foreach ( var site in mgr.Sites )
            {
                foreach ( var app in site.Applications )
                    usedPool.Add( app.ApplicationPoolName );
            }

            foreach ( var pool in mgr.ApplicationPools )
            {
                if ( usedPool.Contains( pool.Name ) == true )
                    continue;

                upool.Add( pool.Name, pool );
            }

            if ( upool.Count > 0 )
            {
                foreach ( var s in upool )
                {
                    _logger.LogWarning( "Pool {PoolName}: Removing", s.Key );
                    mgr.ApplicationPools.Remove( s.Value );
                }

                mgr.CommitChanges();
            }
        }


        /*
         * #7. Recycle (managed) pools
         */
        if ( options.RecycleManagedApplicationPools == true )
        {
            foreach ( var p in defn.ApplicationPools )
            {
                var pool = mgr.ApplicationPools.Single( x => x.Name == p.Name );

                _logger.LogInformation( "Pool {PoolName}: Recycling", p.Name );
                pool.Recycle();
            }
        }


        /*
         * #8. Save the state
         */
        state.Moment = DateTime.UtcNow;

        SaveState( defn, state );


        /*
         * 
         */
        return Task.FromResult( state );
    }
}