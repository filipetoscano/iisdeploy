using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Yttrium.IisDeploy;

namespace IisDeploy
{
    /// <summary />
    [Command( "color", Description = "For blue/green deployments, gets the current / next versions" )]
    public class ColorCommand : CommandBase
    {
        private readonly ILogger<ColorCommand> _logger;
        private readonly IFileLoader _loader;


        /// <summary />
        [Argument( 0, Description = "Definition file (XML/JSON)" )]
        [FileExists]
        [Required]
        public string DefinitionFile { get; set; }

        /// <summary />
        [Option( "-v|--verbose", CommandOptionType.NoValue, Description = "Verbose output" )]
        public bool Verbose { get; set; }


        /// <summary />
        public ColorCommand( ILogger<ColorCommand> logger, IFileLoader loader )
        {
            _logger = logger;
            _loader = loader;
        }


        /// <summary />
        public int OnExecute()
        {
            /*
             * 
             */
            var definition = LoadDefinition( _loader, this.DefinitionFile );


            /*
             * 
             */
            if ( this.Verbose == true )
            {
                var jso = new JsonSerializerOptions() { WriteIndented = true };
                var def = JsonSerializer.Serialize( definition, jso );

                _logger.LogDebug( "Definition: {Definition}", def );
            }


            /*
             * 
             */
            var current = LoadBlueGreen( definition );
            var next = current == "blue" ? "green" : "blue";

            _logger.LogInformation( "Root: {Path}", definition.RootPhysicalPath );
            _logger.LogInformation( "Current: {Current}", current );
            _logger.LogInformation( "Next deployment: {Next}", next );

            return 1;
        }
    }
}
