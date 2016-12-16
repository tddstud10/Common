// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.Testing
open System
open System.IO

MSBuildDefaults <- { MSBuildDefaults with Verbosity = Some MSBuildVerbosity.Minimal }

// Directories
let packagesDir = "./packages/"
let buildDir  = @"./build/"
let testDir  = @"./build/"
let nugetDir = @"./NuGet/"
ensureDirExists (directoryInfo nugetDir)

// Filesets
let solutionFile = "Common.sln"

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir]
)

Target "Rebuild" DoNothing

Target "Build" (fun _ ->
    !! solutionFile
    |> MSBuild buildDir "Build"
         [
            "Configuration", "Debug"
            "Platform", "Any CPU"
         ]
    |> ignore
)

Target "GitLink" (fun _ ->
    let gitLink = (packagesDir @@ @"gitlink" @@ "lib" @@ "net45" @@ "GitLink.exe")
    let args = sprintf "%s -f %s -d %s" __SOURCE_DIRECTORY__ solutionFile buildDir
    let ret =
        ExecProcessAndReturnMessages (fun info ->
            info.FileName <- gitLink
            info.Arguments <- args) (TimeSpan.FromSeconds 30.0)
    let consoleOutput =
        ret.Messages
        |> Seq.append ret.Errors
    consoleOutput
    |> Seq.iter (printfn "%s")
    let loadFailures =
        consoleOutput
        |> Seq.filter (fun m -> m.ToLowerInvariant().Contains("failed to load project"))
    if not ret.OK || not (Seq.isEmpty loadFailures) then failwith (sprintf "GitLink.exe \"%s\" task failed.\nErrors:\m %A" args loadFailures)
)

let runTest pattern =
    fun _ ->
        !! (buildDir + pattern)
        |> xUnit (fun p ->
            { p with
                ToolPath = findToolInSubPath "xunit.console.exe" (currentDirectory @@ "tools" @@ "xUnit")
                WorkingDir = Some testDir })

Target "Test" DoNothing
Target "UnitTests" (runTest "/*.UnitTests*.dll")

Target "Package" (fun _ ->
    let buildDirRel = sprintf @"..\build\%s"

    "Common.nuspec"
    |> NuGet (fun p -> 
        { p with               
            Authors = [ "The TddStud10 Team" ]
            Project = "TddStud10.Common"
            Description = "TddStud10 Common"
            Version = EnvironmentHelper.environVarOrDefault "GitVersion_NuGetVersion" "0.0.0"
            Dependencies = [ "FSharp.Core", GetPackageVersion packagesDir "FSharp.Core" ]
            Files = [ buildDirRel "R4nd0mApps.TddStud10.Common.Domain.dll", Some "lib", None
                      buildDirRel "R4nd0mApps.TddStud10.Common.Domain.pdb", Some "lib", None
                      buildDirRel "R4nd0mApps.TddStud10.Logger.dll", Some "lib", None
                      buildDirRel "R4nd0mApps.TddStud10.Logger.pdb", Some "lib", None ]
            OutputPath = buildDir })
)

Target "Publish" (fun _ ->
    !! "build/*.nupkg"
    |> AppVeyor.PushArtifacts
)

"Clean" ?=> "Build"
"Clean" ==> "Rebuild" 
"Build" ==> "Rebuild" 
"Build" ?=> "UnitTests" ==> "Test"
"Rebuild" ==> "Test"
"GitLink" ==> "Package"
"Test" ?=> "GitLink"
"Test" ==> "Package" ==> "Publish"

// start build
RunTargetOrDefault "Test"
