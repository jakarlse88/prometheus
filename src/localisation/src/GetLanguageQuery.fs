module Localisation.GetLanguageQuery

open Dapper
open Localisation.Domain
open Microsoft.Data.SqlClient
open System.Data

let inline ( => ) a b = 
    a, box b

// bind lets us compose monadic functions
let bind switchFn input =
    match input with 
    | Ok s    -> switchFn s
    | Error f -> Error f

// Infix for bind
let ( >>= ) input switchFn =
    bind switchFn input

// Kleisli lets us compose two 
let ( >=> ) fnA fnB =
    match fnA with
    | Ok a    -> fnB a
    | Error e -> Error e


type ValidationError =
    | LanguageIdNegativeError          
    | LanguageIdZeroError
    | InvalidUserIdError          
    | NameEmptyError               
    | NameTooLongError             
    | NameContainsIllegalCharsError of string
    | DatePrecedesMinimum           of string
    | DateSucceedsMaximum           of string


let idNotNegative ( input : UnverifiedLanguage ) =
    if input.LanguageId < 0 then
         Error LanguageIdNegativeError
    else
        Ok input

let idNotZero ( input : UnverifiedLanguage ) =
    if input.LanguageId = 0 then
        Error LanguageIdNegativeError
    else
        Ok input

let testFunction ( input : int ) =
    if input = 0 then
        Error LanguageIdZeroError
    else
        Ok input

let toLanguageEntity ( input : UnverifiedLanguage ) =
    result {
        let! languageId =
            input.LanguageId
            |> LanguageId.create
            |> Result.mapError        
    }
    // let {
    //        id        = LanguageId;
    //        invariant = NameInvariant;
    //        local     = NameLocal;
    //        cBy       = CreatedBy;
    //        cOn       = CreatedOn;
    //        uBy       = UpdatedBy;
    //        uOn       = UpdatedOn; } = unverifiedLang
    
    // let id        = EntityId.create "languageId" id
    // let invariant = ASCIIName.create invariant
    // let local     = ValidateName "" "" local
    // let createdBy = EntityId.create "createdBy" cBy
    // let updatedOn = ConstrainedDate.createFromDateTime cOn DateTime.MinValue DateTime.MaxValue
    // let updatedBy = EntityId.create "createdBy" uBy
    // let updatedOn = ConstrainedDate.createFromDateTime uOn DateTime.MinValue DateTime.MaxValue



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
                NameInvariant = reader.GetString nameInvariantIndex
                    
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


let getLanguageById ( connStr : string ) ( id : int ) =
    let y = executeQuery connStr query ( queryParams id )
            |> Async.RunSynchronously
            
    match y with
    | Some  lang ->
    | None       -> Error err
    