namespace Yttrium.IisDeploy;

/// <summary />
public interface IIisDeployer
{
    /// <summary />
    Task Deploy( IisDefinition definition, DeploymentConfig config );

    /// <summary />
    Task Configure( IisDefinition definition );
}
