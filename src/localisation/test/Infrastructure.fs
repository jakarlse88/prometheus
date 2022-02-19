module Localisation.Test.Infrastructure


open FsCheck


type Negative =
    
    static member Int() =
        Arb.Default.Int32()
        |> Arb.mapFilter ( fun i -> i ) ( fun i -> i < 0 ) 
        

type Zero =
    
    static member Int() =
        Arb.Default.Int32()
        |> Arb.mapFilter ( fun i -> i ) ( fun i -> i = 0 )


type Positive =
    
    static member Int() =
        Arb.Default.Int32()
        |> Arb.mapFilter ( fun i -> i ) ( fun i -> i > 0 )
        