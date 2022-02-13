namespace global

// ----------------------------------------------------------------------------------------------------
//
//      The 'ValidationResult' is the same as the 'Result' type,
//      but with a list of failures rather than a single value.
//
// ----------------------------------------------------------------------------------------------------
type ValidationResult<'Success, 'Failure> =
    | Success of 'Success
    | Failure of 'Failure list
    
/// Functions for the `ValidationResult` type
[<AutoOpen>]
module ValidationResult =

    let private bindValidationResult switchFn input =
        match input with
        | Success s -> switchFn s
        | Failure f -> Failure f

    let ( >>= ) switchFn input =
        bindValidationResult switchFn input

    let ( >=> ) switchFnA switchFnB =
        match switchFnA with
        | Success s -> switchFnB s
        | Failure f -> Failure f