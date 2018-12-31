open System
open FSharp.Data

let infoCodes = [100 .. 199]
let goodCodes = [200 .. 299]
let okCodes = [300 .. 399]
let clientErrorCodes = [400 .. 499]
let serverErrorCodes = [500 .. 599]

let getStatus url = 
    try
        let response = Http.Request(url, silentHttpErrors = true)
        Some response.StatusCode
    with :? System.Net.WebException as ex -> printfn "There was an error creating the request: %s!" ex.Message; None

[<EntryPoint>]
let main arg =

    match arg with 
    | [|first|] -> 

        let statusCode = getStatus first        

        match statusCode with
        | Some status -> 
                    match status with
                        | s when List.contains s infoCodes = true -> printfn "The site is UP! \r\nHTTP Status: %i" status
                        | s when List.contains s goodCodes = true -> printfn "The site is UP! \r\nHTTP Status: %i" status
                        | s when List.contains s okCodes = true -> printfn "The site is UP! \r\nHTTP Status: %i" status
                        | s when List.contains s clientErrorCodes = true -> printfn "Page is not found or page is down... \r\nHTTP Status: %i" status
                        | s when List.contains s serverErrorCodes = true -> printfn "SERVER ERROR: Site is DOWN! \r\nHTTP Status: %i" status
                        | _ -> printfn "An error occurred while getting status..."
        | None -> printfn "How to use: \"fpoke http://your-url.com\""
        
    | _ -> printfn "How to use: \"fpoke http://your-url.com\""
    
    0 
