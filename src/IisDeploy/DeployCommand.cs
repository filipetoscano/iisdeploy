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
        [Option( "-m|--map", CommandOptionType.SingleValue, Description = "Source map file (XML/JSON)" )]
        [FileExists]
        [Required]
        public string MapFile { get; set; }


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
            var defn = LoadDefinition( this.DefinitionFile );
            var map = LoadMap( this.MapFile );


            /*
             * 
             */
            var state = await _deployer.Deploy( defn, map );

            if ( state.Current.HasValue == true )
                _logger.LogInformation( "✅ Deployment {Name}: Copied files to {Color}", defn.Name, state.NextColor() );
            else
                _logger.LogInformation( "✅ Deployment {Name}: Copied files", defn.Name );

            return 0;
        }
    }
}