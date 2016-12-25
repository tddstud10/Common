module R4nd0mApps.TddStud10.Logger.XFactory

open R4nd0mApps.TddStud10.Common
open System
open System.IO
open System.Reflection

let private getLocalPath() = 
    Assembly.GetExecutingAssembly().CodeBase
    |> fun cb -> Uri(cb).LocalPath
    |> Path.GetFullPath
    |> Path.GetDirectoryName

let X<'T> typeName nullX : 'T = 
    let dir = () |> getLocalPath
    
    let file = 
        sprintf "%s.Windows%s.dll" 
            (Assembly.GetExecutingAssembly().GetName().Name.Replace(".DF", "")) 
            (if DFizer.isDF() then ".DF" else "")
    
    let path = Path.Combine(dir, file)
    if File.Exists path then 
        Assembly.LoadFrom(path)
        |> fun a -> a.GetType(typeName)
        |> fun t -> t.GetProperty("I", System.Reflection.BindingFlags.NonPublic ||| BindingFlags.Static)
        |> fun f -> f.GetValue(null) :?> 'T
    else nullX
