using System.IO;
using Yttrium.IisDeploy;

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
                if ( Path.GetExtension( fileName ) == ".json" )
                    return loader.LoadDefinitionJson( stream );
                else
                    return loader.LoadDefinitionXml( stream );
            }
        }


        /// <summary />
        protected DeploymentConfig LoadConfiguration( IFileLoader loader, string fileName )
        {
            DeploymentConfig cfg;

            using ( var stream = new FileStream( fileName, FileMode.Open ) )
            {
                if ( Path.GetExtension( fileName ).ToLowerInvariant() == ".json" )
                    cfg = loader.LoadConfigJson( stream );
                else
                    cfg = loader.LoadConfigXml( stream );
            }

            return cfg;
        }


        /// <summary />
        protected void MutateDefinition( DeploymentDefinition definition, IisColor next )
        {
            var nextDir = next.ToString();

            foreach ( var s in definition.Sites )
            {
                s.PhysicalPath = Path.Combine( s.PhysicalPath, nextDir );

                if ( s.VirtualDirectories == null )
                    continue;

                foreach ( var vd in s.VirtualDirectories )
                    vd.PhysicalPath = Path.Combine( vd.PhysicalPath, nextDir );
            }
        }
    }
}
