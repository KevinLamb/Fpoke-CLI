open System
open FSharp.Data

let getStatus url = 
    try
        let response = Http.Request(url, silentHttpErrors = true)
        Some response.StatusCode
    with
    | :? System.Net.WebException as ex -> None

[<EntryPoint>]
let main arg =

    match arg with 
    | [|first|] -> 
        let status = getStatus first
        match status.Value with
        | 200 -> printfn "The site is UP! HTTP Status: %i" status.Value
        | 404 -> printfn "Site is not found or page is down... HTTP Status: %i" status.Value
        | 500 -> printfn "SERVER ERROR: Site is DOWN! HTTP Status: %i" status.Value
        | _ -> printfn "An error occurred while getting status..."
    | _ -> printfn "How to use: \"fpoke http://your-url.com\""
    
    0 
