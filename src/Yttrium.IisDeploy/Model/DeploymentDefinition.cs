using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Yttrium.IisDeploy;

/// <summary />
[XmlRoot( "deployment" )]
public class DeploymentDefinition
{
    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [XmlAttribute( "name" )]
    public string? Name { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [XmlAttribute( "hasBlueGreen" )]
    public bool? HasBlueGreen { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [XmlElement( "rootPhysicalPath" )]
    public string? RootPhysicalPath { get; set; }


    /// <summary />
    [XmlArray( "sites" )]
    [XmlArrayItem( "site" )]
    public List<SiteDefinition> Sites { get; set; } = new List<SiteDefinition>();

    /// <summary />
    [XmlArray( "applicationPools" )]
    [XmlArrayItem( "applicationPool" )]
    public List<ApplicationPoolDefinition> ApplicationPools { get; set; } = new List<ApplicationPoolDefinition>();
}