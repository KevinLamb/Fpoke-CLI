namespace Fpoke.Modules

open FSharp.Data
open System
open System.IO
open System.Net.Sockets
open System.Net

module Poke =
    
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