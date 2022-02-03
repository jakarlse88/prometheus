module Localisation.Type

open System
open FSharp.Data.GraphQL
open FSharp.Data.GraphQL.Types.SchemaDefinitions

// ---------------------------------------------------------------------------------------------------------------------
//
//      Simple and constrained types
//
// ---------------------------------------------------------------------------------------------------------------------

type LanguageId = private LanguageId of int
type NameId     = private NameId     of int
type UserId     = private UserId     of int

module LanguageId =
    
    /// Extract value
    let value ( LanguageId id ) =
        id
    
    /// Constructor
    let create value =
        if value <= 0 then
            None
        else
            Some ( LanguageId value )
            
module NameId =
    
    /// Extract value
    let value ( NameId id ) =
        id
    
    /// Constructor
    let create value =
        if value <= 0 then
            None
        else
            Some ( NameId value )
            
module UserId =
    
    /// Extract value
    let value ( UserId id ) =
        id
        
    /// Constructor
    let create value =
        if value <= 0 then
            None
        else
            Some ( UserId value )

// ---------------------------------------------------------------------------------------------------------------------
//
//      Aggregate and GraphQL object types
//
// ---------------------------------------------------------------------------------------------------------------------

type Language = {
    LanguageId : LanguageId
    
    Created    : DateTime
    CreatedBy  : UserId
    Updated    : DateTime
    UpdatedBy  : UserId
}

let LanguageType = Define.Object("Language", [
    Define.Field( "languageId" , Int    , fun ctx l -> LanguageId.value l.LanguageId )
    Define.Field( "created"    , Date   , fun ctx l -> l.Created )
    Define.Field( "createdBy"  , Int    , fun ctx l -> UserId.value l.CreatedBy)
    Define.Field( "updated"    , Date   , fun ctx l -> l.Updated )
    Define.Field( "updatedBy"  , Int    , fun ctx l -> UserId.value l.UpdatedBy)
] )

type Name = {
    NameId     : NameId
    LanguageId : LanguageId
    
    Created    : DateTime
    CreatedBy  : UserId
    Updated    : DateTime
    UpdatedBy  : UserId
}

let NameType = Define.Object("Language", [
    Define.Field( "nameId"     , Int  , fun ctx l -> NameId.value l.NameId )
    Define.Field( "languageId" , Int  , fun ctx l -> LanguageId.value l.LanguageId )
    Define.Field( "created"    , Date , fun ctx l -> l.Created )
    Define.Field( "createdBy"  , Int  , fun ctx l -> UserId.value l.CreatedBy)
    Define.Field( "updated"    , Date , fun ctx l -> l.Updated )
    Define.Field( "updatedBy"  , Int  , fun ctx l -> UserId.value l.UpdatedBy)
] )

let QueryRoot = Define.Object( "Query", [
  Define.Field( "language", ListOf LanguageType, fun ctx () -> [] ) // <-- This is probably where we need to hook up Dapper, somehow  
] )

let schema = Schema( QueryRoot )

