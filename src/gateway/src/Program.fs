namespace Gateway

#nowarn "20"

open Microsoft.AspNetCore.Authentication.JwtBearer
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.IdentityModel.Tokens
open Ocelot.Cache.CacheManager
open Ocelot.DependencyInjection
open Ocelot.Middleware
open System.Security.Claims

module Program =
    
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
                                opt.Authority = builder.Configuration["Auth0:Domain"]                   |> ignore
                                opt.Audience  = builder.Configuration["Auth0:Audience"]                 |> ignore
                                opt.TokenValidationParameters = TokenValidationParameters( )            |> ignore
                                opt.TokenValidationParameters.NameClaimType = ClaimTypes.NameIdentifier |> ignore )
        
        builder.Services
            .AddOcelot( builder.Configuration )
            .AddCacheManager( fun x -> x.WithDictionaryHandle() |> ignore )
        
        let app = builder.Build()

        app.UseHttpsRedirection()

        app.UseAuthentication()
        
        app.UseAuthorization()

        app.UseRouting()

        app.UseOcelot().Wait()
        
        app.UseEndpoints( fun endPoints -> 
                            endPoints.MapGet( "/Ping", fun ctx -> ctx.Response.WriteAsync( "Pong" ) ) |> ignore ) |> ignore

        app.Run()

        exitCode
