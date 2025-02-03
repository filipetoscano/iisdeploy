using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Threading.Tasks;
using Yttrium.IisDeploy;

namespace IisDeploy
{
    /// <summary />
    [Command( "configure", Description = "Configures IIS websites / virtual directories" )]
    public class ConfigureCommand : CommandBase
    {
        private readonly ILogger<ConfigureCommand> _logger;
        private readonly IIisDeployer _deployer;


        /// <summary />
        [Argument( 0, Description = "Definition file (XML/JSON)" )]
        [FileExists]
        [Required]
        public string DefinitionFile { get; set; }

        /// <summary />
        [Option( "-c|--config", CommandOptionType.SingleValue, Description = "Configuration file (XML/JSON)" )]
        [FileExists]
        public string ConfigFile { get; set; }

        /// <summary />
        [Option( "-x|--blue-green", CommandOptionType.NoValue, Description = "Whether to use blue/green alternates" )]
        public bool? BlueGreen { get; set; }

        /// <summary />
        [Option( "-v|--verbose", CommandOptionType.NoValue, Description = "Verbose output" )]
        public bool Verbose { get; set; }


        /// <summary />
        public ConfigureCommand( ILogger<ConfigureCommand> logger, IIisDeployer deployer )
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
            var config = LoadMap( this.ConfigFile );


            /*
             *
             */
            DeploymentColor? next = null;

            if ( this.BlueGreen == true )
            {
                var color = await _deployer.ColorGet( definition.Name );
                next = color == DeploymentColor.Blue ? DeploymentColor.Green : DeploymentColor.Green;

                await _deployer.Mutate( definition, next.Value );
            }


            /*
             * 
             */
            if ( this.Verbose == true )
            {
                var jso = new JsonSerializerOptions() { WriteIndented = true };
                var def = JsonSerializer.Serialize( definition, jso );
                var cfg = JsonSerializer.Serialize( config, jso );

                _logger.LogDebug( "Definition: {Definition}", def );
                _logger.LogDebug( "Config: {Config}", cfg );

                return 1;
            }


            /*
             * 
             */
            await _deployer.Apply( definition, new DefinitionApplyOptions()
            {
            } );

            return 0;
        }
    }
}
