using Microsoft.Web.Administration;
using System.Text.Json.Serialization;

namespace Yttrium.IisDeploy;

/// <summary />
public class ProcessModelDefinition
{
    /// <summary />
    [JsonConverter( typeof( JsonStringEnumConverter ) )]
    public ProcessModelIdentityType IdentityType { get; set; } = ProcessModelIdentityType.ApplicationPoolIdentity;

    /// <summary />
    /// <remarks>Required for service accounts, forbidden otherwise.</remarks>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string? UserName { get; set; }

    /// <summary />
    /// <remarks>Required for service accounts, forbidden otherwise.</remarks>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string? Password { get; set; }
}