namespace Yttrium.IisDeploy;

/// <summary />
public class DefinitionApplyOptions
{
    /// <summary />
    public bool RemoveUnmanagedSites { get; set; } = true;

    /// <summary />
    public bool RemoveUnmanagedApplications { get; set; } = true;

    /// <summary />
    public bool RemoveUnmanagedVdirs { get; set; } = true;

    /// <summary />
    public bool RemoveUnusedApplicationPools { get; set; } = true;

    /// <summary />
    public bool RecycleManagedApplicationPools { get; set; } = true;
}