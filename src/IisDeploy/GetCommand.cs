using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Yttrium.IisDeploy;

namespace IisKnife
{
    /// <summary />
    [Command( "get", Description = "Views the current IIS configuration" )]
    public class GetCommand : CommandBase
    {
        private readonly ILogger<GetCommand> _logger;
        private readonly IIisDeployer _deployer;


        /// <summary />
        [Option( "-o|--output", CommandOptionType.SingleValue, Description = "Output filename, otherwise will write to console" )]
        public string OutputFilename { get; set; }


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


            /*
             * 
             */
            string output = JsonSerializer.Serialize( defn, new JsonSerializerOptions()
            {
                WriteIndented = true,
            } );


            /*
             * 
             */
            if ( this.OutputFilename != null )
            {
                Console.WriteLine( "Write to {0}...", this.OutputFilename );
                File.WriteAllText( this.OutputFilename, output );
            }
            else
            {
                Console.WriteLine( output );
            }

            return 0;
        }
    }
}