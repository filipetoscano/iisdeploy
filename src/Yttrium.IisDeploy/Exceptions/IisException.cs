namespace Yttrium.IisDeploy;

/// <summary />
public class IisException : ApplicationException
{
    /// <summary />
    public IisException()
    {
    }


    /// <summary />
    public IisException( string message ) : base( message )
    {
    }


    /// <summary />
    public IisException( string message, Exception innerException )
        : base( message, innerException )
    {
    }
}