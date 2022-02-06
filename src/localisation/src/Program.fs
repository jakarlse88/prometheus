namespace Localisation

#nowarn "20"

open Giraffe
open Localisation
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.HttpsPolicy
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Threading.Tasks

module Program =
    let exitCode = 0
    
    let webApp =
        choose [
            route "/ping" >=> text "Ok"
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
