namespace Yttrium.IisDeploy;

/// <summary />
public class DeploymentMap
{
    /// <summary>
    /// Root folder for application/vdir source folders.
    /// </summary>
    /// <remarks>
    /// If unspecified, all of the values in 'Source' must be absolute paths.
    /// Otherwise, they may be relative.
    /// </remarks>
    public string? RootSource { get; set; }

    /// <summary>
    /// Application/vdirs source folders, relative to root source.
    /// </summary>
    public Dictionary<string, string> Source { get; set; } = new Dictionary<string, string>();
}