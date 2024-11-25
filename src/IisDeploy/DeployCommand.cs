using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Threading.Tasks;
using Yttrium.IisDeploy;

namespace IisDeploy
{
    /// <summary />
    [Command( "deploy", Description = "Deploys files from source to target directories" )]
    public class DeployCommand : CommandBase
    {
        private readonly ILogger<DeployCommand> _logger;
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
        public DeployCommand( ILogger<DeployCommand> logger, IFileLoader loader, IIisDeployer deployer )
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
            IisColor? next = null;

            if ( this.BlueGreen == true )
            {
                var color = await _deployer.ColorGet();
                next = color == IisColor.Blue ? IisColor.Green : IisColor.Green;
            }


            /*
             * 
             */
            var definition = LoadDefinition( _loader, this.DefinitionFile );
            var config = LoadConfiguration( _loader, this.ConfigFile );


            /*
             * 
             */
            if ( next.HasValue == true )
                MutateDefinition( definition, next.Value );


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
            await _deployer.Deploy( definition, config );

            return 0;
        }
    }
}
