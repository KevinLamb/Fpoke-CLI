// Learn more about F# at http://fsharp.org

open System
open FSharp.Data

let getStatus url = 
    try
        let response = Http.Request(url, silentHttpErrors = true)
        Some response.StatusCode
    with
    | :? System.Net.WebException as ex -> None

[<EntryPoint>]
let main argv =
    printfn "Input url to check status of:"
    let url = Console.ReadLine()

    let status = getStatus url

    match status.IsSome with
    | true -> printfn "HTTP Status: %A" status.Value
    | _ -> printfn "An error occurred while getting status..."

    let line = Console.ReadLine()
    0 // return an integer exit code
