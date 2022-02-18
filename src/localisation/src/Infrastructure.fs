namespace global

type ValidationResult<'Success, 'Failure> =
    | Success of 'Success
    | Failure of 'Failure list

[<RequireQualifiedAccess>]
module ValidationResult =
    
    open System

    let retn x =
        Success x


    let ofError err =
        Failure [ err ]


    let ofEx ( ex : Exception ) =
        Failure [ ex.Message ]

    
    let map fn xResult =
        match xResult with
        | Failure errs -> Failure errs
        | Success x    -> fn x |> retn


    let bind fn xResult =
        match xResult with
        | Failure errs -> Failure errs
        | Success x    -> fn x

    
    let apply fResult xResult =
        match fResult, xResult with
        | Success fn    , Success x     -> fn x |> retn
        | Failure errs  , Success _     -> Failure errs
        | Success _     , Failure errs  -> Failure errs
        | Failure errs1 , Failure errs2 -> errs1 @ errs2 |> Failure
