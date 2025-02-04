using System.Text.Json.Serialization;

namespace Yttrium.IisDeploy;

/// <summary />
public class DeploymentDefinition
{
    /// <summary>
    /// Name of the deployment (bundle).
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string? Name { get; set; }

    /// <summary>
    /// Whether the deployment has alternating blue/green variants.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingDefault )]
    public bool? HasBlueGreen { get; set; } = false;

    /// <summary>
    /// Physical path from which all app/vdir paths shall be derived.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string? RootPhysicalPath { get; set; }


    /// <summary>
    /// List of sites.
    /// </summary>
    public List<SiteDefinition> Sites { get; set; } = new List<SiteDefinition>();

    /// <summary>
    /// List of application pools.
    /// </summary>
    public List<ApplicationPoolDefinition> ApplicationPools { get; set; } = new List<ApplicationPoolDefinition>();
}