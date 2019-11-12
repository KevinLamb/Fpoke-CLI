namespace Fpoke.Modules

open FSharp.Data
open System
open System.IO
open System.Net.Sockets
open System.Net
open System.Collections.Generic

module Poke =

    let infoCodes = [100 .. 199]
    let goodCodes = [200 .. 299]
    let redirectCodes = [300 .. 399]
    let clientErrorCodes = [400 .. 499]
    let serverErrorCodes = [500 .. 599]

    let Advice = dict[
        //100
        100, "Continue to a proper request. Manually verify that the content does load.";

        101, "Switching Protocols";

        102, "Processing";

        103, "This is a response to load things earlier than when the server is ready to process the main content."; 

        //200
        200, "Received an OK response from the server.";

        201, "Created a resource during the successful request";

        202, "The request was accepted but not acted upon";

        203, "The request has been transformed by a proxy to a 200 request.";

        204, "The connection was successfully but there is no need to go away from the previous request.";

        205, "The server wants the client to refresh but the connection was successful.";

        206, "The server responded with partial content.";

        //300
        300, "Double check for typos in the URL, this is a redirect response from the server.";

        301, "This is a permanent redirect. You might want to check the resulting URL.";

        302, "This is a redirect to temporary content. Usually this is nothing to worry about.";

        303, "This is usually a redirect to see newer content. Usually this is nothing to worry about.";

        304, "This is a redirect, usually to cached content. Usually nothing to worry about.";

        307, "This is a temporary redirect. Usually nothing to worry about.";

        308, "This is a permanent redirect. You might want to check the resulting URL.";

        //400
        400, "The URL cannot be parsed by the server resulting in a bad request. Check the URL provided.";

        401, "Authentication is needed to connect to the server.";

        402, "A payment is required to connect to the server.";

        403, "The link provided is forbidden by the server. Check the access of the directory / web server settings.";

        404, "The server can not find the content connected to the provided link. Check the link to make sure it's correct.";

        405, "The server does not support the request method used by fpoke. Please check the server and URL.";

        406, "The server will not produce a response with the acceptable values. Check the server settings or follow the URL to see if there is a list of acceptable values provided.";

        407, "The link leads to a proxy that requires authentication.";

        408, "The request timed out... the server might be down.";

        409, "There was a conflict with the request to the server. Check the state of the server.";

        410, "The URL provided leads to content that no longer exists. Check the URL provided and the server it connects to.";

        411, "The server needs a defined Content-Length to return a response. Check the server and fix the settings.";

        412, "The request failed due to the type of request. Check that a the server needs a method other than GET to access the URL.";

        413, "The payload by fpoke was too large for the server's limits. Check the server's configurations.";

        414, "The URL is too long for the server. Check the link character length and compare to the server's acceptable URL limits";

        415, "The media type is not supported by the server. Check the server's configurations.";

        416, "The server can not serve the requested range in the Range header value. Check the server's configuration.";

        417, "The expectation in fpoke's response could not be met. Check the server's configuration.";

        418, "The server is a teapot. Make sure your URL is going to a valid coffee pot.";

        //500
        500, "There's a server side error. Check the server for errors.";

        501, "The server doesn't have the functionality to complete the request. Check the server for errors.";

        502, "The server is failing while acting as a proxy or gateway. Check the servers for errors.";

        503, "The server can not handle the request and service is unavailable. Check the server for errors.";

        504, "The server acting as a gateway or proxy did not get a response in time. Check the servers related to the URL.";
    ]

    let GetStatus url = 
        try
            let response = Http.Request(url, silentHttpErrors = true)
            Some response.StatusCode
        with :? System.Net.WebException as ex -> printfn "There was an error creating the request: %s!" ex.Message; None

    let GetPortStatus (url: string, port: int): bool = 

        let request = WebRequest.Create(url)
        use response = request.GetResponse()

        let host = response.ResponseUri.Host

        let connected =
            try
                use client = new TcpClient()
                let result = client.BeginConnect(host, port, null, null)
                let success = result.AsyncWaitHandle.WaitOne(3000)

                match success with
                | false -> false
                | true ->
                       client.EndConnect(result)
                       true
            with
            | _ -> (); false
        connected
    
    let CreateResponseMessage url status : string = 
        let mutable message = ""

        match status with
            | stat when List.contains stat infoCodes -> 
                    message <- sprintf "The site (%s) is UP! \r\nHTTP Status: %i" url status

            | stat when List.contains stat goodCodes ->
                    message <- sprintf "The site (%s) is UP! \r\nHTTP Status: %i" url status       

            | stat when List.contains stat redirectCodes -> 
                    message <- sprintf "The site (%s) is UP! \r\nHTTP Status: %i" url status 

            | stat when List.contains stat clientErrorCodes -> 
                message <- sprintf "The page (%s) is not found or page is down... \r\nHTTP Status: %i" url status 

            | stat when List.contains stat serverErrorCodes -> 
                message <- sprintf "SERVER ERROR: The site (%s) is DOWN! \r\nHTTP Status: %i" url status     

            | _ -> printfn "An error occurred while getting status..."

        message <- message + String.Concat(" \r\nDescription: ", Advice.[status])

        message
    
    let CreatePortMessage url port : string = 
        let mutable message = ""

        let portConnected = GetPortStatus(url, port)

        if(portConnected) then
                message <- sprintf "\r\nPort %i is open." port
            else
                message <- sprintf "\r\nPort %i is closed." port

        message
        

    let CheckError status : bool = 
        let mutable error = false

        match status with
            | stat when List.contains stat clientErrorCodes -> 
                error <- true

            | stat when List.contains stat serverErrorCodes -> 
                error <- true     
            |_ ->
                error <- false
                
        error

    