module R4nd0mApps.TddStud10.Common.Domain.PathBuilderTests

open R4nd0mApps.TddStud10.Common.Domain
open Xunit
open System
open R4nd0mApps.TddStud10.Common

let IsMSCLR = ("Mono.Runtime" |> Type.GetType |> isNull);

let inline (~~) s = FilePath s
let (/) a b = System.IO.Path.Combine(a,b)
let C = if IsMSCLR then "c:" else "" 
let D = if IsMSCLR then "d:" else "" 
let X = if IsMSCLR then "x:" else "" 

let ``Test Data for - Tests for makeSlnSnapshotPath`` : obj array seq = 
    [ C/"folder"/"file.sln", D/"tddstud10"/"folder"/"file.sln"
      X/"file.sln", D/"tddstud10"/"file"/"file.sln" ] 
    |> Seq.map (fun (a, b) -> [| box a; box b |])    

[<Theory>]
[<MemberData("Test Data for - Tests for makeSlnSnapshotPath")>]
let ``Tests for makeSlnSnapshotPath`` (slnPath, snapShotPath) =
    let (FilePath sp) = PathBuilder.makeSlnSnapshotPath (FilePath <| D/"tddstud10") (FilePath slnPath)
    Assert.Equal<string>(snapShotPath, sp)

let ``Test Data for - Tests for makeSlnBuildRoot`` : obj array seq = 
    [ C/"folder"/"file.sln", D/"xxx"/"folder"/"out"
      X/"file.sln", D/"xxx"/"file"/"out" ]
    |> Seq.map (fun (a, b) -> [| box a; box b |])    

[<Theory>]
[<MemberData("Test Data for - Tests for makeSlnBuildRoot")>]
let ``Tests for makeSlnBuildRoot`` (slnPath, buildRoot) = 
    let (FilePath sp) = PathBuilder.makeSlnBuildRoot (FilePath <| D/"xxx") (FilePath slnPath)
    Assert.Equal<string>(buildRoot, sp)

let ``Test Data for - Tests for rebaseCodeFilePath`` : obj array seq = 
    [ C/"sln"/"sln.sln", D/"tddstud10"/"sln"/"sln.sln", D/"tddstud10"/"sln"/"proj"/"a.cpp", C/"sln"/"proj"/"a.cpp"
      C/"sln"/"sln.sln", D/"tddstud10"/"sln"/"sln.sln", D/"tddstud10x"/"sln"/"proj"/"a.cpp", D/"tddstud10x"/"sln"/"proj"/"a.cpp" ]
    |> Seq.map (fun (a, b, c, d) -> [| box a; box b; box c; box d |])    

[<Theory>]
// NOTE: We dont support cases where the sln file is at drive level.
[<MemberData("Test Data for - Tests for rebaseCodeFilePath")>]
let ``Tests for rebaseCodeFilePath`` (slnPath, slnSnapPath, inp, outp) =
    let p = PathBuilder.rebaseCodeFilePath (~~slnPath, ~~slnSnapPath) ~~inp
    Assert.Equal(~~outp, p)
