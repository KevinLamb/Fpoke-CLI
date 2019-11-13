open Argu
open Fpoke.Modules.Poke

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

    | _ -> printfn "%s" fpokeUsage
    
    0 