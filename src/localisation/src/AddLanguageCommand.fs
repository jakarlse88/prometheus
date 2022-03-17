module private Localisation.AddLanguageCommand

open Dapper
open Giraffe
open Localisation.Domain
open Microsoft.AspNetCore.Http
open Microsoft.Data.SqlClient
open System


let ( => ) a b =
    a, box b


// --------------------------------------------------------------------------------
//
//      Input
//
// --------------------------------------------------------------------------------

type public InputModel = {
    Name      : string
    CreatedBy : int
    UpdatedBy : int
}


// --------------------------------------------------------------------------------
//
//      Validation
//
// --------------------------------------------------------------------------------

type ValidatedInput = {
    Name : ASCIIString
}


type ValidationError =
    | LanguageIdInvalidError     of string   
    | LanguageNameIdInvalidError of string   
    | InvalidUserIdError         of string
    | LanguageNameInvalidError   of string
    | CreatedByInvalidIdError    of string
    | UpdatedByInvalidIdError    of string
    | CreatedOnInvalidDateError  of string
    | UpdatedOnInvalidDateError  of string
    

type SQLError = SQLError of string


type UnifiedError =
    | ValidationResult
    | SQLError


let validateName input =
    match ASCIIString.create "Name" input with
    | Error err  -> ( LanguageNameInvalidError err ) |> ValidationResult.ofError
    | Ok    name -> Success name


let validateCreatedBy input =
    match int.create "CreatedBy" input with
    | Error err  -> ( InvalidUserIdError err ) |> ValidationResult.ofError
    | Ok    id   -> Success id

let validateInput ( input : InputModel ) =
    validateName input.Name

// --------------------------------------------------------------------------------
//
//      Output
//
// --------------------------------------------------------------------------------    

type UpdatedLanguageId = UpdatedLanguageId of LanguageId


type Payload = {
    LanguageId : UpdatedLanguageId
}

type LanguageCreatedEvent = LanguageCreatedEvent of Payload


let executeQuery connStr query data =
    async {
        let conn = new SqlConnection( connStr )
        do! conn.OpenAsync() |> Async.AwaitTask
        
        let! id = conn.ExecuteAsync( query, data ) |> Async.AwaitTask
        
        return id
    }
    
// --------------------------------------------------------------------------------
//
//      Workflow entrypoint
//
// --------------------------------------------------------------------------------

let public addLanguageCommandHandler ( input : InputModel ) : HttpHandler =
    fun ( next : HttpFunc ) ( ctx : HttpContext ) ->
        match validateInput input with
        | Success id ->
            
