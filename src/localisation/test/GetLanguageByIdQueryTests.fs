module Localisation.Test.GetLanguageByIdQueryTests


open System.Collections.Generic
open Localisation.Test.Infrastructure
open Xunit
open FsCheck
open FsCheck.Xunit
open Localisation
open Localisation.Domain
open Localisation.GetLanguageQuery
open Microsoft.FSharp.Core
open System


[<assembly: Properties( Arbitrary=[| typeof<Negative> |])>] do()


module minDateTests =

    
    [<Fact>]
    let ``minDate returns a DateTime whose value represents January 1 of the current year`` () =
         // Arrange
         let expected = new DateTime( DateTime.Now.Year, 1, 1 )
         
         // Act
         let actual   = minDate
         
         // Assert
         Assert.Equal( expected, actual )
     
     
module maxDateTests =
         
    
    [<Fact>]
    let ``maxDate returns a DateTime whose value represents the current day`` () =
        // Arrange
        let expected = DateTime.Today
        
        // Act
        let actual   = maxDate
        
        // Assert
        Assert.Equal( expected, actual )
        
        
module verifyLanguageIdTests  =
    
    let config = {
        Config.Quick with
            MaxTest = 100
        }
    
    
    let private getErrOrThrow ( errs : ErrorType list ) =
        let getErrorMsg ( LanguageIdInvalidError input ) =
                input
    
        let err = errs |> List.tryHead
        
        match err with
        | Some x -> getErrorMsg x
        | None _ -> failwith "Received empty error list"
            
    
    [<Property( Arbitrary=[| typeof<Negative> |])>]
    let ``verifyLanguageId returns error when input value is a negative int`` ( testId : int ) =
        
        // Arrange
        let expected = sprintf "'LanguageId' must be greater than zero, but was '%i'" testId
        
        match verifyLanguageId testId with   
            
            | ValidationResult.Failure errs ->
                let actual = getErrOrThrow errs
                expected   = actual
            
            | Success id   ->
                sprintf "Expected 'Failure', was Success(%i)" ( LanguageId.value id )
                |> failwith
        
    
    [<Property( MaxTest = 1, Arbitrary=[| typeof<Zero> |])>]
    let ``verifyLanguageId returns error when input value is equal to zero`` ( testId : int ) =
        
        // Arrange
        let expected = sprintf "'LanguageId' must be greater than zero, but was '%i'" testId
        
        match verifyLanguageId testId with   
            
            | ValidationResult.Failure errs ->
                let actual = getErrOrThrow errs
                expected   = actual
            
            | Success id   ->
                sprintf "Expected 'Failure', was Success(%i)" ( LanguageId.value id )
                |> failwith
                
                
    [<Property( Arbitrary=[| typeof<Positive> |])>]
    let ``verifyLanguageId returns success when input is positive integer value`` ( testId : int ) =
        
        // Arrange
        match verifyLanguageId testId with   
            
            | ValidationResult.Failure errs ->
                let err = getErrOrThrow errs
                
                sprintf "Expected 'Success', was Failure: '%s'" err
                |> failwith
            
            | Success id   ->
                testId = ( LanguageId.value id )
                

                
module verifyCreatedByTests  =
    
    let config = {
        Config.Quick with
            MaxTest = 100
        }
    
    
    let private getErrOrThrow ( errs : ErrorType list ) =
        let getErrorMsg ( CreatedByInvalidIdError input ) =
                input
    
        let err = errs |> List.tryHead
        
        match err with
        | Some x -> getErrorMsg x
        | None _ -> failwith "Received empty error list"
    
    
    [<Property( Arbitrary=[| typeof<Negative> |])>]
    let ``verifyCreatedBy returns error when input value is a negative int`` ( testId : int ) =
        
        // Arrange
        let expected = sprintf "'CreatedBy' must be greater than zero, but was '%i'" testId
        
        match verifyCreatedBy testId with   
            
            | ValidationResult.Failure errs ->
                let actual = getErrOrThrow errs
                expected   = actual
            
            | Success id   ->
                sprintf "Expected 'Failure', was Success(%i)" ( UserId.value id )
                |> failwith
        
    
    [<Property( MaxTest = 1, Arbitrary=[| typeof<Zero> |])>]
    let ``verifyCreatedBy returns error when input value is equal to zero`` ( testId : int ) =
        
        // Arrange
        let expected = sprintf "'CreatedBy' must be greater than zero, but was '%i'" testId
        
        match verifyCreatedBy testId with   
            
            | ValidationResult.Failure errs ->
                let actual = getErrOrThrow errs
                expected   = actual
            
            | Success id   ->
                sprintf "Expected 'Failure', was Success(%i)" ( UserId.value id )
                |> failwith
                
                
    [<Property( Arbitrary=[| typeof<Positive> |])>]
    let ``verifyCreatedBy returns success when input is positive integer value`` ( testId : int ) =
        
        // Arrange
        match verifyCreatedBy testId with   
            
            | ValidationResult.Failure errs ->
                let err = getErrOrThrow errs
                
                sprintf "Expected 'Success', was Failure: '%s'" err
                |> failwith
            
            | Success id   ->
                testId = ( UserId.value id )
                
                
