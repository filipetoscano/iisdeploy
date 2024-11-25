using Microsoft.Web.Administration;
using System.Text.Json.Serialization;

namespace Yttrium.IisDeploy;


/// <summary />
public class IisDefinition
{
    /// <summary />
    public string RootPhysicalPath { get; set; } = default!;

    /// <summary />
    public List<ApplicationPoolDefinition> Pools { get; set; } = new List<ApplicationPoolDefinition>();

    /// <summary />
    public List<SiteDefinition> Sites { get; set; } = new List<SiteDefinition>();
}


/// <summary />
public class ApplicationPoolDefinition
{
    /// <summary />
    public string Name { get; set; } = default!;

    /// <summary />
    public bool AutoStart { get; set; }

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
    public string? UserName { get; set; }

    /// <summary />
    /// <remarks>Required for service accounts, forbidden otherwise.</remarks>
    public string? Password { get; set; }
}


/// <summary />
public class SiteDefinition
{
    /// <summary>
    /// Name of the website.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary />
    public string PhysicalPath { get; set; } = default!;

    /// <summary>
    /// Name of the application pool.
    /// </summary>
    /// <remarks>
    /// Must exist in <see cref="IisDefinition.Pools" />. Name is case sensitive.
    /// </remarks>
    public string ApplicationPoolName { get; set; } = default!;

    /// <summary />
    public List<SiteBinding> Bindings { get; set; } = default!;


    /// <summary />
    public List<ApplicationDefinition>? Applications { get; set; }

    /// <summary />
    public List<VirtualDirectoryDefinition>? VirtualDirectories { get; set; }
}


/// <summary />
public class ApplicationDefinition
{
    /// <summary />
    public string Path { get; set; } = default!;

    /// <summary />
    public string PhysicalPath { get; set; } = default!;

    /// <summary />
    public string ApplicationPoolName { get; set; } = default!;
}


/// <summary />
public class VirtualDirectoryDefinition
{
    /// <summary />
    public string Path { get; set; } = default!;

    /// <summary />
    public string PhysicalPath { get; set; } = default!;

    /// <summary />
    public string ApplicationPoolName { get; set; } = default!;
}


/// <summary />
public class SiteBinding
{
    /// <summary />
    [JsonConverter( typeof( JsonStringEnumConverter ) )]
    public Protocol Protocol { get; set; }

    /// <summary />
    public string? Host { get; set; }

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

        if ( this.Host == null )
            bindings.Add( $"*:{this.Port}", proto );
        else
            bindings.Add( $"{this.Host}:{this.Port}", proto );
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