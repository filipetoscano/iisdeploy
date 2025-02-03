using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Yttrium.IisDeploy;

/// <summary />
public class DeploymentDefinition
{
    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [XmlElement( "name" )]
    public string? Name { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [XmlElement( "rootPhysicalPath" )]
    public string? RootPhysicalPath { get; set; }

    /// <summary />
    [XmlElement( "sites" )]
    [XmlArrayItem( "site" )]
    public List<SiteDefinition> Sites { get; set; } = new List<SiteDefinition>();

    /// <summary />
    [XmlElement( "applicationPools" )]
    [XmlArrayItem( "applicationPool" )]
    public List<ApplicationPoolDefinition> ApplicationPools { get; set; } = new List<ApplicationPoolDefinition>();
}