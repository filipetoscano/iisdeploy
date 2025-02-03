using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
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
        [Option( "-o|--output", CommandOptionType.SingleValue, Description = "Output filename, otherwise will write to console" )]
        public string OutputFilename { get; set; }

        /// <summary />
        [Option( "-f|--format", CommandOptionType.SingleValue, Description = "Output format" )]
        public Format? OutputFormat { get; set; }


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
            if ( this.OutputFilename != null && this.OutputFormat == null )
            {
                if ( Path.GetExtension( this.OutputFilename ) == ".xml" )
                    this.OutputFormat = Format.Xml;
                else
                    this.OutputFormat = Format.Json;
            }


            /*
             * 
             */
            string output;

            if ( this.OutputFormat == Format.Xml )
            {
                var ser = new XmlSerializer( typeof( DeploymentDefinition ) );
                var sb = new StringBuilder();

                using ( var xs = new StringWriter( sb ) )
                {
                    ser.Serialize( xs, defn );
                }

                output = sb.ToString();
            }
            else
            {
                output = JsonSerializer.Serialize( defn, new JsonSerializerOptions()
                {
                    WriteIndented = true,
                } );
            }


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