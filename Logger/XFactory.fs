module R4nd0mApps.TddStud10.Logger.XFactory

open System.IO
open System.Reflection
open System

let X<'T> typeName nullX : 'T = 
    let isDF() = Assembly.GetExecutingAssembly().GetName().Name.EndsWith(".DF", StringComparison.Ordinal)

    let dir = () |> Path.getExecutingAssemblyPath
    
    let file = 
        sprintf "%s.Windows%s.dll" 
            (Assembly.GetExecutingAssembly().GetName().Name.Replace(".DF", "")) 
            (if isDF() then ".DF" else "")
    
    let path = Path.Combine(dir, file)
    if File.Exists path then 
        Assembly.LoadFrom(path)
        |> fun a -> a.GetType(typeName)
        |> fun t -> t.GetProperty("I", System.Reflection.BindingFlags.NonPublic ||| BindingFlags.Static)
        |> fun f -> f.GetValue(null) :?> 'T
    else nullX
