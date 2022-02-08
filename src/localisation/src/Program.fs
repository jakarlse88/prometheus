namespace Localisation

#nowarn "20"

open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

module Program =
    let exitCode = 0
    
    let webApp =
        choose [
            GET >=> route  "/ping"          >=> text "Ok"
            GET >=> routef "/Language/%i" ( fun id -> getLanguageIdBy "" id )
        ]
        
    let configureApp ( app : IApplicationBuilder ) =
        app.UseGiraffe webApp
        
    let configureServices ( services : IServiceCollection ) =
        services.AddGiraffe() |> ignore
        
    [<EntryPoint>]
    let main args =
        Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(
                fun webHostBuilder ->
                    webHostBuilder
                        .Configure( configureApp )
                        .ConfigureServices( configureServices )
                        |> ignore )
            .Build()
            .Run()

        exitCode
