using Microsoft.Extensions.Logging;
using Microsoft.Web.Administration;
using System.Net;

namespace Yttrium.IisDeploy;

public partial class IisDeployer
{
    /// <inheritdoc />
    public async Task<DeploymentDefinition> Get()
    {
        await Task.Yield();

        var mgr = new ServerManager();
        var defn = new DeploymentDefinition();


        /*
         * Application pools
         */
        foreach ( var ap in mgr.ApplicationPools )
        {
            // Process Model
            var pmd = new ProcessModelDefinition();
            pmd.IdentityType = ap.ProcessModel.IdentityType;
            pmd.UserName = Nullify( ap.ProcessModel.UserName );

            // Recycling
            //_logger.LogDebug( "DisallowOverlappingRotation = {DisallowOverlappingRotation}", ap.Recycling.DisallowOverlappingRotation );
            //_logger.LogDebug( "PeriodicRestart.Time = {PeriodicRestart}", ap.Recycling.PeriodicRestart.Time );
            //_logger.LogDebug( "PeriodicRestart.Requests = {Requests}", ap.Recycling.PeriodicRestart.Requests );
            //_logger.LogDebug( "PeriodicRestart.Memory = {Memory}", ap.Recycling.PeriodicRestart.Memory );
            //_logger.LogDebug( "PeriodicRestart.PrivateMemory = {PrivateMemory}", ap.Recycling.PeriodicRestart.PrivateMemory );
            //_logger.LogDebug( "PeriodicRestart.LogEventOnRecycle = {LogEventOnRecycle}", ap.Recycling.LogEventOnRecycle );

            //foreach ( var x in ap.Recycling.PeriodicRestart.Schedule )
            //    _logger.LogDebug( "PeriodicRestart.Schedule = {Schedule}", x.Time );

            var apd = new ApplicationPoolDefinition();
            apd.Name = ap.Name;
            apd.AutoStart = ap.AutoStart;
            apd.ProcessModel = pmd;
            apd.ManagedPipelineMode = ap.ManagedPipelineMode;
            apd.ManagedRuntimeVersion = Nullify( ap.ManagedRuntimeVersion );

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


            // Site bindings
            foreach ( var b in site.Bindings )
            {
                var bd = new SiteBindingDefinition();
                bd.Protocol = ToProtocol( b.Protocol );
                bd.Host = Nullify( b.Host );
                bd.Port = b.EndPoint.Port;

                if ( b.EndPoint.Address != IPAddress.Any )
                    bd.Address = b.EndPoint.Address.ToString();

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


    /// <summary />
    private static Protocol ToProtocol( string protocol )
    {
        if ( protocol == "http" )
            return Protocol.HTTP;

        if ( protocol == "https" )
            return Protocol.HTTPS;

        throw new NotSupportedException();
    }


    /// <summary />
    private static string? Nullify( string value )
    {
        if ( value == "" )
            return null;

        return value;
    }
}
