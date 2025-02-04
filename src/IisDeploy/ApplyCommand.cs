using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
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
        [Argument( 0, Description = "Definition file" )]
        [FileExists]
        [Required]
        public string DefinitionFile { get; set; }

        /// <summary />
        [Option( "-v|--verbose", CommandOptionType.NoValue, Description = "Verbose output" )]
        public bool Verbose { get; set; }


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
            var defn = LoadDefinition( this.DefinitionFile );


            /*
             * 
             */
            if ( this.Verbose == true )
            {
                var jso = new JsonSerializerOptions() { WriteIndented = true };
                var def = JsonSerializer.Serialize( defn, jso );

                _logger.LogDebug( "Definition: {Definition}", def );

                return 1;
            }


            /*
             * 
             */
            var state = await _deployer.Apply( defn, new DefinitionApplyOptions()
            {
            } );

            if ( state.Current.HasValue == true )
                _logger.LogInformation( "🔥 Deployment {Name}: Live with {Color}", defn.Name, state.Current );
            else
                _logger.LogInformation( "🔥 Deployment {Name}: Live", defn.Name );

            return 0;
        }
    }
}
