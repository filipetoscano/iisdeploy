using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Yttrium.IisDeploy;

/// <summary />
public class SiteDefinition
{
    /// <summary>
    /// Name of the website.
    /// </summary>
    [XmlAttribute( "name" )]
    public string Name { get; set; } = default!;

    /// <summary />
    [XmlAttribute( "autoStart" )]
    public bool AutoStart { get; set; }

    /// <summary />
    [XmlElement( "bindings" )]
    [XmlArrayItem( "binding" )]
    public List<SiteBindingDefinition> Bindings { get; set; } = new List<SiteBindingDefinition>();

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [XmlElement( "limits" )]
    public SiteLimitsDefinition? Limits { get; set; }

    /// <summary />
    [XmlElement( "applications" )]
    [XmlArrayItem( "application" )]
    public List<ApplicationDefinition> Applications { get; set; } = new List<ApplicationDefinition>();
}