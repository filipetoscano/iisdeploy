﻿using Microsoft.Web.Administration;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Yttrium.IisDeploy;

/// <summary />
public class SiteBindingDefinition
{
    /// <summary />
    [JsonConverter( typeof( JsonStringEnumConverter ) )]
    [XmlAttribute( "protocol")]
    public Protocol Protocol { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [XmlAttribute( "host" )]
    public string? Host { get; set; }

    /// <summary>
    /// IP address, or null for any.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [XmlAttribute( "address" )]
    public string? Address { get; set; }

    /// <summary />
    [XmlAttribute( "port" )]
    public int Port { get; set; }


    /// <summary />
    internal void AddTo( BindingCollection bindings )
    {
        string proto = this.Protocol switch
        {
            Protocol.HTTP => "http",
            Protocol.HTTPS => "https",
            _ => throw new InvalidOperationException(),
        };

        // TODO: https requires certificate data

        if ( this.Host == null )
            bindings.Add( $"{this.Address ?? "*"}:{this.Port}", proto );
        else
            bindings.Add( $"{this.Address ?? "*"}:{this.Port}:{this.Host}", proto );
    }
}