module verifyUpdatedByTests  =
    
    let config = {
        Config.Quick with
            MaxTest = 100
        }
    
    
    let private getErrOrThrow ( errs : ErrorType list ) =
        let getErrorMsg ( UpdatedByInvalidIdError input ) =
                input
    
        let err = errs |> List.tryHead
        
        match err with
        | Some x -> getErrorMsg x
        | None _ -> failwith "Received empty error list"
        
    
    [<Property( Arbitrary=[| typeof<Negative> |])>]
    let ``verifyUpdatedBy returns error when input value is a negative int`` ( testId : int ) =
        
        // Arrange
        let expected = sprintf "'UpdatedBy' must be greater than zero, but was '%i'" testId
        
        match verifyUpdatedBy testId with   
            
            | ValidationResult.Failure errs ->
                let actual  = getErrOrThrow errs
                expected    = actual
            
            | Success id   ->
                sprintf "Expected 'Failure', was Success(%i)" ( UserId.value id )
                |> failwith
        
    
    [<Property( MaxTest = 1, Arbitrary=[| typeof<Zero> |])>]
    let ``verifyUpdatedBy returns error when input value is equal to zero`` ( testId : int ) =
        
        // Arrange
        let expected = sprintf "'UpdatedBy' must be greater than zero, but was '%i'" testId
        
        match verifyUpdatedBy testId with   
            
            | ValidationResult.Failure errs ->
                let actual  = getErrOrThrow errs
                expected    = actual
            
            | Success id   ->
                sprintf "Expected 'Failure', was Success(%i)" ( UserId.value id )
                |> failwith
                
                
    [<Property( Arbitrary=[| typeof<Positive> |])>]
    let ``verifyUpdatedBy returns success when input is positive integer value`` ( testId : int ) =
        
        match verifyUpdatedBy testId with   
            
        | ValidationResult.Failure errs ->
            let err = getErrOrThrow errs 
            
            sprintf "Expected 'Success', was Failure: '%s'" err
            |> failwith
        
        | Success id   ->
            testId = ( UserId.value id )
                
module verifyLanguageNameTests =
    
    let config = {
        Config.Quick with
            MaxTest = 100
        }
    
    
    let private getErrOrThrow ( errs : ErrorType list ) =
        let getErrorMsg ( LanguageNameInvalidError input ) =
                input
    
        let err = errs |> List.tryHead
        
        match err with
        | Some x -> getErrorMsg x
        | None _ -> failwith "Received empty error list"
    
          
            
    [<Property( Arbitrary=[| typeof<Zero> |])>]
    let ``VerifyLanguageName returns error when input is an empty string`` ( testInput : string ) =
        
        let expected = "'Name' cannot not be empty"
        
        match verifyLanguageName testInput with
        
        | ValidationResult.Failure errs ->
            let actual = getErrOrThrow errs
            
            expected = actual
        | Success x ->
            sprintf "Expected 'Failure', was Success(%s)" ( ASCIIString.value x )
            |> failwith
            
            
    [<Property( Arbitrary=[| typeof<SizeOver50> |])>]
    let ``VerifyLanguageName returns error when input is a string with length exceeding 50`` ( testInput : string ) =
        
        
        
        let expected = "'Name' cannot exceed 50 characters in length, "
                        + sprintf "but the provided value had a length of '%i'" testInput.Length
        
        match verifyLanguageName testInput with
        
        | ValidationResult.Failure errs ->
            let actual = getErrOrThrow errs
            
            expected = actual
        | Success x ->
            sprintf "Expected 'Failure', was Success(%s)" ( ASCIIString.value x )
            |> failwith
            