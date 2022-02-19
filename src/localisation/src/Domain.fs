module Localisation.Domain

open System
open System.Text.RegularExpressions


// ---------------------------------------------------------------------------------------------------------------------
//
//      Simple and constrained types
//
// ---------------------------------------------------------------------------------------------------------------------

type LanguageId      = private LanguageId      of int
type LanguageNameId  = private LanguageNameId  of int
type UserId          = private UserId          of int
type ASCIIString     = private ASCIIString     of string
type ValidatedName   = private ValidatedName   of string
type ConstrainedDate = private ConstrainedDate of DateTime


module LanguageId =
    
    /// Extract value
    let value ( LanguageId id ) =
        id
    
    /// Constructor
    let create fieldName value =
        if value <= 0 then
            sprintf "'%s' must be greater than zero, but was '%i'" fieldName value
            |> Error 
        else
            LanguageId value
            |> Ok
            

module LanguageNameId =
    
    /// Extract value
    let value ( LanguageNameId id ) =
        id
    
    /// Constructor
    let create fieldName value =
        if value <= 0 then
            sprintf "'%s' must be greater than zero, but was '%i'" fieldName value
            |> Error 
        else
            LanguageNameId value
            |> Ok 
            
            
module UserId =
    
    /// Extract value
    let value ( UserId id ) =
        id
    
    /// Constructor
    let create fieldName value =
        if value <= 0 then
            sprintf "'%s' must be greater than zero, but was '%i'" fieldName value
            |> Error 
        else
            UserId value
            |> Ok 


module ASCIIString =
    
    /// Extract value
    let value ( ASCIIString str ) =
        str
        
    /// Constructor
    let create fieldName str =
        if String.IsNullOrWhiteSpace( str ) then
            sprintf "'%s' cannot not be empty" fieldName
            |> Error 
        elif str.Length > 50 then
            sprintf "'%s' cannot be longer than 50 characters, " fieldName
            |> ( + ) ( sprintf "but the provided value was '%i'" str.Length )
            |> Error 
        elif Regex.IsMatch( "^[a-zA-Z()-]+$", str ) then
            "LanguageName can only contain ASCII characters, parentheses, and dashes"
            |> Error 
        else
            Ok ( ASCIIString str )


module ConstrainedDate =

    /// Extract value
    let value ( ConstrainedDate date ) =
        date

    /// Constructor
    let createFromDateTime ( minDate : DateTime ) ( maxDate : DateTime ) ( date : DateTime ) =
        if  date < minDate then 
            let minD' = minDate.ToShortDateString()
            let date' = date.ToShortDateString()

            sprintf "Date cannot occur before %s (value was %s)" minD' date'
            |> Error 
        elif date > maxDate then 
            let maxD' = maxDate.ToShortDateString()
            let date' = date.ToShortDateString()

            sprintf "Date cannot occur after %s (value was %s) " maxD' date'
            |> Error
        else
            ConstrainedDate date 
            |> Ok 
            

// ---------------------------------------------------------------------------------------------------------------------
//
//      Aggregate types
//
// ---------------------------------------------------------------------------------------------------------------------

type Language = {
    LanguageId : LanguageId
    Name       : ASCIIString
    
    CreatedOn  : ConstrainedDate
    CreatedBy  : UserId
    UpdatedOn  : ConstrainedDate
    UpdatedBy  : UserId
}


