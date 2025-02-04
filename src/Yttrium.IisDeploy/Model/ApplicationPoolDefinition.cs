using Microsoft.Web.Administration;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Yttrium.IisDeploy;

/// <summary />
public class ApplicationPoolDefinition
{
    /// <summary>
    /// Name of the application pool.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Whether the pool autostarts when IIS service starts, or whether a manual
    /// intervention from an admin user is required.
    /// </summary>
    public bool AutoStart { get; set; } = true;

    /// <summary />
    [JsonConverter( typeof( JsonStringEnumConverter ) )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public StartMode? StartMode { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public long? QueueLength { get; set; }

    /// <summary />
    [JsonConverter( typeof( JsonStringEnumConverter ) )]
    [XmlElement( "managedPipelineMode" )]
    public ManagedPipelineMode ManagedPipelineMode { get; set; }

    /// <summary />
    /// <remarks>Use null if 'No Managed Code'.</remarks>
    public string? ManagedRuntimeVersion { get; set; }

    /// <summary />
    public ProcessModelDefinition ProcessModel { get; set; } = default!;
}


/// <summary />
public class ApplicationPoolDefaults
{
    /// <summary />
    public static StartMode StartMode { get; } = StartMode.OnDemand;

    /// <summary />
    public static long QueueLength { get; } = 1000;
}