open Argu
open Fpoke.Modules.Poke
open Fpoke.Modules.SMTP
open System

type FpokeArguments =
    | [<MainCommand>] URL of url:string
    | [<AltCommandLine("-email")>] Email of email:string
    with
        interface IArgParserTemplate with
            member fpoke.Usage =
                match fpoke with
                | URL _ -> "The URL of the site you're checking."
                | Email _ -> "The email you want to send a report to. (Make sure to set up SMTP connection)"


let infoCodes = [100 .. 199]
let goodCodes = [200 .. 299]
let okCodes = [300 .. 399]
let clientErrorCodes = [400 .. 499]
let serverErrorCodes = [500 .. 599]

[<EntryPoint>]
let main arg =

    let argParser = ArgumentParser.Create<FpokeArguments>(programName = "fpoke")

    let results = argParser.ParseCommandLine(arg)
    
    let containsEmail = results.Contains Email

    let fpokeUsage = argParser.PrintUsage()
    
    match results.Contains URL with
    | true -> 

        let url = results.GetResult URL

        let statusCode = GetStatus url

        match statusCode with
        | Some status -> 
                    match status with
                        | s when List.contains s infoCodes = true -> 
                            printfn "The site is UP! \r\nHTTP Status: %i" status

                            if (containsEmail) then
                                let email = results.GetResult Email
                                let message =  String.Concat("The site is UP! \r\nHTTP Status: ", status)
                                SendMail email message

                        | s when List.contains s goodCodes = true -> 
                            printfn "The site is UP! \r\nHTTP Status: %i" status

                            if (containsEmail) then
                                let email = results.GetResult Email
                                let message =  String.Concat("The site is UP! \r\nHTTP Status: ", status)
                                SendMail email message

                        | s when List.contains s okCodes = true -> 
                            printfn "The site is UP! \r\nHTTP Status: %i" status

                            if (containsEmail) then
                                let email = results.GetResult Email
                                let message =  String.Concat("The site is UP! \r\nHTTP Status: ", status)
                                SendMail email message

                        | s when List.contains s clientErrorCodes = true -> 
                            printfn "Page is not found or page is down... \r\nHTTP Status: %i" status

                            if (containsEmail) then
                                let email = results.GetResult Email
                                let message =  String.Concat("Page is not found or page is down... \r\nHTTP Status: ", status)
                                SendMail email message

                        | s when List.contains s serverErrorCodes = true -> 
                            printfn "SERVER ERROR: Site is DOWN! \r\nHTTP Status: %i" status

                            if (containsEmail) then
                                let email = results.GetResult Email
                                let message =  String.Concat("SERVER ERROR: Site is DOWN! \r\nHTTP Status: ", status)
                                SendMail email message

                        | _ -> printfn "An error occurred while getting status..."

        | None -> printfn "%s" fpokeUsage
    | _ -> printfn "%s" fpokeUsage
    
    0 