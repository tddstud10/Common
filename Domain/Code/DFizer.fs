module R4nd0mApps.TddStud10.Common.DFizer

open System
open System.Reflection

let isDF() = Assembly.GetExecutingAssembly().GetName().Name.EndsWith(".DF", StringComparison.Ordinal)
