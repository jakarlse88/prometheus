﻿module Localisation.Domain

open System
open System.Text.RegularExpressions


// ---------------------------------------------------------------------------------------------------------------------
//
//      Simple and constrained types
//
// ---------------------------------------------------------------------------------------------------------------------

type LanguageId      = private LanguageId      of int
type Userid          = private UserId          of int
type LanguageName    = private LanguageName    of string
type ValidatedName   = private ValidatedName   of string
type ConstrainedDate = private ConstrainedDate of DateTime


module LanguageId =
    
    /// Extract value
    let value ( LanguageId id ) =
        id
    
    /// Constructor
    let create value =
        if value <= 0 then
            sprintf "'LanguageId' must be greater than zero, but was '%d'" value
            |> Error 
        else
            LanguageId value
            |> Ok 
            

module UserId =
    
    /// Extract value
    let value ( UserId id ) =
        id
    
    /// Constructor
    let create value =
        if value <= 0 then
            sprintf "'UserId' must be greater than zero, but was '%d'" value
            |> Error
        else
            UserId value
            |> Ok 
            

module LanguageName =
    
    /// Extract value
    let value ( LanguageName str ) =
        str
        
    /// Constructor
    let create str =
        if String.IsNullOrWhiteSpace( str ) then
            sprintf "'LanguageName' cannot not be empty"
            |> Error 
        elif str.Length > 50 then
            sprintf "'LanguageName' cannot be longer than 50 characters, "
            |> ( + ) ( sprintf "but the provided valuewas '%i'" str.Length )
            |> Error 
        elif Regex.IsMatch( "^[a-zA-Z]+$", str ) then
            "LanguageName can only contain ASCII characters"
            |> Error 
        else
            Ok ( LanguageName str )

module ConstrainedDate =

    /// Extract value
    let value ( ConstrainedDate date ) =
        date

    /// Constructor
    let createFromDateTime ( minDate : DateTime ) ( maxDate : DateTime ) fieldName ( date : DateTime ) =
        if  date < minDate then 
            let minD' = minDate.ToShortDateString()
            let date' = date.ToShortDateString()

            sprintf "%s cannot occur before %s (value was %s)" fieldName minD' date'
            |> Error 
        elif date > maxDate then 
            let maxD' = maxDate.ToShortDateString()
            let date' = date.ToShortDateString()

            sprintf "%s cannot occur after %s (value was %s) " fieldName maxD' date'
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
    LanguageId    : LanguageId
    NameInvariant : LanguageName
    
    CreatedOn  : DateTime
    CreatedBy  : LanguageId
    UpdatedOn  : DateTime
    UpdatedBy  : LanguageId
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