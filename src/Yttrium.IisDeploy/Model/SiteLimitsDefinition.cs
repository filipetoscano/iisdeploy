using System.Text.Json.Serialization;

namespace Yttrium.IisDeploy;

/// <summary />
public class SiteLimitsDefinition
{
    /// <summary />
    public const long MaxValue = UInt32.MaxValue;


    /// <summary />
    public long MaxUrlSegments { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public long? MaxBandwidthBytes { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public long? MaxConnections { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public long? ConnectionTimeout { get; set; }


    /// <summary />
    public bool HasDefaultValues()
    {
        if ( this.MaxUrlSegments != 32 )
            return false;

        if ( this.MaxBandwidthBytes != null )
            return false;

        if ( this.MaxConnections != null )
            return false;

        if ( this.ConnectionTimeout != null )
            return false;

        return true;
    }
}