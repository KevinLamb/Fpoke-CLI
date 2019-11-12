open Argu
open Fpoke.Modules.Poke
open Fpoke.Modules.SMTP
open System

type FpokeArguments =
    | [<MainCommand>] URL of url:string
    | [<AltCommandLine("-email")>] Email of email:string
    | [<AltCommandLine("-error")>] ErrorOnly
    | [<AltCommandLine("-p")>] Port of port:int
    | [<AltCommandLine("-list")>] UrlList
    with
        interface IArgParserTemplate with
            member fpoke.Usage =
                match fpoke with
                | URL _ -> "The URL of the site you're checking."
                | Email _ -> "The email you want to send a report to. (Make sure to set up SMTP connection)"
                | ErrorOnly _ -> "A setting for only sending an email report upon error codes."
                | Port _ -> "The port you want to ping on the site's server."
                | UrlList _ -> "Grab from the list provided in the urls.json file."
                
[<EntryPoint>]
let main arg =

    let mutable message = ""
    let mutable error = false

    let argParser = ArgumentParser.Create<FpokeArguments>(programName = "fpoke")

    let results = argParser.ParseCommandLine(arg)
    
    let containsEmail = results.Contains Email
    let errorOnly = results.Contains ErrorOnly
    let urlList = results.Contains UrlList
    let portCheck = results.Contains Port

    let fpokeUsage = argParser.PrintUsage()
    
    match results.Contains URL with
    | true -> 

        //Check Status and send email
        let url = results.GetResult URL

        let statusCode = GetStatus url

        match statusCode with
        | Some status -> 
            error <- CheckError status

            if(not errorOnly || error) then
                message <- CreateResponseMessage url status

        | None ->
                message <- "SERVER ERROR: Site is DOWN! \r\nUnable to connect to the server."
                

        
        //Check port
        if(portCheck) then
            let port = results.GetResult Port
            
            if(not errorOnly || error) then
                message <- message + CreatePortMessage url port

            
        //Print message diagonostics        
        printf "%s \r\n" message

        if (containsEmail) then
            let email = results.GetResult Email

            message <- message.Replace("\r\n", "</p><p>")

            if(not errorOnly || error) then
                SendMail email message error

    | _ -> printfn "%s" fpokeUsage
    
    0 