module private Localisation.AddLanguageCommand

open System
open Giraffe
open Giraffe.HttpStatusCodeHandlers
open Localisation.Domain
open Microsoft.AspNetCore.Http
open System.Net.Http


let ( => ) a b =
    a, box b


// --------------------------------------------------------------------------------
//
//      Input
//
// --------------------------------------------------------------------------------

type InputModel = {
    Name      : string
    CreatedBy : int
    CreatedOn : DateTime
    UpdatedBy : int
    UpdatedOn : DateTime
}


// --------------------------------------------------------------------------------
//
//      Validation
//
// --------------------------------------------------------------------------------

type ValidatedInput = {
    Name : ASCIIString
    
    CreatedBy : UserId
    CreatedOn : ConstrainedDate
    UpdatedBy : UserId
    UpdatedOn : ConstrainedDate
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


type ValidateName =
    string -> ValidationResult<ASCIIString, ValidationError>
    

type ValidateCreatedBy =
    int -> ValidationResult<UserId, ValidationError>
    

type ValidateCreatedOn =
    DateTime -> ValidationResult<ConstrainedDate, ValidationError>
    

type ValidateUpdatedBy =
    int -> ValidationResult<UserId, ValidationError>
    
    
type ValidateUpdatedOn =
    DateTime -> ValidationResult<ConstrainedDate, ValidationError>
    
    
type ValidateInput =
    InputModel
        -> ValidateName
        -> ValidateCreatedBy
        -> ValidateCreatedOn
        -> ValidateUpdatedBy
        -> ValidateUpdatedOn
        -> ValidationResult<ValidatedInput, ValidationError>


// --------------------------------------------------------------------------------
//
//      Output
//
// --------------------------------------------------------------------------------    

type ExtantLanguageId = ExtantLanguageId of LanguageId


type Payload = {
    LanguageId : ExtantLanguageId
}

type LanguageCreatedEvent = LanguageCreatedEvent of Payload

    
// --------------------------------------------------------------------------------
//
//      Workflow entrypoint
//
// --------------------------------------------------------------------------------

let public addLanguageCommandHandler : HttpHandler =
    fun ( next : HttpFunc ) ( ctx : HttpContext ) ->
        Successful.OK "POST" next ctx
