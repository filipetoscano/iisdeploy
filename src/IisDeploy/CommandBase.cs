using System.IO;
using Yttrium.IisDeploy;

namespace IisDeploy
{
    /// <summary />
    public abstract class CommandBase
    {
        /// <summary />
        protected IisDefinition LoadDefinition( IFileLoader loader, string fileName )
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
        protected DeploymentConfig LoadConfiguration( IFileLoader loader, bool? blueGreen, string fileName )
        {
            DeploymentConfig cfg;

            using ( var stream = new FileStream( fileName, FileMode.Open ) )
            {
                if ( Path.GetExtension( fileName ) == ".json" )
                    cfg = loader.LoadConfigJson( stream );
                else
                    cfg = loader.LoadConfigXml( stream );
            }

            if ( blueGreen.HasValue == true )
                cfg.BlueGreen = blueGreen.Value;

            return cfg;
        }


        /// <summary />
        protected string LoadBlueGreen( IisDefinition definition )
        {
            var bg = Path.Combine( definition.RootPhysicalPath, "blue-green.txt" );

            if ( File.Exists( bg ) == false )
                return null;


            var current = File.ReadAllText( bg ).Trim().ToLowerInvariant();

            return current;
        }


        /// <summary />
        protected string MutateDefinition( IisDefinition definition, bool blueGreen )
        {
            if ( blueGreen == false )
                return null;


            /*
             * 
             */
            var current = LoadBlueGreen( definition );
            var next = current == "blue" ? "green" : "blue";


            /*
             * 
             */
            foreach ( var s in definition.Sites )
            {
                s.PhysicalPath = Path.Combine( s.PhysicalPath, next );

                if ( s.VirtualDirectories == null )
                    continue;

                foreach ( var vd in s.VirtualDirectories )
                    vd.PhysicalPath = Path.Combine( vd.PhysicalPath, next );
            }

            return next;
        }
    }
}
