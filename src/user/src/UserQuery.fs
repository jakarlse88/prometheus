module private UserQuery


open Giraffe
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Caching.Memory
open Microsoft.Extensions.Configuration
open Polly
open System
open System.Net
open System.Net.Http
open System.Net.Http.Json
open Thoth.Json.Net
open UserService.Types


// ---------------------------------------------------------------------------------------------------------------------
//
//      Infrastructure
//
// ---------------------------------------------------------------------------------------------------------------------

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


// ---------------------------------------------------------------------------------------------------------------------
//
//      Functions
//
// ---------------------------------------------------------------------------------------------------------------------

let deserialize<'T> ( input : HttpResponseMessage ) ( decoder : Decoder<'T> ): Result<'T, DomainError> =
    Decode.fromString decoder ( input.Content.ReadAsStringAsync().Result )
    |> Result.mapError ( fun _ -> DeserializeError "Failed to deserialize response" )
    |> Result.bind     ( fun x -> Ok x )
    

let httpResponseToResult ( response : HttpResponseMessage ) =
    match response.StatusCode with
    | HttpStatusCode.OK           -> Ok    ( response )
    | HttpStatusCode.Unauthorized -> Error ( AuthError    response.ReasonPhrase )
    | _                           -> Error ( NetworkError response.ReasonPhrase )


let execHttpGetReq ( token : string ) ( url : string ) : Async<Result<HttpResponseMessage, DomainError>> =
    async {
        try 
            use client = new HttpClient()
            
            client.DefaultRequestHeaders.Add( "Authorization", "Bearer" + " " + token )

            let! res = waitAndRetryPolicy.Execute ( fun _ -> client.GetAsync( url ) |> Async.AwaitTask ) 

            return httpResponseToResult res
        with
        | :? Exception as ex ->
            return Error ( NetworkError ex.Message )
    } 


let execHttpPostReq( url : string ) ( body : Object ) : Async<Result<HttpResponseMessage, DomainError>> =
    async {
        try
            use client = new HttpClient()

            let! res = waitAndRetryPolicy.Execute ( fun _ -> client.PostAsJsonAsync( url, body ) |> Async.AwaitTask )
                        
            return httpResponseToResult res
        with
        | :? Exception as ex ->
            return Error ( NetworkError ex.Message )
    } 


let getAuthToken ( config : IConfiguration ) : Async<Result<Auth0TokenResponse, DomainError>> =
    async {
        let body = {| grant_type    = "client_credentials" 
                      client_id     = config[ "Auth0:ClientId" ] 
                      client_secret = config[ "Auth0:ClientSecret" ]
                      audience      = config[ "Auth0:ManagementApiAudience" ] |}
                
        let! res = execHttpPostReq
                    ( "https://" + config["Auth0:Domain"] + "/oauth/token" )
                    body

        match res with
        | Error err ->
            return Error err
        | Ok x ->
            return deserialize<Auth0TokenResponse> x Auth0TokenResponse.decoder
                   |> Result.mapError ( fun _ -> DeserializeError "Failed to deserialize response" )
    } 


let fetchUser ( config : IConfiguration ) ( token : AccessToken ) ( id : UserId )   =
    async {
        let req = AccessToken.value token |> execHttpGetReq  
        
        let! res = ( "https://" + config["Auth0:Domain"] + "/api/v2/users/" + id.ToString() )
                    |> req
        
        match res with
        | Error err ->
            return Error err
        | Ok x ->
            return deserialize<Auth0User> x Auth0UserResponse.decoder
                    |> Result.mapError ( fun _ -> DeserializeError "Failed to deserialize response" )
    }


// ---------------------------------------------------------------------------------------------------------------------
//
//      Workflow entrypoint
//
// ---------------------------------------------------------------------------------------------------------------------

let userQueryHandler ( userId : int ) : HttpHandler = 
    fun ( next : HttpFunc ) ( ctx : HttpContext ) ->
        let userId' = UserId.create ( userId.ToString() )
        
        match userId' with
        | Error err ->
            ServerErrors.INTERNAL_ERROR err next ctx
        | Ok id ->
            let tokenResult = getOrCreate<Auth0TokenResponse> 
                                ( ctx.GetService<IMemoryCache>() ) 
                                CacheKey.token
                                ( Absolute ( DateTimeOffset.Now + TimeSpan.FromSeconds ( 3600 ) ) ) 
                                ( ( ctx.GetService<IConfiguration>() ) |> getAuthToken )
                              |> Async.RunSynchronously
                        
            match tokenResult with
            | Error err ->
                ServerErrors.INTERNAL_ERROR err next ctx
            | Ok token ->
                let userRes = fetchUser
                                ( ctx.GetService<IConfiguration>() )
                                token.AccessToken
                                id
                              |> Async.RunSynchronously
                           
                match userRes with
                | Error err ->
                    ServerErrors.INTERNAL_ERROR err next ctx
                | Ok user ->
                    Successful.OK ( UserId.value user.UserId ) next ctx
