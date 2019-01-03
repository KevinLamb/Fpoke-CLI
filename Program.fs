open System
open Fpoke.Modules.Poke
open Fpoke.Modules.SMTP

let infoCodes = [100 .. 199]
let goodCodes = [200 .. 299]
let okCodes = [300 .. 399]
let clientErrorCodes = [400 .. 499]
let serverErrorCodes = [500 .. 599]

[<EntryPoint>]
let main arg =

    match arg with
    | a when Array.contains "/email" a-> 

        let statusCode = GetStatus arg.[0]

        match statusCode with
        | Some status -> 
                    match status with
                        | s when List.contains s infoCodes = true -> 
                            printfn "The site is UP! \r\nHTTP Status: %i" status
                            let message =  String.Concat("<h1>The site is UP! \r\nHTTP Status: ", status, "</h1>")
                            SendMail arg.[2] message

                        | s when List.contains s goodCodes = true -> 
                            printfn "The site is UP! \r\nHTTP Status: %i" status
                            let message =  String.Concat("<h1>The site is UP! \r\nHTTP Status: ", status, "</h1>")
                            SendMail arg.[2] message

                        | s when List.contains s okCodes = true -> 
                            printfn "The site is UP! \r\nHTTP Status: %i" status
                            let message =  String.Concat("<h1>The site is UP! \r\nHTTP Status: ", status, "</h1>")
                            SendMail arg.[2] message

                        | s when List.contains s clientErrorCodes = true -> 
                            printfn "Page is not found or page is down... \r\nHTTP Status: %i" status
                            let message =  String.Concat("<h1>Page is not found or page is down... \r\nHTTP Status: ", status, "</h1>")
                            SendMail arg.[2] message

                        | s when List.contains s serverErrorCodes = true -> 
                            printfn "SERVER ERROR: Site is DOWN! \r\nHTTP Status: %i" status
                            let message =  String.Concat("<h1>SERVER ERROR: Site is DOWN! \r\nHTTP Status: ", status, "</h1>")
                            SendMail arg.[2] message

                        | _ -> printfn "An error occurred while getting status..."

        | None -> printfn "How to use: \"fpoke http://your-url.com\""
    
    | [|first|]-> 
        let statusCode = GetStatus first

        match statusCode with
        | Some status -> 
                    match status with
                        | s when List.contains s infoCodes = true -> 
                            printfn "The site is UP! \r\nHTTP Status: %i" status

                        | s when List.contains s goodCodes = true -> 
                            printfn "The site is UP! \r\nHTTP Status: %i" status

                        | s when List.contains s okCodes = true -> 
                            printfn "The site is UP! \r\nHTTP Status: %i" status

                        | s when List.contains s clientErrorCodes = true -> 
                            printfn "Page is not found or page is down... \r\nHTTP Status: %i" status

                        | s when List.contains s serverErrorCodes = true -> 
                            printfn "SERVER ERROR: Site is DOWN! \r\nHTTP Status: %i" status

                        | _ -> printfn "An error occurred while getting status..."

        | None -> printfn "How to use: \"fpoke http://your-url.com\""
    | _ -> printfn "How to use: \"fpoke http://your-url.com\""
    
    0 
