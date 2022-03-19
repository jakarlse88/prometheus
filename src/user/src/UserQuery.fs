module private UserQuery


open Giraffe
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open Polly
open System
open System.Net
open System.Net.Http
open System.Net.Http.Json
open Thoth.Json.Net
open Microsoft.Extensions.Caching.Memory


// ---------------------------------------------------------------------------------------------------------------------------
//
//      Infrastructure
//
// ---------------------------------------------------------------------------------------------------------------------------

module CacheKey =

    let token = 
        "token"


type CacheExpiration =
    | Absolute of DateTimeOffset
    | Sliding  of TimeSpan


let getOrCreate<'T> ( cache : IMemoryCache ) ( key : string ) ( expiration : CacheExpiration ) fn  =
    cache.GetOrCreateAsync( key, fun entry -> 
        task {
            match expiration with
            | Absolute dto -> entry.AbsoluteExpiration <- Nullable dto
            | Sliding  ts  -> entry.SlidingExpiration  <- Nullable ts

            let! payload = fn |> Async.StartAsTask

            return payload

        }
    ) |> Async.AwaitTask


let waitAndRetryPolicy =
    Policy
        .Handle<HttpRequestException>()
        .WaitAndRetry [
            TimeSpan.FromSeconds 1.
            TimeSpan.FromSeconds 3.
            TimeSpan.FromSeconds 5.
            TimeSpan.FromSeconds 7.
        ]



// ---------------------------------------------------------------------------------------------------------------------------
//
//      Error types
//
// ---------------------------------------------------------------------------------------------------------------------------

type UserQueryError =
    | NetworkError      of string
    | AuthError         of string
    | DeserializeError  of string


type Auth0Response = {
    access_token : string
    expires_in   : int
    scope        : string
    token_type   : string
}


// ---------------------------------------------------------------------------------------------------------------------------
//
//      Workflow-specific types
//
// ---------------------------------------------------------------------------------------------------------------------------

module Auth0Response =
    
    let decoder : Decoder<Auth0Response> =
        Decode.object ( fun get ->
            {
                access_token = get.Required.Raw ( Decode.field "access_token" Decode.string )
                expires_in   = get.Required.Raw ( Decode.field "expires_in"   Decode.int )
                scope        = get.Required.Raw ( Decode.field "scope"        Decode.string )
                token_type   = get.Required.Raw ( Decode.field "token_type"   Decode.string )
            }
        )   


// ---------------------------------------------------------------------------------------------------------------------------
//
//      Functions
//
// ---------------------------------------------------------------------------------------------------------------------------


let deserialize<'T> ( input : HttpResponseMessage ) ( decoder : Decoder<'T> ): Result<'T, UserQueryError> =
    Decode.fromString decoder ( input.Content.ReadAsStringAsync().Result )
    |> Result.mapError ( fun _ -> DeserializeError "Failed to deserialize response" )
    |> Result.bind     ( fun x -> Ok x )
    

let executeHttpGetReq ( url : string ) : Async<Result<HttpResponseMessage, UserQueryError>> =
    async {
        try 
            use client = new HttpClient()

            let! res = client.GetAsync( url ) |> Async.AwaitTask 

            return Ok ( res )
        with
        | :? Exception as ex ->
            return Error ( NetworkError ex.Message )
    } 

let executeHttpPostReq ( url : string ) ( body : Object ) : Async<Result<HttpResponseMessage, UserQueryError>> =
    async {
        use client = new HttpClient()

        let! res = client.PostAsJsonAsync( url, body ) |> Async.AwaitTask

        return res
    } 


let getAuthToken ( config : IConfiguration ) : Async<Result<Auth0Response, string>> =
    async {
        use client = new HttpClient()
        
        let body = {| grant_type    = "client_credentials" 
                      client_id     = config[ "Auth0:ClientId" ] 
                      client_secret = config[ "Auth0:ClientSecret" ]
                      audience      = config[ "Auth0:ManagementApiAudience" ] |}
        
        let! res =
            let url = "https://" + config["Auth0:Domain"] + "/oauth/token"
            
            client.PostAsJsonAsync( url, body ) |> Async.AwaitTask

        let returnVal = match res.StatusCode with
                        | HttpStatusCode.OK           -> Ok    res
                        | HttpStatusCode.Unauthorized -> Error res.ReasonPhrase
                        | _                           -> Error "Unexpected response from Auth0"
                        
        return returnVal // async.map? fstools?
    } 


let fetchUser ( config : IConfiguration ) ( token : string ) ( id : int ) =
    task {
        use client = new HttpClient()
        
        let! res = client.GetAsync( "https://" + config["Auth0:Domain"] + "/api/v2/users/" + id.ToString() )
    }


// ---------------------------------------------------------------------------------------------------------------------------
//
//      Workflow entrypoint
//
// ---------------------------------------------------------------------------------------------------------------------------

let userQueryHandler ( userId : int ) : HttpHandler = 
    fun ( next : HttpFunc ) ( ctx : HttpContext ) ->
        let cache = ctx.GetService<IMemoryCache>()

        let token = getOrCreate<Auth0Response> 
                        cache 
                        CacheKey.token
                        ( Absolute ( DateTimeOffset.Now + TimeSpan.FromSeconds ( 3600 ) ) ) 
                        ( ( ctx.GetService<IConfiguration>() ) |> getAuthToken )

        match auth' with
        | Error _ -> ServerErrors.INTERNAL_ERROR "Couldn't auhorise with Auth0" next ctx
        | Ok    x -> Successful.OK               x next ctx
