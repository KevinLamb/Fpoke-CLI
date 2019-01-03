namespace Fpoke.Modules

open System.Net.Mail

module SMTP = 
    let server = "smtp.mailtrap.io" 
    let sender = ""
    let username = ""
    let password = ""
    let port = 587

    let SendMail email message =
        let msg = 
            new MailMessage(sender, email, "Fpoke - Report", "<h1>" + message + "</h1>")

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