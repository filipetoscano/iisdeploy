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
    [XmlAttribute( "name" )]
    public string Name { get; set; } = default!;

    /// <summary>
    /// Whether the pool autostarts when IIS service starts, or whether a manual
    /// intervention from an admin user is required.
    /// </summary>
    [XmlAttribute( "autoStart" )]
    public bool AutoStart { get; set; }

    /// <summary />
    [JsonConverter( typeof( JsonStringEnumConverter ) )]
    [XmlElement( "managedPipelineMode" )]
    public ManagedPipelineMode ManagedPipelineMode { get; set; }

    /// <summary />
    [XmlElement( "managedRuntimeVersion" )]
    public string? ManagedRuntimeVersion { get; set; } = default!;

    /// <summary />
    [XmlElement( "processModel" )]
    public ProcessModelDefinition ProcessModel { get; set; } = default!;
}