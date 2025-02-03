using System;
using System.IO;
using Yttrium.IisDeploy;
using Yttrium.IisDeploy.Model;

namespace IisDeploy
{
    /// <summary />
    public abstract class CommandBase
    {
        /// <summary />
        protected DeploymentDefinition LoadDefinition( IFileLoader loader, string fileName )
        {
            using ( var stream = new FileStream( fileName, FileMode.Open ) )
            {
                throw new NotImplementedException();
                //if ( Path.GetExtension( fileName ) == ".json" )
                //    return loader.LoadDefinitionJson( stream );
                //else
                //    return loader.LoadDefinitionXml( stream );
            }
        }


        /// <summary />
        protected DeploymentMap LoadMap( IFileLoader loader, string fileName )
        {
            DeploymentMap cfg;

            using ( var stream = new FileStream( fileName, FileMode.Open ) )
            {
                throw new NotImplementedException();

                //if ( Path.GetExtension( fileName ).ToLowerInvariant() == ".json" )
                //    cfg = loader.LoadConfigJson( stream );
                //else
                //    cfg = loader.LoadConfigXml( stream );
            }

            return cfg;
        }
    }
}
