using Microsoft.Web.Administration;
using System.Text.Json.Serialization;

namespace Yttrium.IisDeploy;

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