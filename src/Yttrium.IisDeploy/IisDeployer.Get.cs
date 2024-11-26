using Microsoft.Extensions.Logging;
using Microsoft.Web.Administration;
using System.Net;

namespace Yttrium.IisDeploy;

public partial class IisDeployer
{
    /// <inheritdoc />
    public async Task<IisDefinition> Get()
    {
        var mgr = new ServerManager();
        var defn = new IisDefinition();


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
        foreach ( var s in mgr.Sites )
        {
            var sd = new SiteDefinition();
            sd.Name = s.Name;
            sd.AutoStart = s.ServerAutoStart;

            defn.Sites.Add( sd );


            // Site bindings
            foreach ( var b in s.Bindings )
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

            ld.MaxUrlSegments = s.Limits.MaxUrlSegments;

            if ( s.Limits.MaxBandwidth != SiteLimitsDefinition.MaxValue )
                ld.MaxBandwidthBytes = s.Limits.MaxBandwidth;

            if ( s.Limits.MaxConnections != SiteLimitsDefinition.MaxValue )
                ld.MaxConnections = s.Limits.MaxConnections;

            if ( s.Limits.ConnectionTimeout.TotalSeconds > 0 )
                ld.ConnectionTimeout = (long) s.Limits.ConnectionTimeout.TotalSeconds;

            if ( ld.HasDefaultValues() == false )
                sd.Limits = ld;


            /*
             * 
             */
            var sa = s.Applications.Single( x => x.Path == "/" );
            Fill( sd, sa );


            /*
             * 
             */
            foreach ( var a in s.Applications )
            {
                if ( a.Path == "/" )
                    continue;

                if ( sd.Applications == null )
                    sd.Applications = new List<ApplicationDefinition>();

                var suba = new ApplicationDefinition();
                sd.Applications.Add( suba );

                Fill( suba, a );
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
