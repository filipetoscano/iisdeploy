using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Yttrium.IisDeploy;

/// <summary />
public class SiteDefinition
{
    /// <summary>
    /// Name of the website.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary />
    public bool AutoStart { get; set; } = true;

    /// <summary />
    public List<SiteBindingDefinition> Bindings { get; set; } = new List<SiteBindingDefinition>();

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public SiteLimitsDefinition? Limits { get; set; }

    /// <summary />
    public List<ApplicationDefinition> Applications { get; set; } = new List<ApplicationDefinition>();
}