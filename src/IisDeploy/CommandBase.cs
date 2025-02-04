using System;
using System.IO;
using System.Text.Json;
using Yttrium.IisDeploy;

namespace IisKnife
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
                obj = JsonSerializer.Deserialize<DeploymentDefinition>( stream );
            }

            return obj;
        }


        /// <summary />
        protected DeploymentMap LoadMap( string fileName )
        {
            DeploymentMap obj;

            using ( var stream = new FileStream( fileName, FileMode.Open ) )
            {
                obj = JsonSerializer.Deserialize<DeploymentMap>( stream );
            }

            return obj;
        }
    }
}
