using System.Text.Json.Serialization;

namespace Yttrium.IisDeploy;

/// <summary />
public class ApplicationDefinition
{
    /// <summary />
    public const string RootPath = "/";


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
    public List<VirtualDirectoryDefinition>? VirtualDirectories { get; set; }
}