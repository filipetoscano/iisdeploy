using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Yttrium.IisDeploy;

namespace IisDeploy
{
    /// <summary />
    [Command( "deploy", Description = "Deploys files from source to target directories" )]
    public class DeployCommand : CommandBase
    {
        private readonly ILogger<DeployCommand> _logger;
        private readonly IIisDeployer _deployer;


        /// <summary />
        [Argument( 0, Description = "Definition file (XML/JSON)" )]
        [FileExists]
        [Required]
        public string DefinitionFile { get; set; }

        /// <summary />
        [Option( "-c|--config", CommandOptionType.SingleValue, Description = "Configuration file (XML/JSON)" )]
        [FileExists]
        public string MapFile { get; set; }

        /// <summary />
        [Option( "-x|--blue-green", CommandOptionType.NoValue, Description = "Whether to use blue/green alternates" )]
        public bool? BlueGreen { get; set; }


        /// <summary />
        public DeployCommand( ILogger<DeployCommand> logger, IIisDeployer deployer )
        {
            _logger = logger;
            _deployer = deployer;
        }


        /// <summary />
        public async Task<int> OnExecuteAsync()
        {
            /*
             * 
             */
            var definition = LoadDefinition( this.DefinitionFile );
            var map = LoadMap( this.MapFile );


            /*
             *
             */
            DeploymentColor? next = null;

            if ( this.BlueGreen == true )
            {
                var color = await _deployer.ColorGet( definition.Name );
                next = color == DeploymentColor.Blue ? DeploymentColor.Green : DeploymentColor.Blue;

                await _deployer.Mutate( definition, next.Value );
            }


            /*
             * 
             */
            await _deployer.Deploy( definition, map );

            return 0;
        }
    }
}
