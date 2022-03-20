module UserService.Types


open System
open System.Text.RegularExpressions
open Thoth.Json.Net


// ---------------------------------------------------------------------------------------------------------------------------
//
//      Error types
//
// ---------------------------------------------------------------------------------------------------------------------------

type DomainError =
    | NetworkError             of string
    | AuthError                of string
    | DeserializeError         of string
    | AccessTokenCreationError of string
    | UserIdCreationError      of string


// ---------------------------------------------------------------------------------------------------------------------------
//
//      Simple and constrained types
//
// ---------------------------------------------------------------------------------------------------------------------------

type AccessToken = private AccessToken of string
type UserId      = private UserId      of string


module AccessToken =
    
    let value ( AccessToken token ) =
        token
        
    let create ( str : string ) =
        if String.IsNullOrWhiteSpace( str ) then
            AccessTokenCreationError "Access token must be non-empty"
            |> Error
        
        elif Regex.IsMatch( "/^([a-zA-Z0-9_=]+)\.([a-zA-Z0-9_=]+)\.([a-zA-Z0-9_\-\+\/=]*)/", str ) = false then
            AccessTokenCreationError "Value was not a valid access token"
            |> Error
            
        else
            AccessToken str
            |> Ok
            
        
        
type Auth0User = {
    UserId : UserId    
}

// ---------------------------------------------------------------------------------------------------------------------------
//
//      Aggregate types
//
// ---------------------------------------------------------------------------------------------------------------------------

type Auth0TokenResponse = {
    AccessToken : AccessToken
    ExpiresIn   : int
    Scope       : string
    TokenType   : string
}

module Auth0TokenResponse =
    
    let decoder : Decoder<Auth0TokenResponse> =
        Decode.object ( fun get ->
            {
                AccessToken = get.Required.Raw ( Decode.field "access_token" Decode.string )
                ExpiresIn   = get.Required.Raw ( Decode.field "expires_in"   Decode.int )
                Scope       = get.Required.Raw ( Decode.field "scope"        Decode.string )
                TokenType   = get.Required.Raw ( Decode.field "token_type"   Decode.string )
            }
        )