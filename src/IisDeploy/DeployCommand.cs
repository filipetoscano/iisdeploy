using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Yttrium.IisDeploy;

namespace IisKnife
{
    /// <summary />
    [Command( "deploy", Description = "Deploys files from source to target directories" )]
    public class DeployCommand : CommandBase
    {
        private readonly ILogger<DeployCommand> _logger;
        private readonly IIisDeployer _deployer;


        /// <summary />
        [Argument( 0, Description = "Definition file (JSON)" )]
        [FileExists]
        [Required]
        public string DefinitionFile { get; set; }

        /// <summary />
        [Argument( 1, Description = "Source map file (JSON)" )]
        [FileExists]
        [Required]
        public string MapFile { get; set; }

        /// <summary />
        [Option( "--options", CommandOptionType.SingleValue, Description = "Options file (JSON)" )]
        [FileExists]
        public string OptionsFile { get; set; }


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
            var defn = Load<DeploymentDefinition>( this.DefinitionFile );
            var maps = Load<DeploymentMap>( this.MapFile );
            var opts = Load<DeployOptions>( this.OptionsFile );


            /*
             * 
             */
            var state = await _deployer.Deploy( defn, maps, opts );

            if ( state.Current.HasValue == true )
                _logger.LogInformation( "✅ Deployment {Name}: Copied files to {Color}", defn.Name, state.NextColor() );
            else
                _logger.LogInformation( "✅ Deployment {Name}: Copied files", defn.Name );

            return 0;
        }
    }
}