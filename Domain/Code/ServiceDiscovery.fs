module R4nd0mApps.TddStud10.Common.ServiceDiscovery

open R4nd0mApps.TddStud10.Common.Domain
open System
open System.IO
open System.Threading

type UriShare = 
    | UriShare of FilePath

let fromString = FilePath >> UriShare
let toString (UriShare path) = path.ToString()
let createShare = Path.GetTempFileName >> fromString
let shareUri (UriShare share) uri = File.WriteAllText(share.ToString(), uri.ToString())

let readSharedUri (UriShare share) = 
    let rec readLoop path = 
        if FileInfo(path).Length > 0L then 
            path
            |> File.ReadAllText
            |> Uri
        else 
            Thread.Sleep(500)
            readLoop path
    
    let uri = share.ToString() |> readLoop
    share.ToString() |> File.Delete
    uri
