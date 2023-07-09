using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.IO;
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
        private readonly IFileLoader _loader;
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
        public ConfigureCommand( ILogger<ConfigureCommand> logger, IFileLoader loader, IIisDeployer deployer )
        {
            _logger = logger;
            _loader = loader;
            _deployer = deployer;
        }


        /// <summary />
        public async Task<int> OnExecuteAsync()
        {
            /*
             *
             */
            var definition = LoadDefinition( _loader, this.DefinitionFile );
            var config = LoadConfiguration( _loader, this.BlueGreen, this.ConfigFile );


            /*
             * 
             */
            var next = MutateDefinition( definition, config.BlueGreen );


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
            await Task.Delay( 1 );
            // await _deployer.Configure( definition );


            /*
             * Update color
             */
            if ( config.BlueGreen == true )
            {
                var bg = Path.Combine( definition.RootPhysicalPath, "blue-green.txt" );

                _logger.LogInformation( "Configured {Color}", next );
                File.WriteAllText( bg, next );
            }

            return 0;
        }
    }
}
