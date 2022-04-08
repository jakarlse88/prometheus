module UserService.Types


open System
open System.Text.RegularExpressions
open Thoth.Json.Net

// ---------------------------------------------------------------------------------------------------------------------
//
//      Simple and constrained types
//
// ---------------------------------------------------------------------------------------------------------------------

type DomainError =
    | NetworkError             of string
    | AuthError                of string
    | DeserializeError         of string
    | AccessTokenCreationError of string
    | UserIdCreationError      of string


type AccessToken = private AccessToken of string
type UserId      = private UserId      of string


module AccessToken =
    
    let value ( AccessToken token ) =
        token
        
    let create ( str : string ) =
        if String.IsNullOrWhiteSpace( str ) then
            AccessTokenCreationError "`AccessToken` cannot be empty"
            |> Error
        
        elif Regex.IsMatch( "^([a-zA-Z0-9_=]+)\.([a-zA-Z0-9_=]+)\.([a-zA-Z0-9_\-\+\/=]*)", str ) = false then
            AccessTokenCreationError "Value was not a valid access token"
            |> Error
            
        else
            AccessToken str
            |> Ok
            

module UserId =
    
    let value ( UserId id ) =
        id
        
    let create ( str : string ) =
        if String.IsNullOrWhiteSpace( str ) then
            UserIdCreationError "`UserId` cannot be empty"
            |> Error
        
        elif Regex.IsMatch( "^auth0\|[a-zA-Z0-9]+$" , str ) = false then
            UserIdCreationError "Value was not a valid user id"
            |> Error
            
        else
            UserId str
            |> Ok
        
            
            
// ---------------------------------------------------------------------------------------------------------------------
//
//      JSON Decoders
//
// ---------------------------------------------------------------------------------------------------------------------

module Decode =
    
    let timestamp ( date : DateTime ) =
        DateTimeOffset( date ).ToUnixTimeSeconds()
        |> box
    
    let authToken : Decoder<AccessToken> =
        fun path value ->
            if Decode.Helpers.isString value then
                let value' : string = unbox value
                
                match AccessToken.create value' with
                | Error err ->
                    match err with
                    | AccessTokenCreationError err ->
                        Decode.fail err path value
                    | _     ->
                        Decode.fail "Unexpected error" path value
                | Ok    token -> Ok token
            
            else
                ( path, BadPrimitive( "string", value ) ) |> Error
                
    
    let userId : Decoder<UserId> =
        fun path value ->
            if Decode.Helpers.isString value then
                let value' : string = unbox value
                
                match UserId.create value' with
                | Error err ->
                    match err with
                    | UserIdCreationError err ->
                        Decode.fail err path value
                    | _     ->
                        Decode.fail "Unexpected error" path value
                | Ok    id -> Ok id
            
            else
                ( path, BadPrimitive( "string", value ) ) |> Error
                

// ---------------------------------------------------------------------------------------------------------------------
//
//      Aggregate types
//
// ---------------------------------------------------------------------------------------------------------------------

type Auth0TokenResponse = {
    AccessToken : AccessToken
    ExpiresIn   : int
    Scope       : string
    TokenType   : string
}


type Auth0User = {
    UserId : UserId    
}


module Auth0TokenResponse =
    
    let decoder : Decoder<Auth0TokenResponse> =
        Decode.object ( fun get ->
            {
                AccessToken = get.Required.Raw ( Decode.field "access_token" Decode.authToken ) 
                ExpiresIn   = get.Required.Raw ( Decode.field "expires_in"   Decode.int )
                Scope       = get.Required.Raw ( Decode.field "scope"        Decode.string )
                TokenType   = get.Required.Raw ( Decode.field "token_type"   Decode.string )
            }
        )
        

module Auth0UserResponse =
    
    let decoder : Decoder<Auth0User> =
        Decode.object ( fun get ->
            {
                UserId = get.Required.Raw ( Decode.field "user_id" Decode.userId )
            }
        )
        
        
        