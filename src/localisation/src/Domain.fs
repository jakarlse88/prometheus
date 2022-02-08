module Localisation.Domain

open System
open System.Text.RegularExpressions

type SystemString = System.String

// ---------------------------------------------------------------------------------------------------------------------
//
//      Simple and constrained types
//
// ---------------------------------------------------------------------------------------------------------------------

type ValidationError =
    | InvalidEntityIdError
    | NameEmptyError      
    | NameTooLongError
    | NameContainsIllegalCharsError of string
    | DatePrecedesMinimum
    | DateSucceedsMaximum

type EntityId        = private LanguageId of int
type ASCIIName       = private ASCIIName of string
type ValidatedName   = private ValidatedName of string
type ConstrainedDate = private ConstrainedDate of DateTime

module EntityId =
    
    /// Extract value
    let value ( LanguageId id ) =
        id
    
    /// Constructor
    let create fieldName value =
        if value <= 0 then
            Error ValidationError.InvalidEntityIdError 
        else
            LanguageId value
            |> Ok 
            
module ASCIIName =
    
    /// Extract value
    let value ( ASCIIName str ) =
        str
        
    /// Constructor
    let create str =
        if String.IsNullOrWhiteSpace( str ) then
            Error ValidationError.NameEmptyError 
        elif str.Length > 50 then
            Error ValidationError.NameTooLongError
        elif Regex.IsMatch( "^[a-zA-Z]+$", str ) then
            "String value contains illegal characters; only alphabetical ASCII values are legal"
            |> ValidationError.NameContainsIllegalCharsError
            |> Error 
        else
            Ok ( ASCIIName str )

module ValidatedName =
    
    /// Extract value
    let value ( ValidatedName str ) =
        str
        
    /// Constructor
    let create regEx illegalChars str =
        if String.IsNullOrWhiteSpace( str ) then
            Error ValidationError.NameEmptyError 
        elif str.Length > 50 then
            Error ValidationError.NameTooLongError
        elif Regex.IsMatch( regEx, str ) then
            sprintf "String value contains illegal characters; characters <%s> are not allowed" illegalChars
            |> ValidationError.NameContainsIllegalCharsError
            |> Error 
        else
            Ok ( ValidatedName str )
            
module ConstrainedDate =

    /// Extract value
    let value ( ConstrainedDate date ) =
        date

    /// Constructor
    let createFromDateTime minD maxD date =
        if date < minD then
            Error ValidationError.DatePrecedesMinimum
        elif date > maxD then
            Error ValidationError.DateSucceedsMaximum
        else    
            Ok ( ConstrainedDate date )
            
// ---------------------------------------------------------------------------------------------------------------------
//
//      Aggregate types
//
// ---------------------------------------------------------------------------------------------------------------------

/// Unverified input type
type LanguageInput = {
    LanguageId    : int
    NameInvariant : string
    NameLocal     : string
    
    CreatedBy : int
    CreatedOn : DateTime 
    UpdatedBy : int
    UpdatedOn : DateTime
}

/// Domain entity
type Language = {
    LanguageId    : EntityId
    NameInvariant : ASCIIName
    NameLocal     : ASCIIName
    
    CreatedOn  : DateTime
    CreatedBy  : EntityId
    UpdatedOn  : DateTime
    UpdatedBy  : EntityId
}

/// Unverified intermediate DB type
type UnverifiedLanguage = {
    LanguageId    : int
    NameInvariant : string
    NameLocal     : string
    
    CreatedBy : int
    CreatedOn : DateTime 
    UpdatedBy : int
    UpdatedOn : DateTime
}