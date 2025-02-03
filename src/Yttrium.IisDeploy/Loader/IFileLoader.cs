using Yttrium.IisDeploy.Model;

namespace Yttrium.IisDeploy;

/// <summary />
public interface IFileLoader
{
    /// <summary />
    DeploymentDefinition LoadDefinition( Stream stream );

    /// <summary />
    DeploymentMap LoadMap( Stream stream );
}