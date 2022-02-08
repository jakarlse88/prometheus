module Localisation.GetLanguageQuery

open System.Data
open System.Net
open Dapper
open Localisation.Domain
open Microsoft.Data.SqlClient

let inline ( => ) a b = a, box b

let private validateInput id =
    EntityId.create "LanguageId" id
    
let private mapToEntity ( unverifiedLang : UnverifiedLanguage ) =
    result {
        
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
    let nameLocalIndex      = reader.GetOrdinal "NameLocal"
    let createdByIndex      = reader.GetOrdinal "CreatedBy"
    let createdOnIndex      = reader.GetOrdinal "CreatedOn"
    let updatedByIndex      = reader.GetOrdinal "UpdatedBy"
    let updatedOnIndex      = reader.GetOrdinal "UpdatedOn"
    
    [
        while reader.Read() do
            yield {
                LanguageId    = reader.GetInt32  languageIdIndex
                NameInvariant = reader.GetString nameInvariantIndex
                NameLocal     = reader.GetString nameLocalIndex
                    
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
    