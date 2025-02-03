using Microsoft.Web.Administration;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Yttrium.IisDeploy;

/// <summary />
public class ProcessModelDefinition
{
    /// <summary />
    [JsonConverter( typeof( JsonStringEnumConverter ) )]
    [XmlAttribute( "identityType" )]
    public ProcessModelIdentityType IdentityType { get; set; } = ProcessModelIdentityType.ApplicationPoolIdentity;

    /// <summary />
    /// <remarks>Required for service accounts, forbidden otherwise.</remarks>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [XmlAttribute( "userName" )]
    public string? UserName { get; set; }

    /// <summary />
    /// <remarks>Required for service accounts, forbidden otherwise.</remarks>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [XmlAttribute( "password" )]
    public string? Password { get; set; }
}