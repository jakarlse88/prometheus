namespace UserService


#nowarn "20"


open System
open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Serilog
open Serilog.Events
open Serilog.Exceptions
open UserQuery


module Program =
    let exitCode = 0
    
    
    let webApp = 
        subRoute "/api" (
            choose [
                GET >=> choose [
                    routef "/User/%i" userQueryHandler
                ]

                RequestErrors.NOT_FOUND "Could not find a matching route"
            ]
        )

    
    let errorHandler ( ex : Exception ) ( _ : Microsoft.Extensions.Logging.ILogger ) =
        Log.Error( ex, "An unhandled exception occurred while processing the request" )

        clearResponse
        >=> ServerErrors.INTERNAL_ERROR ex.Message

    
    let configureApp ( app : IApplicationBuilder ) =
        app
            .UseGiraffeErrorHandler( errorHandler )
            .UseSerilogRequestLogging()
            .UseGiraffe webApp
        
        
    let configureServices ( services : IServiceCollection ) =
        services
            .AddGiraffe()
            |> ignore
        
    
    Log.Logger = LoggerConfiguration()
            .MinimumLevel.Override( "Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();    
    
        
    [<EntryPoint>]
    let main _ =
                Host.CreateDefaultBuilder()
                    .ConfigureWebHostDefaults(
                        fun webHostBuilder ->
                            webHostBuilder
                                .Configure( configureApp )
                                .ConfigureServices( configureServices )
                            |> ignore
                        )
                    .ConfigureLogging( fun config   -> config.ClearProviders() |> ignore )
                    .UseSerilog(       fun ctx conf -> conf.ReadFrom.Configuration( ctx.Configuration) |> ignore )
                    .Build()
                    .Run()
        
                exitCode
