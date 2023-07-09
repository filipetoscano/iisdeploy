namespace Yttrium.IisDeploy;

/// <summary />
public interface IFileLoader
{
    /// <summary />
    IisDefinition LoadDefinitionXml( Stream stream );

    /// <summary />
    IisDefinition LoadDefinitionJson( Stream stream );

    /// <summary />
    DeploymentConfig LoadConfigXml( Stream stream );

    /// <summary />
    DeploymentConfig LoadConfigJson( Stream stream );
}
