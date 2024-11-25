namespace Yttrium.IisDeploy;

/// <summary />
public class DeploymentConfig
{
    /// <summary>
    /// Root folder for application source folders.
    /// </summary>
    /// <remarks>
    /// If unspecified, all of the values in 'Source' must be absolute paths.
    /// Otherwise, they may be relative.
    /// </remarks>
    public string? RootSource { get; set; }

    /// <summary>
    /// Application source folders.
    /// </summary>
    public Dictionary<string, string> Source { get; set; } = new Dictionary<string, string>();
}
