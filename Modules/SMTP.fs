namespace Fpoke.Modules

open FSharp.Data
open System.Net.Mail

module SMTP = 
    type SmtpConnection = XmlProvider<"<connection><host></host><sender></sender><username></username><password></password><port></port></connection>">

    let smtpConnection = SmtpConnection.Parse("./Connections/SmtpConnection.xml")

    let server = smtpConnection.Host.XElement.Value
    let sender = smtpConnection.Sender.XElement.Value
    let username = smtpConnection.Username.XElement.Value
    let password = smtpConnection.Password.XElement.Value
    let port = smtpConnection.Port.XElement.Value |> int 

    let SendMail email message =
        let msg = 
            new MailMessage(sender, email, "Fpoke - Report", "<h1>F# Report</h1> <p>" + message + "</p>")

        msg.IsBodyHtml <- true

        let client = new SmtpClient(server, port)
        client.EnableSsl <- true
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