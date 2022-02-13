module Localisation.Domain

open System
open System.Text.RegularExpressions

type SystemString = String

// ---------------------------------------------------------------------------------------------------------------------
//
//      Simple and constrained types
//
// ---------------------------------------------------------------------------------------------------------------------


type EntityId        = private LanguageId      of int
type LanguageName    = private ASCIIName       of string
type ValidatedName   = private ValidatedName   of string
type ConstrainedDate = private ConstrainedDate of DateTime

type ValidationError =
    | InvalidEntityIdError          of string
    | NameEmptyError               
    | NameTooLongError             
    | NameContainsIllegalCharsError of string
    | DatePrecedesMinimum           of string
    | DateSucceedsMaximum           of string

module EntityId =
    
    /// Extract value
    let value ( LanguageId id ) =
        id
    
    /// Constructor
    let create fieldName value =
        if value <= 0 then
            sprintf "Invalid value for '%s' (%i); must be a positive integer value" fieldName value
            |> InvalidEntityIdError 
            |> Error  
        else
            LanguageId value
            |> Ok 
            
module LanguageName =
    
    /// Extract value
    let value ( ASCIIName str ) =
        str
        
    /// Constructor
    let create str =
        if String.IsNullOrWhiteSpace( str ) then
            Error NameEmptyError 
        elif str.Length > 50 then
            Error NameTooLongError
        elif Regex.IsMatch( "^[a-zæøåA-ZÆØÅ]+$", str ) then
            "String value contains illegal characters; only alphabetical characters are allowed."
            |> NameContainsIllegalCharsError 
            |> Error 
        else
            Ok ( ASCIIName str )

module ConstrainedDate =
    
    /// Extract value
    let value ( ConstrainedDate date ) =
        date

    /// Constructor
    let createFromDateTime ( minD : DateTime ) ( maxD : DateTime ) fieldName ( date : DateTime ) =
        if  date < minD then 
            sprintf "%s cannot occur before %s (value was %s) " fieldName ( minD.ToShortDateString() ) ( date.ToShortDateString() )
            |> DatePrecedesMinimum
            |> Error 
        elif date > maxD then 
            sprintf "%s cannot occur after %s (value was %s) " fieldName ( maxD.ToShortDateString() ) ( date.ToShortDateString() )
            |> DateSucceedsMaximum
            |> Error
        else
            ConstrainedDate date 
            |> Ok 
            
// ---------------------------------------------------------------------------------------------------------------------
//
//      Aggregate types
//
// ---------------------------------------------------------------------------------------------------------------------

/// Unverified input type
type LanguageInput = {
    LanguageId    : int
    NameInvariant : string
    
    CreatedBy : int
    CreatedOn : DateTime 
    UpdatedBy : int
    UpdatedOn : DateTime
}

/// Domain entity
type Language = {
    LanguageId    : EntityId
    NameInvariant : LanguageName
    
    CreatedOn  : DateTime
    CreatedBy  : EntityId
    UpdatedOn  : DateTime
    UpdatedBy  : EntityId
}

/// Unverified intermediate DB type
type UnverifiedLanguage = {
    LanguageId    : int
    NameInvariant : string
    
    CreatedBy : int
    CreatedOn : DateTime 
    UpdatedBy : int
    UpdatedOn : DateTime
}