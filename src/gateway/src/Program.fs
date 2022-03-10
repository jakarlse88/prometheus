namespace Gateway

#nowarn "20"

open Microsoft.AspNetCore.Authentication.JwtBearer
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Ocelot.DependencyInjection
open Ocelot.Middleware
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Hosting

module Program =
    open System.Threading.Tasks
    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)

        builder.Services.AddControllers()

        builder.Services.AddHttpClient()
        
        builder
            .Services
            .AddAuthentication( fun opt ->
                                    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme |> ignore
                                    opt.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme |> ignore )
            .AddJwtBearer( fun opt ->
                                opt.Authority = "https://soprom.eu.auth0.com/" |> ignore
                                opt.Audience  = "http://localhost:8004"        |> ignore )
        
        builder.Services.AddOcelot( builder.Configuration )
        
        let app = builder.Build()

        app.UseHttpsRedirection()

        app.UseAuthorization()

        app.UseRouting()

        app.UseOcelot().Wait()
        
        app.UseEndpoints( fun endPoints -> 
                            endPoints.MapGet( "/Ping", fun ctx -> ctx.Response.WriteAsync( "Pong" ) ) |> ignore ) |> ignore

        app.Run()

        exitCode
