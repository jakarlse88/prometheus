namespace Localisation

#nowarn "20"

open Giraffe
open Localisation.GetLanguageQuery
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

module Program =
    let exitCode = 0
    
    let webApp =
        subRoute "/api"
            (
             GET >=> choose [
                route  "/ping"          >=> Successful.OK "pong"
                routef "/Language/%i"       getLanguageByIdHandler 

                RequestErrors.NOT_FOUND "Could not find a route matching the specified route"
            ]
        )
        
    let configureApp ( app : IApplicationBuilder ) =
        app.UseGiraffe webApp
        
    let configureServices ( services : IServiceCollection ) =
        services.AddGiraffe() |> ignore
        
    [<EntryPoint>]
    let main _ =
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
