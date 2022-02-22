module Localisation.AddLanguageCommand

open Giraffe
open Giraffe.HttpStatusCodeHandlers
open Microsoft.AspNetCore.Http
open System.Net.Http


let ( => ) a b =
    a, box b
    
    
let addLanguageCommandHandler : HttpHandler =
    fun ( next : HttpFunc ) ( ctx : HttpContext ) ->
        Successful.OK "POST" next ctx
