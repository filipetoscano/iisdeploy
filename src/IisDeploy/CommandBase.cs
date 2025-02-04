using System;
using System.IO;
using System.Text.Json;
using Yttrium.IisDeploy;

namespace IisDeploy
{
    /// <summary />
    public abstract class CommandBase
    {
        /// <summary />
        protected DeploymentDefinition LoadDefinition( string fileName )
        {
            DeploymentDefinition obj;

            using ( var stream = new FileStream( fileName, FileMode.Open ) )
            {
                if ( Path.GetExtension( fileName ) == ".xml" )
                {
                    throw new NotImplementedException();
                }
                else
                {
                    obj = JsonSerializer.Deserialize<DeploymentDefinition>( stream );
                }
            }

            return obj;
        }


        /// <summary />
        protected DeploymentMap LoadMap( string fileName )
        {
            DeploymentMap obj;

            using ( var stream = new FileStream( fileName, FileMode.Open ) )
            {
                if ( Path.GetExtension( fileName ) == ".xml" )
                {
                    throw new NotImplementedException();
                }
                else
                {
                    obj = JsonSerializer.Deserialize<DeploymentMap>( stream );
                }
            }

            return obj;
        }
    }
}
