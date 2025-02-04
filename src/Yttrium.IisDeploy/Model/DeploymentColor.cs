using System.Text.Json.Serialization;

namespace Yttrium.IisDeploy;

/// <summary />
[JsonConverter( typeof( JsonStringEnumConverter ) )]
public enum DeploymentColor
{
    /// <summary>
    /// Blue environment.
    /// </summary>
    Blue = 1,

    /// <summary>
    /// Green environment.
    /// </summary>
    Green,
}