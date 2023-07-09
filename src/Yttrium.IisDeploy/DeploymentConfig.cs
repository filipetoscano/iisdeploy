namespace Yttrium.IisDeploy;

/// <summary />
public class DeploymentConfig
{
    /// <summary />
    public bool BlueGreen { get; set; }

    /// <summary />
    public string? RootSource { get; set; }

    /// <summary />
    public Dictionary<string, string> Source { get; set; } = new Dictionary<string, string>();
}
