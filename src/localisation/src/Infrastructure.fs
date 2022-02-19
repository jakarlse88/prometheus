namespace global

type ValidationResult<'Success, 'Failure> =
    | Success of 'Success
    | Failure of 'Failure list
    

[<RequireQualifiedAccess>]
module ValidationResult =
    
    
    open System

    
    /// Lift a value to a `Success` case of `ValidationResult`
    let retn x =
        Success x

    
    /// Lift an `Error` to a `Failure` case of `ValidationResult` 
    let ofError err =
        Failure [ err ]

    
    /// Lift an `Exception` to a `Failure` case of `ValidationResult`
    let ofEx ( ex : Exception ) =
        Failure [ ex.Message ]

    
    /// Lift a function to `ValidationResult` 
    let map fn xResult =
        match xResult with
        | Failure errs -> Failure errs
        | Success x    -> fn x |> retn

    
    /// Compose monadic `ValidationResult` functions
    let bind fn xResult =
        match xResult with
        | Failure errs -> Failure errs
        | Success x    -> fn x

    
    /// Lift a multi-parameter function to `ValidationResult` 
    let apply fResult xResult =
        match fResult, xResult with
        | Success fn    , Success x     -> fn x |> retn
        | Failure errs  , Success _     -> Failure errs
        | Success _     , Failure errs  -> Failure errs
        | Failure errs1 , Failure errs2 -> errs1 @ errs2 |> Failure
