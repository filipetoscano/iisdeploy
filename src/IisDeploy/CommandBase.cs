using System;
using System.IO;
using Yttrium.IisDeploy;

namespace IisDeploy
{
    /// <summary />
    public abstract class CommandBase
    {
        /// <summary />
        protected DeploymentDefinition LoadDefinition( string fileName )
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
        protected DeploymentMap LoadMap( string fileName )
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
