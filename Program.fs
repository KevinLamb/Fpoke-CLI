open Argu
open Fpoke.Modules.Poke
open Fpoke.Modules.SMTP
open System

type FpokeArguments =
    | [<MainCommand>] URL of url:string
    | [<AltCommandLine("-email")>] Email of email:string
    | [<AltCommandLine("-error")>] ErrorOnly
    | [<AltCommandLine("-p")>] Port of port:int
    with
        interface IArgParserTemplate with
            member fpoke.Usage =
                match fpoke with
                | URL _ -> "The URL of the site you're checking."
                | Email _ -> "The email you want to send a report to. (Make sure to set up SMTP connection)"
                | ErrorOnly _ -> "A setting for only sending an email report upon error codes."
                | Port _ -> "The port you want to ping on the site's server."


let infoCodes = [100 .. 199]
let goodCodes = [200 .. 299]
let redirectCodes = [300 .. 399]
let clientErrorCodes = [400 .. 499]
let serverErrorCodes = [500 .. 599]

[<EntryPoint>]
let main arg =

    let mutable message = ""

    let argParser = ArgumentParser.Create<FpokeArguments>(programName = "fpoke")

    let results = argParser.ParseCommandLine(arg)
    
    let containsEmail = results.Contains Email
    let errorOnly = results.Contains ErrorOnly
    let portCheck = results.Contains Port

    let fpokeUsage = argParser.PrintUsage()
    
    match results.Contains URL with
    | true -> 

        //Check Status and send email
        let url = results.GetResult URL

        let statusCode = GetStatus url

        match statusCode with
        | Some status -> 
                    match status with
                        | s when List.contains s infoCodes = true -> 
                            printfn "The site is UP! \r\nHTTP Status: %i" status
                            message <- String.Concat("The site (" + url + ") is UP! \r\nHTTP Status: ", status)

                        | s when List.contains s goodCodes = true ->
                            printfn "The site is UP! \r\nHTTP Status: %i" status
                            message <- String.Concat("The site (" + url + ") is UP! \r\nHTTP Status: ", status)        

                        | s when List.contains s redirectCodes = true -> 
                            printfn "The site is UP! \r\nHTTP Status: %i" status
                            message <- String.Concat("The site (" + url + ") is UP! \r\nHTTP Status: ", status)

                        | s when List.contains s clientErrorCodes = true -> 
                            printfn "Page is not found or page is down... \r\nHTTP Status: %i" status
                            message <- String.Concat("The page (" + url + ") is not found or page is down... \r\nHTTP Status: ", status)

                        | s when List.contains s serverErrorCodes = true -> 
                            printfn "SERVER ERROR: Site is DOWN! \r\nHTTP Status: %i" status
                            message <- String.Concat("SERVER ERROR: The site (" + url + ") is DOWN! \r\nHTTP Status: ", status)
                
                        | _ -> printfn "An error occurred while getting status..."

        | None -> printfn "%s" fpokeUsage

        //Check port
        if(portCheck) then
            let port = results.GetResult Port

            let portConnected = GetPortStatus(url, port)

            if(portConnected) then
                printfn "Port %i is open." port
                message <- message + String.Concat(" \r\nPort %i is open.", port)
            else
                printfn "Port %i is closed." port
                message <- message + String.Concat(" \r\nPort %i is closed.", port)
        
        if (containsEmail && not errorOnly) then
            let email = results.GetResult Email
            SendMail email message
    | _ -> printfn "%s" fpokeUsage
    
    0 