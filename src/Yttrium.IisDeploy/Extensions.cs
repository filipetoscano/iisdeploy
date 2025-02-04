using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.Administration;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Yttrium.IisDeploy;

/// <summary />
public static class Extensions
{
    /// <summary />
    public static IServiceCollection AddIisDeployer( this IServiceCollection services )
    {
        services.AddScoped<IisDeployer, IisDeployer>();

        return services;
    }


    /// <summary />
    internal static ApplicationPoolDefinition ToDefinition( this ApplicationPool pool )
    {
        // Process Model
        var pmd = new ProcessModelDefinition();
        pmd.IdentityType = pool.ProcessModel.IdentityType;
        pmd.UserName = pool.ProcessModel.UserName.Nullify();

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
        apd.Name = pool.Name;
        apd.AutoStart = pool.AutoStart;

        if ( pool.StartMode != ApplicationPoolDefaults.StartMode )
            apd.StartMode = pool.StartMode;

        if ( pool.QueueLength != ApplicationPoolDefaults.QueueLength )
            apd.QueueLength = pool.QueueLength;

        apd.ManagedPipelineMode = pool.ManagedPipelineMode;
        apd.ManagedRuntimeVersion = pool.ManagedRuntimeVersion.Nullify();

        apd.ProcessModel = pmd;

        return apd;
    }


    /// <summary />
    internal static SiteBindingDefinition ToDefinition( this Binding b )
    {
        var bd = new SiteBindingDefinition();
        bd.Protocol = ToProtocol( b.Protocol );
        bd.Host = b.Host.Nullify();
        bd.Port = b.EndPoint.Port;

        if ( b.EndPoint.Address != IPAddress.Any )
            bd.Address = b.EndPoint.Address.ToString();

        if ( bd.Protocol == Protocol.HTTPS )
        {
            bd.CertificateStore = (StoreName) Enum.Parse( typeof( StoreName ), b.CertificateStoreName );
            bd.CertificateHash = U.ConvertBytesToCertificateHexString( b.CertificateHash ).ToLowerInvariant();
            bd.SslFlags = (BindingSslFlags) (int) b.SslFlags;
        }

        return bd;
    }


    /// <summary />
    internal static string? Nullify( this string value )
    {
        if ( value == "" )
            return null;

        return value;
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
}
