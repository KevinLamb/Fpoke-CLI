namespace Fpoke.Modules

open FSharp.Data

module Poke =

    let GetStatus url = 
        try
            let response = Http.Request(url, silentHttpErrors = true)
            Some response.StatusCode
        with :? System.Net.WebException as ex -> printfn "There was an error creating the request: %s!" ex.Message; None