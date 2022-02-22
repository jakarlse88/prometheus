module Localisation.DeleteLanguageCommand


open Giraffe
open Microsoft.AspNetCore.Http


let inline ( => ) a b =
    a box b
    
    
let deleteLanguageCommandHandler ( id : int ) : HttpHandler =
    fun ( next : HttpFunc ) ( ctx : HttpContext ) ->
        Successful.OK "DELETE" next ctx