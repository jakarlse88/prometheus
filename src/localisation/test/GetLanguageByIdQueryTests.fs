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
    
    
    [<Property( Arbitrary=[| typeof<Negative> |])>]
    let ``verifyLanguageId returns error when input value is a negative int`` ( testId : int ) =
        
        // Arrange
        let intGen = Arb.generate<int>
        
        let getValue ( LanguageIdInvalidError input ) =
            input
        
        let expected = sprintf "'LanguageId' must be greater than zero, but was '%i'" testId
        
        match verifyLanguageId testId with   
            
            | ValidationResult.Failure errs ->
                let actual = errs |> List.head |> getValue
                expected = actual
            
            | Success id   ->
                sprintf "Expected 'Failure', was Success(%i)" ( LanguageId.value id )
                |> failwith
        
    
    [<Property( MaxTest = 1, Arbitrary=[| typeof<Zero> |])>]
    let ``verifyLanguageId returns error when input value is equal to zero`` ( testId : int ) =
        
        // Arrange
        let intGen = Arb.generate<int>
        
        let getValue ( LanguageIdInvalidError input ) =
            input
        
        let expected = sprintf "'LanguageId' must be greater than zero, but was '%i'" testId
        
        match verifyLanguageId testId with   
            
            | ValidationResult.Failure errs ->
                let actual = errs |> List.head |> getValue
                expected = actual
            
            | Success id   ->
                sprintf "Expected 'Failure', was Success(%i)" ( LanguageId.value id )
                |> failwith
                
                
    [<Property( Arbitrary=[| typeof<Positive> |])>]
    let ``verifyLanguageId returns success when input is positive integer value`` ( testId : int ) =
        
        // Arrange
        let intGen = Arb.generate<int>
        
        let getValue ( LanguageIdInvalidError input ) =
            input
        
        match verifyLanguageId testId with   
            
            | ValidationResult.Failure errs ->
                let err = errs |> List.head |> getValue
                
                sprintf "Expected 'Success', was Failure: '%s'" err
                |> failwith
            
            | Success id   ->
                testId = ( LanguageId.value id )
        