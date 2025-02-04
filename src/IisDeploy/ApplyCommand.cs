using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Threading.Tasks;
using Yttrium.IisDeploy;

namespace IisDeploy
{
    /// <summary />
    [Command( "apply", Description = "Applies IIS configuration" )]
    public class ApplyCommand : CommandBase
    {
        private readonly ILogger<ApplyCommand> _logger;
        private readonly IIisDeployer _deployer;


        /// <summary />
        [Argument( 0, Description = "Definition file (XML/JSON)" )]
        [FileExists]
        [Required]
        public string DefinitionFile { get; set; }

        /// <summary />
        [Option( "-m|--map", CommandOptionType.SingleValue, Description = "Map file (XML/JSON)" )]
        [FileExists]
        public string MapFile { get; set; }

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
            var config = LoadMap( this.MapFile );


            /*
             * 
             */
            if ( this.Verbose == true )
            {
                var jso = new JsonSerializerOptions() { WriteIndented = true };
                var def = JsonSerializer.Serialize( defn, jso );
                var cfg = JsonSerializer.Serialize( config, jso );

                _logger.LogDebug( "Definition: {Definition}", def );
                _logger.LogDebug( "Config: {Config}", cfg );

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
