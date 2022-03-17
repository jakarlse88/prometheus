namespace UserService


#nowarn "20"


open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
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