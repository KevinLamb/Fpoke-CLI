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
                        | stat when List.contains stat infoCodes -> 
                            message <- String.Concat("The site (" + url + ") is UP! \r\nHTTP Status: ", status)

                        | stat when List.contains stat goodCodes ->
                            message <- String.Concat("The site (" + url + ") is UP! \r\nHTTP Status: ", status)        

                        | stat when List.contains stat redirectCodes -> 
                            message <- String.Concat("The site (" + url + ") is UP! \r\nHTTP Status: ", status)

                        | stat when List.contains stat clientErrorCodes -> 
                            message <- String.Concat("The page (" + url + ") is not found or page is down... \r\nHTTP Status: ", status)
                            
                        | stat when List.contains stat serverErrorCodes -> 
                            message <- String.Concat("SERVER ERROR: The site (" + url + ") is DOWN! \r\nHTTP Status: ", status)
                                
                        | _ -> printfn "An error occurred while getting status..."

                    message <- message + String.Concat(" \r\nDescription: ", Advice.[status])

        | None -> 
            message <- "SERVER ERROR: Site is DOWN! \r\nUnable to connect to the server."

        
        //Check port
        if(portCheck) then
            let port = results.GetResult Port

            let portConnected = GetPortStatus(url, port)

            if(portConnected) then
                message <- message + String.Concat(" \r\nPort ", port, " is open.")
            else
                message <- message + String.Concat(" \r\nPort ", port, " is closed.")
        
        //Print message diagonostics        
        printf "%s \r\n" message

        if (containsEmail && not errorOnly) then
            let email = results.GetResult Email
            message <- message.Replace("\r\n", "</p><p>")
            SendMail email message

    | _ -> printfn "%s" fpokeUsage
    
    0 