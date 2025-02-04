using Microsoft.Web.Administration;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

namespace Yttrium.IisDeploy;

/// <summary />
public class SiteBindingDefinition
{
    /// <summary>
    /// Protocol.
    /// </summary>
    public Protocol Protocol { get; set; }

    /// <summary>
    /// Host name, or null for any.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string? Host { get; set; }

    /// <summary>
    /// IP address, or null for any.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string? Address { get; set; }

    /// <summary>
    /// TCP port.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Certificate location (Windows store name).
    /// </summary>
    [JsonConverter( typeof( JsonStringEnumConverter ) )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public StoreName? CertificateStore { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string? CertificateHash { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public BindingSslFlags? SslFlags { get; set; }



    /// <summary />
    internal Binding AddTo( BindingCollection bindings )
    {
        var proto = this.Protocol switch
        {
            Protocol.HTTP => "http",
            Protocol.HTTPS => "https",
            _ => throw new InvalidOperationException(),
        };

        var information = $"{Address}:{Port}:{Host}";

        /*
         * 
         */
        var b = bindings.SingleOrDefault( x => x.Protocol == proto && x.BindingInformation == information );

        if ( b == null )
            b = bindings.Add( information, proto );


        /*
         * 
         */
        if ( this.Protocol == Protocol.HTTP )
        {
            b.CertificateStoreName = null;
            b.CertificateHash = null;
            b.SslFlags = Microsoft.Web.Administration.SslFlags.None;
        }
        else
        {
            b.CertificateStoreName = CertificateStore?.ToString();
            b.CertificateHash = CertificateHash == null ? null : U.ConvertCertificateHexStringToBytes( CertificateHash!.ToUpperInvariant() );
            b.SslFlags = (SslFlags) (int) ( SslFlags ?? BindingSslFlags.None );
        }

        return b;
    }
}