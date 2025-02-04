namespace Yttrium.IisDeploy;

/// <summary />
public class DeploymentState
{
    /// <summary />
    public DeploymentColor? Current { get; set; }

    /// <summary />
    public DateTime? Moment { get; set; }


    /// <summary />
    public DeploymentColor NextColor()
    {
        if ( Current.HasValue == false )
            return DeploymentColor.Blue;

        if ( Current.Value == DeploymentColor.Blue )
            return DeploymentColor.Green;

        return DeploymentColor.Blue;
    }
}