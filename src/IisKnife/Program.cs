using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using Yttrium.IisDeploy;

namespace IisKnife
{
    /// <summary />
    [Command( "iisknife" )]
    [Subcommand( typeof( ApplyCommand ) )]
    [Subcommand( typeof( DeployCommand ) )]
    [Subcommand( typeof( GetCommand ) )]
    [Subcommand( typeof( StateCommand ) )]
    [HelpOption]
    [VersionOption( "1.0.0" )]
    public class Program
    {
        /// <summary />
        public static int Main( string[] args )
        {
            /*
             * Logging
             */
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console( outputTemplate: @"{Level:w3}: {Message:lj}{NewLine}{Exception}" )
                .CreateLogger();

            var logger = Log.Logger.ForContext<Program>();


            /*
             * Dependency Injection
             */
            var services = new ServiceCollection()
                .AddLogging( builder => builder.AddSerilog( dispose: true ) )
                .AddScoped<IIisDeployer, IisDeployer>()
                .BuildServiceProvider();


            /*
             * 
             */
            try
            {
                var app = new CommandLineApplication<Program>();

                app.Conventions
                    .UseDefaultConventions()
                    .UseConstructorInjection( services );

                return app.Execute( args );
            }
            catch ( UnrecognizedCommandParsingException  ex )
            {
                logger.Error( "{Message}", ex.Message );

                return 2;
            }
            catch ( IisException ex )
            {
                logger.Error( ex, "{Message}", ex.Message );

                return 1;
            }
            catch ( Exception ex )
            {
                logger.Fatal( ex, "Unhandled: {Message}", ex.Message );

                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }


        /// <summary />
        public int OnExecute( CommandLineApplication app )
        {
            app.ShowHelp();
            return 1;
        }
    }
}