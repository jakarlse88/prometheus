module Localisation.GetLanguageQuery


open Dapper
open Giraffe
open Localisation.Domain
open System
open System.Data
open Microsoft.AspNetCore.Http
open Microsoft.Data.SqlClient
open Microsoft.Extensions.Configuration


let inline ( => ) a b = 
    a, box b


let minDate = new DateTime( DateTime.Now.Year, 1, 1 )


let maxDate = DateTime.Today


/// Unverified intermediate DB type
type UnverifiedLanguage = {
    LanguageId : int
    Name       : string
    
    CreatedBy : int
    CreatedOn : DateTime 
    UpdatedBy : int
    UpdatedOn : DateTime
}


type ValidationError =
    | LanguageIdInvalidError    of string   
    | InvalidUserIdError        of string
    | LanguageNameInvalidError  of string
    | CreatedByInvalidIdError   of string
    | UpdatedByInvalidIdError   of string
    | CreatedOnInvalidDateError of string
    | UpdatedOnInvalidDateError of string


type InfrastructureError = 
    | SQLError of string


type UnifiedError =
    | ValidationError
    | InfrastructureError

type LanguageDto = {
    LanguageId : int
    Name       : string
    
    CreatedOn  : string
    CreatedBy  : int
    UpdatedOn  : string
    UpdatedBy  : int
    }

type LanguageErrorDto = {
    Code    : string
    Message : string list
    }

let verifyLanguageId input =
    match LanguageId.create input with
    | Error err -> ( LanguageIdInvalidError err ) |> ValidationResult.ofError
    | Ok id     -> Success id


let verifyUserId input = 
    match UserId.create input with
    | Error err -> ( InvalidUserIdError err ) |> ValidationResult.ofError
    | Ok id     -> Success id


let verifyLanguageName input =
    match LanguageName.create input with
    | Error err -> ( LanguageNameInvalidError err ) |> ValidationResult.ofError
    | Ok str    -> Success str


let verifyCreatedBy input =
    match UserId.create input with
    | Error err -> ( CreatedByInvalidIdError err ) |> ValidationResult.ofError
    | Ok id     -> Success id


let verifyCreatedOn input  =
    match ConstrainedDate.createFromDateTime minDate maxDate input with
    | Error err -> ( CreatedOnInvalidDateError err ) |> ValidationResult.ofError
    | Ok date   -> Success date


let verifyUpdatedBy input =
    match UserId.create input with
    | Error err -> ( UpdatedByInvalidIdError err ) |> ValidationResult.ofError
    | Ok id     -> Success id


let verifyUpdatedOn input =
    match ConstrainedDate.createFromDateTime minDate maxDate input with
    | Error err -> ( UpdatedOnInvalidDateError err ) |> ValidationResult.ofError
    | Ok date   -> Success date

let toDto ( language : Language ) = {
    LanguageId =   LanguageId.value       language.LanguageId; 
    Name       =   LanguageName.value     language.Name; 
    CreatedBy  =   UserId.value           language.CreatedBy;
    CreatedOn  = ( ConstrainedDate.value  language.CreatedOn ).ToIsoString(); 
    UpdatedBy  =   UserId.value           language.UpdatedBy; 
    UpdatedOn  = ( ConstrainedDate.value  language.UpdatedOn ).ToIsoString();
}

let toLanguageEntity id name createdBy createdOn updatedBy updatedOn : Language = {
        LanguageId = id; 
        Name       = name; 
        CreatedBy  = createdBy;
        CreatedOn  = createdOn; 
        UpdatedBy  = updatedBy; 
        UpdatedOn  = updatedOn;
    }


let validateLanguageEntityIntegrity ( input : UnverifiedLanguage ) =
    let ( <!> ) = ValidationResult.map
    let ( <*> ) = ValidationResult.apply
    
    let idResult        = verifyLanguageId   input.LanguageId
    let nameResult      = verifyLanguageName input.Name
    let createdByResult = verifyCreatedBy    input.CreatedBy
    let createdOnResult = verifyCreatedOn    input.CreatedOn
    let updatedByResult = verifyUpdatedBy    input.UpdatedBy
    let updatedOnResult = verifyUpdatedOn    input.UpdatedOn

    toLanguageEntity
    <!> idResult
    <*> nameResult
    <*> createdByResult
    <*> createdOnResult
    <*> updatedByResult
    <*> updatedOnResult


let private query =
    @"SELECT
        [LanguageId], [Name], [NameLocal], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn]
    FROM
        [dbo].[Language]
    WHERE
        [LanguageId] = @id
    "
    
let private queryParams id =
    [
        "id" => id
    ]


let private mapRowsToRecords ( reader : IDataReader ) : UnverifiedLanguage list =
    let languageIdIndex     = reader.GetOrdinal "LanguageId"
    let nameInvariantIndex  = reader.GetOrdinal "NameInvariant"
    let createdByIndex      = reader.GetOrdinal "CreatedBy"
    let createdOnIndex      = reader.GetOrdinal "CreatedOn"
    let updatedByIndex      = reader.GetOrdinal "UpdatedBy"
    let updatedOnIndex      = reader.GetOrdinal "UpdatedOn"
    
    [
        while reader.Read() do
            yield {
                LanguageId    = reader.GetInt32  languageIdIndex
                Name = reader.GetString nameInvariantIndex
                    
                CreatedBy = reader.GetInt32     createdByIndex
                CreatedOn = reader.GetDateTime  createdOnIndex
                UpdatedBy = reader.GetInt32     updatedByIndex
                UpdatedOn = reader.GetDateTime  updatedOnIndex
            }
    ]


let private executeQuery ( connStr : string ) ( query : string ) data =
    async {
        let conn = new SqlConnection( connStr )
        
        use! reader = conn.ExecuteReaderAsync( query, data ) |> Async.AwaitTask
        
        return mapRowsToRecords reader |> Seq.tryHead
    }


let getLanguageById ( connStr : string ) ( id : int ) : ValidationResult<Language, UnifiedError> =
    try
        let y = executeQuery connStr query ( queryParams id ) |> Async.RunSynchronously
    
        match y with
        | Some  lang -> validateLanguageEntityIntegrity lang
        | None  _    -> Failure err
    with
    | :? SqlException as ex -> 
        Error ex.Message
    

let getLanguageByIdHandler ( id : int ) : HttpHandler =
    fun ( next : HttpFunc ) ( ctx : HttpContext ) ->
        let config  = ctx.GetService<IConfiguration>()
        let langRes = getLanguageById ( config.GetConnectionString( "DefaultConnection" ) ) id

        match langRes with
        | Success lang -> Successful.OK ( toDto lang ) |> next ctx
        | Failure err  -> err  |> toErrorDto |> RequestErrors.unprocessableEntity