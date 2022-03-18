namespace Localisation


#nowarn "20"


open Giraffe
open Localisation.AddLanguageCommand
open Localisation.DeleteLanguageCommand
open Localisation.GetLanguageAllQuery
open Localisation.GetLanguageQuery
open Localisation.UpdateLanguageCommand
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting


module Program =
    let exitCode = 0
    
    let webApp =
        subRoute "/api"
            (
             choose [
                GET >=> choose [
                    route  "/ping"          >=> Successful.OK "pong"
                    routef "/Language/%i"       getLanguageByIdQueryHandler 
                    route  "/Language"      >=> getLanguageAllQueryHandler 
                ]   
                
                POST >=> choose [
                    route "/Language" >=> addLanguageCommandHandler
                ]

                PUT >=> choose [
                    routef "/Language/%i" updateLanguageCommandHandler
                ]

                DELETE >=> choose [
                    routef "/Language/%i" deleteLanguageCommandHandler
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
