using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using Yttrium.IisDeploy;

namespace IisDeploy
{
    /// <summary />
    [Command( "iisdeploy" )]
    [Subcommand( typeof( ColorCommand ) )]
    [Subcommand( typeof( ConfigureCommand ) )]
    [Subcommand( typeof( DeployCommand ) )]
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
                .WriteTo.Console()
                .CreateLogger();

            var logger = Log.Logger.ForContext<Program>();


            /*
             * Dependency Injection
             */
            var services = new ServiceCollection()
                .AddLogging( builder => builder.AddSerilog( dispose: true ) )
                .AddScoped<IIisDeployer, IisDeployer>()
                .AddScoped<IFileLoader, FileLoader>()
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
