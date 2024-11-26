using Microsoft.Web.Administration;
using System.Text.Json.Serialization;

namespace Yttrium.IisDeploy;


/// <summary />
public class IisDefinition
{
    /// <summary />
    public string RootPhysicalPath { get; set; } = default!;

    /// <summary />
    public List<SiteDefinition> Sites { get; set; } = new List<SiteDefinition>();

    /// <summary />
    public List<ApplicationPoolDefinition> ApplicationPools { get; set; } = new List<ApplicationPoolDefinition>();
}


/// <summary />
public class ApplicationPoolDefinition
{
    /// <summary />
    public string Name { get; set; } = default!;

    /// <summary />
    public bool AutoStart { get; set; }

    /// <summary />
    [JsonConverter( typeof( JsonStringEnumConverter ) )]
    public ManagedPipelineMode ManagedPipelineMode { get; set; }

    /// <summary />
    public string? ManagedRuntimeVersion { get; set; } = default!;

    /// <summary />
    public ProcessModelDefinition ProcessModel { get; set; } = default!;
}


/// <summary />
public class ProcessModelDefinition
{
    /// <summary />
    [JsonConverter( typeof( JsonStringEnumConverter ) )]
    public ProcessModelIdentityType IdentityType { get; set; } = ProcessModelIdentityType.ApplicationPoolIdentity;

    /// <summary />
    /// <remarks>Required for service accounts, forbidden otherwise.</remarks>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string? UserName { get; set; }

    /// <summary />
    /// <remarks>Required for service accounts, forbidden otherwise.</remarks>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string? Password { get; set; }
}


/// <summary />
public class SiteDefinition : ApplicationDefinition
{
    /// <summary>
    /// Name of the website.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary />
    public bool AutoStart { get; set; }

    /// <summary />
    public List<SiteBindingDefinition> Bindings { get; set; } = new List<SiteBindingDefinition>();

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public SiteLimitsDefinition? Limits { get; set;  }
}


/// <summary />
public class SiteLimitsDefinition
{
    /// <summary />
    public const long MaxValue = UInt32.MaxValue;


    /// <summary />
    public long MaxUrlSegments { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public long? MaxBandwidthBytes { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public long? MaxConnections { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public long? ConnectionTimeout { get; set; }


    /// <summary />
    public bool HasDefaultValues()
    {
        if ( this.MaxUrlSegments != 32 )
            return false;

        if ( this.MaxBandwidthBytes != null )
            return false;

        if ( this.MaxConnections != null )
            return false;

        if ( this.ConnectionTimeout != null )
            return false;

        return true;
    }
}


/// <summary />
public class ApplicationDefinition
{
    /// <summary />
    public string Path { get; set; } = default!;

    /// <summary />
    public string PhysicalPath { get; set; } = default!;

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string? ApplicationPoolName { get; set; }

    /// <summary>
    /// Application pool inline definition.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ApplicationPoolDefinition? ApplicationPool { get; set; }


    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public List<ApplicationDefinition>? Applications { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public List<VirtualDirectoryDefinition>? VirtualDirectories { get; set; }
}


/// <summary />
public class VirtualDirectoryDefinition
{
    /// <summary />
    public string Path { get; set; } = default!;

    /// <summary />
    public string PhysicalPath { get; set; } = default!;
}


/// <summary />
public class SiteBindingDefinition
{
    /// <summary />
    [JsonConverter( typeof( JsonStringEnumConverter ) )]
    public Protocol Protocol { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string? Host { get; set; }

    /// <summary>
    /// IP address, or null for any.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string? Address { get; set; }

    /// <summary />
    public int Port { get; set; }


    /// <summary />
    internal void AddTo( BindingCollection bindings )
    {
        string proto = this.Protocol switch
        {
            Protocol.HTTP => "http",
            Protocol.HTTPS => "https",
            _ => throw new InvalidOperationException(),
        };

        // TODO: https requires certificate data

        if ( this.Host == null )
            bindings.Add( $"{this.Address ?? "*"}:{this.Port}", proto );
        else
            bindings.Add( $"{this.Address ?? "*"}:{this.Port}:{this.Host}", proto );
    }
}


/// <summary />
public enum Protocol
{
    /// <summary />
    HTTP,

    /// <summary />
    HTTPS,
}
