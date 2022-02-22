module Localisation.UpdateLanguageCommand

open Giraffe
open Giraffe.HttpStatusCodeHandlers
open Microsoft.AspNetCore.Http


let ( => ) a b =
    a, box b
    
    
let updateLanguageCommandHandler ( id : int ) : HttpHandler =
    fun ( next : HttpFunc ) ( ctx : HttpContext ) ->
        Successful.OK "PUT" next ctx