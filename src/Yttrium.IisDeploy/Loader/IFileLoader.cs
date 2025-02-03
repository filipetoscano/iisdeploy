namespace Yttrium.IisDeploy;

/// <summary />
public interface IFileLoader
{
    /// <summary />
    DeploymentDefinition LoadDefinition( Stream stream );

    /// <summary />
    DeploymentConfig LoadMap( Stream stream );
}