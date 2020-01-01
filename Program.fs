open Argu
open Fpoke.Modules.Poke
open Fpoke.Modules.JsonParser

[<EntryPoint>]
let main arg =

    let argParser = ArgumentParser.Create<FpokeArguments>(programName = "fpoke")

    let results = argParser.ParseCommandLine(arg)

    let fpokeUsage = argParser.PrintUsage()

    match results.Contains URL with
    | true -> 
        //Check Status and send email
        let url = results.GetResult URL

        CreateMessage url results 

    | _ -> printfn "Url not provided..."

    match results.Contains UrlList with
    | true -> 
        for url in ParseUrls do
            CreateMessage url results

    | _ -> printfn ""
    
    0 