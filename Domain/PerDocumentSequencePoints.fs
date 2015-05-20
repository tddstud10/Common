﻿namespace R4nd0mApps.TddStud10.Common.Domain

open System.Collections.Concurrent
open System.Runtime.Serialization.Formatters.Binary
open System.IO
open System
open System.Collections.Generic

[<Serializable>]
type PerDocumentSequencePoints = 
    inherit DataStoreBase<FilePath, List<SequencePoint>>
    
    new() = 
        { inherit DataStoreBase<_, _>() }
        then ()
    
    static member public Deserialize path = DataStoreBase<_, _>.Deserialize<PerDocumentSequencePoints> (path)
