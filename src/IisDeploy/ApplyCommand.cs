using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Yttrium.IisDeploy;

namespace IisKnife
{
    /// <summary />
    [Command( "apply", Description = "Applies IIS configuration" )]
    public class ApplyCommand : CommandBase
    {
        private readonly ILogger<ApplyCommand> _logger;
        private readonly IIisDeployer _deployer;


        /// <summary />
        [Argument( 0, Description = "Definition file (JSON)" )]
        [FileExists]
        [Required]
        public string DefinitionFile { get; set; }

        /// <summary />
        [Option( "--options", CommandOptionType.SingleValue, Description = "Options file (JSON)" )]
        [FileExists]
        public string OptionsFile { get; set; }


        /// <summary />
        public ApplyCommand( ILogger<ApplyCommand> logger, IIisDeployer deployer )
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
            var opts = Load<ApplyOptions>( this.OptionsFile );


            /*
             * 
             */
            var state = await _deployer.Apply( defn, opts );

            if ( state.Current.HasValue == true )
                _logger.LogInformation( "🔥 Deployment {Name}: Live with {Color}", defn.Name, state.Current );
            else
                _logger.LogInformation( "🔥 Deployment {Name}: Live", defn.Name );

            return 0;
        }
    }
}