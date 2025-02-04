using System.Text.Json.Serialization;

namespace Yttrium.IisDeploy;

/// <summary />
[JsonConverter( typeof( JsonStringEnumConverter ) )]
[Flags]
public enum BindingSslFlags
{
    /// <summary>
    /// No SSL flags.
    /// </summary>
    None = 0,

    /// <summary>
    /// Requires (SNI) Server Name Indication.
    /// </summary>
    Sni = 1,

    /// <summary>
    /// ?
    /// </summary>
    CentralCertStore = 2,

    /// <summary>
    /// Disable HTTP/2
    /// </summary>
    DisableHttp2 = 4,

    /// <summary>
    /// Disable OCSP stapling
    /// </summary>
    DisableOcspStapling = 8,
}