module R4nd0mApps.TddStud10.Common.Domain.TypeConverterTests

open FsUnit.Xunit
open System
open System.ComponentModel
open global.Xunit

let ``Test Data for Type converter tests`` = 
    [ typeof<FilePath>, 
      "A file path "
      |> FilePath
      |> box
      typeof<DocumentLocation>, 
      { document = "some document" |> FilePath
        line = 100 |> DocumentCoordinate }
      |> box
      typeof<TestId>, 
      { source = "test source" |> FilePath
        location = 
            { document = "test file" |> FilePath
              line = 200 |> DocumentCoordinate } }
      |> box
      typeof<SequencePointId>, 
      { methodId = 
            { assemblyId = Guid.NewGuid() |> AssemblyId
              mdTokenRid = 555u |> MdTokenRid }
        uid = 100 }
      |> box ]
    |> Seq.map (fun (a, b) -> 
           [| box a
              box b |])

[<Theory>]
[<MemberData("Test Data for Type converter tests")>]
let ``Type converter tests`` (t, it) = 
    let strConverter = TypeDescriptor.GetConverter(typeof<string>)
    let str = strConverter.ConvertTo(it, typeof<string>)
    let converter = TypeDescriptor.GetConverter(t)
    converter.ConvertFrom(str) |> should equal it
