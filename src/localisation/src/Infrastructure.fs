namespace Global

[<AutoOpen>]
module Result =

    let mapError = Result.mapError

[<AutoOpen>]
module ResultComputationExpression =

    type ResultBuilder() =
        member __.Return( x )   = Ok x 
        member __.Bind( x, fn ) = Result.bind fn x
        member __.Zero()        = __.Return ()
        member __.Run( fn )     = fn()
        member __.Delay( fn )   = fn

    let result = new ResultBuilder()
