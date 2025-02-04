using System.Text.Json.Serialization;

namespace Yttrium.IisDeploy;

/// <summary />
[JsonConverter( typeof( JsonStringEnumConverter ) )]
public enum Protocol
{
    /// <summary />
    HTTP,

    /// <summary />
    HTTPS,
}