module Localisation.GetLanguageAllQuery


open Giraffe
open Microsoft.AspNetCore.Http


let inline ( => ) a b =
    a, box b


let getLanguageAllQueryHandler : HttpHandler =
    fun ( next : HttpFunc ) ( ctx : HttpContext ) ->
        Successful.OK "POST" next ctx