module Localisation.Test.Infrastructure


open System
open FsCheck


type Negative =
    
    static member Int() =
        Arb.Default.Int32()
        |> Arb.mapFilter
               ( fun i -> i )
               ( fun i -> i < 0 ) 
        

type Zero =
    
    static member Int() =
        Arb.Default.Int32()
        |> Arb.mapFilter
               ( fun i -> i )
               ( fun i -> i = 0 )
        
    
    static member String() =
        Arb.Default.String()
        |> Arb.filter
               ( fun str -> str = null or str = "\0" )


type SizeOver50 =
    
    static member String() =
        let predicate str =
            String.IsNullOrWhiteSpace( str ) = false && str.Length > 50
        
        Arb.Default.String()
        |> Arb.mapFilter
               ( fun x -> x )
               ( fun str -> predicate str )


type Positive =
    
    static member Int() =
        Arb.Default.Int32()
        |> Arb.mapFilter
               ( fun i -> i )
               ( fun i -> i > 0 )
        