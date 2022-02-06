module Localisation.Domain

open System

type SystemString = System.String

// ---------------------------------------------------------------------------------------------------------------------
//
//      Simple and constrained types
//
// ---------------------------------------------------------------------------------------------------------------------

type LanguageId = private LanguageId of int
type NameId     = private NameId     of int
type UserId     = private UserId     of int
type NameString = private NameString of string

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
            
module NameString =
    
    open System
    
    let value ( NameString str ) =
        str
        
    let create ( str : string ) =
        if String.IsNullOrWhiteSpace( str ) then
            None
        elif str.Length > 50 then
            None
        else
            Some ( NameString str )

// ---------------------------------------------------------------------------------------------------------------------
//
//      Aggregate types
//
// ---------------------------------------------------------------------------------------------------------------------

/// Unverified input type
type LanguageInput = {
    LanguageId  : int
    Name        : string
    NameLocal   : string
    
    CreatedBy : int
    CreatedOn : DateTime 
    UpdatedBy : int
    UpdatedOn : DateTime
}

/// Domain entity
[<CLIMutable>]
type Language = {
    LanguageId : LanguageId
    Name       : NameString
    NameLocal  : NameString
    
    CreatedOn  : DateTime
    CreatedBy  : UserId
    UpdatedOn  : DateTime
    UpdatedBy  : UserId
}

/// Unverified intermediate DB type
type UnverifiedLanguage = {
    LanguageId  : int
    Name        : string
    NameLocal   : string
    
    CreatedBy : int
    CreatedOn : DateTime 
    UpdatedBy : int
    UpdatedOn : DateTime
}