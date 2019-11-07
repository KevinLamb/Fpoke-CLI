namespace Fpoke.Modules

open FSharp.Data
open System
open System.Net.Mail

module SMTP = 
    type SmtpConnection = XmlProvider<"""<connection><host></host><sender></sender><username></username><password></password><port></port><enablessl></enablessl></connection>""">
    
    let smtpConfigPath = Environment.CurrentDirectory + "\\Config\\SmtpConnection.xml"
    
    let smtpConnection = SmtpConnection.Load(smtpConfigPath)

    let server = smtpConnection.Host.XElement.Value
    let sender = smtpConnection.Sender.XElement.Value
    let username = smtpConnection.Username.XElement.Value
    let password = smtpConnection.Password.XElement.Value
    let port = smtpConnection.Port.XElement.Value |> int 

    //Default to true if not specified
    let enableSsl = smtpConnection.Enablessl.XElement.Value <> "false"

    let SendMail email message important =
        let msg = 
            new MailMessage(sender, email, 
                "Fpoke - Report", "<h1>Fpoke Report</h1> <p>" + message + "</p> 
                <style>
                    h1, p {
                        font-family: Arial, sans-serif;
                    }
                </style>")

        msg.IsBodyHtml <- true

        //Check if we should make this as an important email (aka errors)
        if(important) then
            msg.Priority <- MailPriority.High

        let client = new SmtpClient(server, port)
        client.EnableSsl <- enableSsl
        client.Credentials <- System.Net.NetworkCredential(username, password)
        client.SendCompleted |> Observable.add(fun e -> 
            let msg = e.UserState :?> MailMessage
            if e.Cancelled then
                printfn "Mail message cancelled"
            if e.Error <> null then
                printfn "Sending mail failed for message:\r\n reason:\r\n %A" e.Error
            if msg<>Unchecked.defaultof<MailMessage> then msg.Dispose()
            if client<>Unchecked.defaultof<SmtpClient> then client.Dispose()
        )
        
        client.Send(msg)

        printfn "Report sent to email"      