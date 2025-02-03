using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Threading.Tasks;
using Yttrium.IisDeploy;

namespace IisDeploy
{
    /// <summary />
    [Command( "color", Description = "For blue/green deployments, gets the current / next versions" )]
    public class ColorCommand : CommandBase
    {
        private readonly ILogger<ColorCommand> _logger;
        private readonly IIisDeployer _deployer;


        /// <summary />
        [Argument( 0, Description = "Name of deployment" )]
        [Required]
        public string DeploymentName { get; set; }

        /// <summary />
        [Option( "--json", CommandOptionType.NoValue, Description = "Output as JSON object" )]
        public bool AsJson { get; set; } = false;


        /// <summary />
        public ColorCommand( ILogger<ColorCommand> logger, IIisDeployer deployer )
        {
            _logger = logger;
            _deployer = deployer;
        }


        /// <summary />
        public async Task<int> OnExecuteAsync()
        {
            var curr = await _deployer.ColorGet( this.DeploymentName );
            var next = curr == DeploymentColor.Blue ? DeploymentColor.Green : DeploymentColor.Blue;

            if ( this.AsJson == true )
            {
                var obj = new
                {
                    Current = curr,
                    Next = next,
                };

                var json = JsonSerializer.Serialize( obj, new JsonSerializerOptions()
                {
                    WriteIndented = true,
                } );

                Console.WriteLine( json );
            }
            else
            {
                _logger.LogInformation( "Current: {Current}", curr );
                _logger.LogInformation( "Next: {Next}", next );
            }

            return 0;
        }
    }
}
