using System.Text.Json.Serialization;

namespace Yttrium.IisDeploy;

/// <summary />
public class DeploymentDefinition
{
    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string? Name { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public DeploymentColor? Color { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string? RootPhysicalPath { get; set; }

    /// <summary />
    public List<SiteDefinition> Sites { get; set; } = new List<SiteDefinition>();

    /// <summary />
    public List<ApplicationPoolDefinition> ApplicationPools { get; set; } = new List<ApplicationPoolDefinition>();
}