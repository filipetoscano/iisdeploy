namespace Yttrium.IisDeploy;

/// <summary />
public class IisNotInstalledException : IisException
{
    /// <summary />
    public IisNotInstalledException()
    {
    }


    /// <summary />
    public IisNotInstalledException( string message ) : base( message )
    {
    }


    /// <summary />
    public IisNotInstalledException( string message, Exception innerException )
        : base( message, innerException )
    {
    }
}