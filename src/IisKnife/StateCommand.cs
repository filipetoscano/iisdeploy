using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Threading.Tasks;
using Yttrium.IisDeploy;

namespace IisKnife
{
    /// <summary />
    [Command( "state", Description = "For blue/green deployments, gets the current / next versions" )]
    public class StateCommand : CommandBase
    {
        private readonly ILogger<StateCommand> _logger;
        private readonly IIisDeployer _deployer;


        /// <summary />
        [Argument( 0, Description = "Definition file (XML/JSON)" )]
        [FileExists]
        [Required]
        public string DefinitionFile { get; set; }

        /// <summary />
        [Option( "--json", CommandOptionType.NoValue, Description = "Output as JSON object" )]
        public bool AsJson { get; set; } = false;


        /// <summary />
        public StateCommand( ILogger<StateCommand> logger, IIisDeployer deployer )
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
            var state = await _deployer.State( defn );


            if ( this.AsJson == true )
            {
                var obj = new
                {
                    Current = state.Current,
                    Next = state.NextColor(),
                    Moment = state.Moment,
                };

                var json = JsonSerializer.Serialize( obj, new JsonSerializerOptions()
                {
                    WriteIndented = true,
                } );

                Console.WriteLine( json );
            }
            else
            {
                _logger.LogInformation( "Current: {Current}", state.Current );
                _logger.LogInformation( "Next: {Next}", state.NextColor() );
            }

            return 0;
        }
    }
}