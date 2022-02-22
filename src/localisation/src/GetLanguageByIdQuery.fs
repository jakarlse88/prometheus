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


type UnverifiedLanguage = {
    LanguageId : int
    Name       : string
    
    CreatedBy : int
    CreatedOn : DateTime 
    UpdatedBy : int
    UpdatedOn : DateTime
}


type ErrorType =
    | LanguageIdInvalidError     of string   
    | LanguageNameIdInvalidError of string   
    | InvalidUserIdError         of string
    | LanguageNameInvalidError   of string
    | CreatedByInvalidIdError    of string
    | UpdatedByInvalidIdError    of string
    | CreatedOnInvalidDateError  of string
    | UpdatedOnInvalidDateError  of string
    | SQLError                   of string


type LanguageDto = {
    LanguageId : int
    Name       : string
    
    CreatedOn  : string
    CreatedBy  : int
    UpdatedOn  : string
    UpdatedBy  : int
    }


let verifyLanguageId input =
    match LanguageId.create "LanguageId" input with
    | Error err -> ( LanguageIdInvalidError err ) |> ValidationResult.ofError
    | Ok    id  -> Success id
    

let verifyLanguageName input =
    match ASCIIString.create "Name" input with
    | Error err -> ( LanguageNameInvalidError err ) |> ValidationResult.ofError
    | Ok    str -> Success str


let verifyCreatedBy input =
    match UserId.create "CreatedBy" input with
    | Error err -> ( CreatedByInvalidIdError err ) |> ValidationResult.ofError
    | Ok    id  -> Success id


let verifyCreatedOn input  =
    match ConstrainedDate.createFromDateTime minDate maxDate input with
    | Error err -> ( CreatedOnInvalidDateError err ) |> ValidationResult.ofError
    | Ok date   -> Success date


let verifyUpdatedBy input =
    match UserId.create "UpdatedBy" input with
    | Error err -> ( UpdatedByInvalidIdError err ) |> ValidationResult.ofError
    | Ok id     -> Success id


let verifyUpdatedOn input =
    match ConstrainedDate.createFromDateTime minDate maxDate input with
    | Error err -> ( UpdatedOnInvalidDateError err ) |> ValidationResult.ofError
    | Ok date   -> Success date


let toLanguageDto ( language : Language ) = {
    LanguageId =   LanguageId.value      language.LanguageId; 
    Name       =   ASCIIString.value     language.Name 
    CreatedBy  =   UserId.value          language.CreatedBy;
    CreatedOn  = ( ConstrainedDate.value language.CreatedOn ).ToIsoString(); 
    UpdatedBy  =   UserId.value          language.UpdatedBy; 
    UpdatedOn  = ( ConstrainedDate.value language.UpdatedOn ).ToIsoString();
}


let toLanguageEntity id names createdBy createdOn updatedBy updatedOn : Language = {
        LanguageId = id
        Name      = names
        CreatedBy  = createdBy
        CreatedOn  = createdOn 
        UpdatedBy  = updatedBy 
        UpdatedOn  = updatedOn
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
        [LanguageId], [Name], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn]
    FROM
        [dbo].[Language]
    WHERE
        [LanguageId] = @id
    "
    
    
let private queryParams id =
    dict [
        "id" => id
    ]


let private mapRowsToRecords ( reader : IDataReader ) : UnverifiedLanguage list =
    let languageIdIndex    = reader.GetOrdinal "LanguageId"
    let nameInvariantIndex = reader.GetOrdinal "Name"
    let createdByIndex     = reader.GetOrdinal "CreatedBy"
    let createdOnIndex     = reader.GetOrdinal "CreatedOn"
    let updatedByIndex     = reader.GetOrdinal "UpdatedBy"
    let updatedOnIndex     = reader.GetOrdinal "UpdatedOn"
    
    [
        while reader.Read() do
            yield {
                LanguageId = reader.GetInt32    languageIdIndex
                Name       = reader.GetString   nameInvariantIndex
                CreatedBy  = reader.GetInt32    createdByIndex
                CreatedOn  = reader.GetDateTime createdOnIndex
                UpdatedBy  = reader.GetInt32    updatedByIndex
                UpdatedOn  = reader.GetDateTime updatedOnIndex
            }
    ]


let private executeQuery ( connStr : string ) ( query : string ) data =
    async {
        let conn = new SqlConnection( connStr )
        do! conn.OpenAsync() |> Async.AwaitTask
        
        use! reader = conn.ExecuteReaderAsync( query, data ) |> Async.AwaitTask
        
        return mapRowsToRecords reader |> Seq.tryHead
    }


let public getLanguageByIdQueryHandler ( id : int ) : HttpHandler =
    fun ( next : HttpFunc ) ( ctx : HttpContext ) ->
        try
            let connStr = ctx.GetService<IConfiguration>().GetValue("DefaultConnection")
            let langRes = executeQuery connStr query ( queryParams id )
                          |> Async.RunSynchronously

            match langRes with
            | None _ ->
                RequestErrors.NOT_FOUND ( sprintf "Could not find a 'Language' record with the identifier '%i'" id ) next ctx
            | Some res ->
                match validateLanguageEntityIntegrity res with
                | Success lang -> Successful.OK            ( toLanguageDto lang ) next ctx
                | Failure errs -> RequestErrors.BAD_REQUEST  errs                 next ctx
        with
        | :? SqlException as ex -> 
            ServerErrors.INTERNAL_ERROR ex.Message next ctx