module R4nd0mApps.TddStud10.Common.DFizer

open System
open System.IO
open System.Reflection

let isDF() = Assembly.GetExecutingAssembly().GetName().Name.EndsWith(".DF", StringComparison.Ordinal)

let dfize s = sprintf "%s%s" s (if isDF() then ".DF" else "")

let dfizePath path = 
    let newExtn = 
        sprintf "%s%s" (if isDF() then ".DF"
                        else "") (Path.GetExtension path)
    (path, newExtn) |> Path.ChangeExtension
