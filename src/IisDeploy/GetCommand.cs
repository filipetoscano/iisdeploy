using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Yttrium.IisDeploy;

namespace IisDeploy
{
    /// <summary />
    [Command( "get", Description = "Views the current IIS configuration" )]
    public class GetCommand : CommandBase
    {
        private readonly ILogger<GetCommand> _logger;
        private readonly IIisDeployer _deployer;


        /// <summary />
        [Option( "-v|--verbose", CommandOptionType.NoValue, Description = "Verbose output" )]
        public bool Verbose { get; set; }


        /// <summary />
        public GetCommand( ILogger<GetCommand> logger, IIisDeployer deployer )
        {
            _logger = logger;
            _deployer = deployer;
        }


        /// <summary />
        public async Task<int> OnExecuteAsync()
        {
            var defn = await _deployer.Get();

            var json = JsonSerializer.Serialize( defn, new JsonSerializerOptions()
            {
                WriteIndented = true,
            } );

            Console.WriteLine( json );

            return 0;
        }
    }
}
