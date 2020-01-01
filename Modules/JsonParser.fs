namespace Fpoke.Modules

open FSharp.Data
open System.IO

module JsonParser = 
    
    type Urls = JsonProvider<"./Config/urls.json">

    let ParseUrls : string array =

        let urlsJson = Urls.Load "./Config/urls.json"

        urlsJson.Urls