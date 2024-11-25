using Microsoft.Web.Administration;

namespace Yttrium.IisDeploy;

public partial class IisDeployer
{
    /// <inheritdoc />
    public async Task<IisDefinition> Get()
    {
        var mgr = new ServerManager();
        var defn = new IisDefinition();


        /*
         * Application pools
         */
        foreach ( var ap in mgr.ApplicationPools )
        {
            var pmd = new ProcessModelDefinition();
            pmd.IdentityType = ap.ProcessModel.IdentityType;
            pmd.UserName = ap.ProcessModel.UserName;

            var apd = new ApplicationPoolDefinition();
            apd.Name = ap.Name;
            apd.AutoStart = ap.AutoStart;
            apd.ProcessModel = pmd;

            defn.Pools.Add( apd );
        }


        /*
         * 
         */




        /*
         * 
         */
        await Task.Yield();

        return defn;
    }
}
