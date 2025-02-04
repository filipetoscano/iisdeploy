using Microsoft.Web.Administration;
using System.Net;

namespace Yttrium.IisDeploy;

public partial class IisDeployer
{
    /// <inheritdoc />
    public async Task<DeploymentDefinition> Get()
    {
        await Task.Yield();

        using var mgr = new ServerManager();
        var defn = new DeploymentDefinition();


        /*
         * Application pools
         */
        foreach ( var ap in mgr.ApplicationPools )
        {
            var apd = ap.ToDefinition();

            defn.ApplicationPools.Add( apd );
        }


        /*
         * Sites
         */
        foreach ( var site in mgr.Sites )
        {
            var sd = new SiteDefinition();
            sd.Name = site.Name;
            sd.AutoStart = site.ServerAutoStart;

            defn.Sites.Add( sd );

            foreach ( var b in site.Bindings )
            {
                var bd = b.ToDefinition();
                sd.Bindings.Add( bd );
            }


            // Site limits
            var ld = new SiteLimitsDefinition();

            ld.MaxUrlSegments = site.Limits.MaxUrlSegments;

            if ( site.Limits.MaxBandwidth != SiteLimitsDefinition.MaxValue )
                ld.MaxBandwidthBytes = site.Limits.MaxBandwidth;

            if ( site.Limits.MaxConnections != SiteLimitsDefinition.MaxValue )
                ld.MaxConnections = site.Limits.MaxConnections;

            if ( site.Limits.ConnectionTimeout.TotalSeconds > 0 )
                ld.ConnectionTimeout = (long) site.Limits.ConnectionTimeout.TotalSeconds;

            if ( ld.HasDefaultValues() == false )
                sd.Limits = ld;


            /*
             * 
             */
            foreach ( var app in site.Applications )
            {
                if ( sd.Applications == null )
                    sd.Applications = new List<ApplicationDefinition>();

                var ad = new ApplicationDefinition();
                sd.Applications.Add( ad );

                Fill( ad, app );
            }
        }


        /*
         * 
         */
        await Task.Yield();

        return defn;
    }


    /// <summary />
    private static void Fill( ApplicationDefinition defn, Application app )
    {
        /*
         * 
         */
        defn.Path = app.Path;
        defn.PhysicalPath = app.VirtualDirectories[ "/" ].PhysicalPath;
        defn.ApplicationPoolName = app.ApplicationPoolName;


        /*
         * 
         */
        foreach ( var vd in app.VirtualDirectories )
        {
            if ( vd.Path == "/" )
                continue;

            if ( defn.VirtualDirectories == null )
                defn.VirtualDirectories = new List<VirtualDirectoryDefinition>();

            defn.VirtualDirectories.Add( new VirtualDirectoryDefinition()
            {
                Path = vd.Path,
                PhysicalPath = vd.PhysicalPath,
            } );
        }
    }
}