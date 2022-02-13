﻿module Localisation.GetLanguageQuery

open Dapper
open Localisation.Domain
open Microsoft.Data.SqlClient
open System.Data

let inline ( => ) a b = a, box b

type ValidationError =
    | InvalidLanguageIdError          
    | InvalidUserIdError          
    | NameEmptyError               
    | NameTooLongError             
    | NameContainsIllegalCharsError of string
    | DatePrecedesMinimum           of string
    | DateSucceedsMaximum           of string

// let private validateId input =
//     match EntityId.create "LanguageId" input with
//     | Ok id     -> Success id
//     | Error err -> Failure [ err ]

// let private validateLanguageName input = 
//     match LanguageName.create input with
//     | Ok name   -> Success name
//     | Error err -> Failure [ err ]

// let private validateCreatedby input =
//     match EntityId.create "CreatedBy" input with
//     | Ok id     -> Success id
//     | Error err -> Failure [ err ]

// let private validateCreatedOn input =
//     match ConstrainedDate.createFromDateTime ( new DateTime(1, 1, 2022) ) ( DateTime.Today ) "createdOn" input with
//     | Ok date   -> Success date
//     | Error err -> Failure [ err ]

// let private validateUpdatedBy input =
//     match EntityId.create "UpdatedBy" input with
//     | Ok id     -> Success id
//     | Error err -> Failure [ err ]

// let private validateUpdatedOn input =
//     match ConstrainedDate.createFromDateTime ( new DateTime(1, 1, 2022) ) ( DateTime.Today ) "createdOn" input with
//     | Ok date   -> Success date
//     | Error err -> Failure [ err ]

// let private validateLanguage =
//     validateId            
//     >>= validateLanguageName    
//     >>= validateCreatedBy     
//     >>= validateCreatedOn     
//     >>= validateUpdatedBy     
//     >>= validateUpdatedOn

    

let private mapToEntity ( unverifiedLang : UnverifiedLanguage ) =
    validationResult {
        let! id = 
            unverifiedLang.LanguageId
            |> EntityId.create 
            |> 
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



let private sqlQuery =
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
    let y = executeQuery connStr sqlQuery ( queryParams id )
            |> Async.RunSynchronously
            
    match y with
    | Some  lang ->
    | None       -> Error err
    