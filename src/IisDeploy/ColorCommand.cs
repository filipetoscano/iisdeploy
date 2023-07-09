using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
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
            var current = LoadBlueGreen( definition );
            var next = current == "blue" ? "green" : "blue";

            _logger.LogInformation( "Current: {Current}", current );
            _logger.LogInformation( "Next: {Next}", next );

            return 1;
        }
    }
}
