using System.Text.Json.Serialization;

namespace Yttrium.IisDeploy;

/// <summary />
public class ApplicationDefinition
{
    /// <summary>
    /// Virtual path of the website application.
    /// </summary>
    public const string RootPath = "/";


    /// <summary>
    /// Virtual path of the application under a given website.
    /// </summary>
    /// <remarks>
    /// The 'root' application will always have the path set to <see cref="RootPath" />.
    /// </remarks>
    public string Path { get; set; } = default!;

    /// <summary>
    /// Physical path of the application on the local filesystem.
    /// </summary>
    public string PhysicalPath { get; set; } = default!;

    /// <summary>
    /// Name of the application pool.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string? ApplicationPoolName { get; set; }

    /// <summary>
    /// Application pool inline definition.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ApplicationPoolDefinition? ApplicationPool { get; set; }

    /// <summary>
    /// List of virtual directories under the application.
    /// </summary>
    /// <remarks>
    /// Virtual directories will inherit the execution context of the application.
    /// </remarks>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public List<VirtualDirectoryDefinition>? VirtualDirectories { get; set; }
}