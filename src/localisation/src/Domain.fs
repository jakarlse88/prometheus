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
    
    /// Get a formatted creation error message 
    let private getErrorMsg fieldName value =
        sprintf "'%s' must be greater than zero, but was '%i'" fieldName value
    
    /// Extract value
    let value ( LanguageId id ) =
        id
    
    /// Constructor
    let create fieldName value =
        if value <= 0 then
            getErrorMsg fieldName value
            |> Error 
        else
            LanguageId value
            |> Ok
            

module LanguageNameId =
    
    /// Get a formatted creation error message
    let private getErrorMsg fieldName value =
        sprintf "'%s' must be greater than zero, but was '%i'" fieldName value
    
    /// Extract value
    let value ( LanguageNameId id ) =
        id
    
    /// Constructor
    let create fieldName value =
        if value <= 0 then
            getErrorMsg fieldName value
            |> Error 
        else
            LanguageNameId value
            |> Ok 
            
            
module UserId =
    
    /// Get a formatted error creation message
    let private getErrorMsg fieldName value =
        sprintf "'%s' must be greater than zero, but was '%i'" fieldName value
    
    /// Extract value
    let value ( UserId id ) =
        id
    
    /// Constructor
    let create fieldName value =
        if value <= 0 then
            getErrorMsg fieldName value
            |> Error 
        else
            UserId value
            |> Ok 


module ASCIIString =
    
    /// Get a formatted error message for an empty value creation error
    let private getValueEmptyErrorMsg fieldName =
        sprintf "'%s' cannot not be empty" fieldName
        
        
    /// Get a formatted error message for a too long value creation error
    let private getValueTooLongErrorMsg fieldName len =
        sprintf "'%s' cannot exceed 50 characters in length, " fieldName
        |> ( + ) ( sprintf "but the provided had a length of '%i'" len )
        

    /// Get a formatted error message for an illegal character creation error
    let private getIllegalCharErrorMsg fieldName =
        sprintf "%s can only contain ASCII characters, parentheses, and dashes" fieldName
        

    /// Extract value
    let value ( ASCIIString str ) =
        str
        
    /// Constructor
    let create fieldName str =
        if String.IsNullOrWhiteSpace( str ) then
            getValueEmptyErrorMsg fieldName
            |> Error 

        elif str.Length > 50 then
            getValueTooLongErrorMsg fieldName str.Length
            |> Error 

        elif Regex.IsMatch( "^[a-zA-Z()-]+$", str ) then
            getIllegalCharErrorMsg fieldName
            |> Error 

        else
            Ok ( ASCIIString str )


module ConstrainedDate =

    /// Get a formatted error message for a date preceding the minimum date
    let private getDatePrecedesMinDateErrorMsg ( minDate : DateTime ) ( actualDate : DateTime ) =
        let minDate'    = minDate.ToShortDateString()
        let actualDate' = actualDate.ToShortDateString()

        sprintf "Date cannot occur before %s (value was %s)" minDate' actualDate'
    
    /// Get a formatted error message for a date succeeding the maximum date
    let private getDateSucceedsMaxDateErrorMsg ( maxDate : DateTime ) ( actualDate : DateTime ) =
        let maxDate'    = maxDate.ToShortDateString()
        let actualDate' = actualDate.ToShortDateString()

        sprintf "Date cannot occur after %s (value was %s) " maxDate' actualDate'
    
    /// Extract value
    let value ( ConstrainedDate date ) =
        date

    /// Constructor
    let createFromDateTime ( minDate : DateTime ) ( maxDate : DateTime ) ( actualDate : DateTime ) =
        if  actualDate < minDate then 
            getDatePrecedesMinDateErrorMsg minDate actualDate
            |> Error 

        elif actualDate > maxDate then 
            getDateSucceedsMaxDateErrorMsg maxDate actualDate
            |> Error

        else
            ConstrainedDate actualDate 
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


