module private UserQuery


open Giraffe
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open System.Net.Http
open System


let auth ( config : IConfiguration ) =
    task {
        let req = new HttpRequestMessage( HttpMethod.Get, config["Auth0:Domain"] + "/oauth/token" )
    
        req.Headers.Add( "content-type", "application/x-www-form-urlencoded" )

        let content = [ "grant_type"    , "client_credentials" 
                      ; "client_id"     , config["Auth0:ClientId" ] 
                      ; "client_secret" , config["Auth0:ClientSecret"]
                      ; "audience"      , config["Auth0:Audience" + "/api/v2/" ] 
                      ] |> dict

        req.Content <- new FormUrlEncodedContent( content ) 

        let client = new HttpClient()

        let! res = client.SendAsync( req ) 

        return res
    } |> Async.AwaitTask
      |> Async.RunSynchronously


let userQueryHandler ( userId : int ) : HttpHandler = 
    fun ( next : HttpFunc ) ( ctx : HttpContext ) ->
        let auth = ctx.GetService<IConfiguration>() |> auth 

        Console.WriteLine( auth )
        
        Successful.OK 1 next ctx