using System.Xml.Serialization;

namespace Yttrium.IisDeploy;

/// <summary />
public class VirtualDirectoryDefinition
{
    /// <summary />
    [XmlAttribute( "path" )]
    public string Path { get; set; } = default!;

    /// <summary />
    [XmlAttribute( "physicalPath" )]
    public string PhysicalPath { get; set; } = default!;
}