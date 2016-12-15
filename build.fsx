// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.Testing
open System
open System.IO

MSBuildDefaults <- { MSBuildDefaults with Verbosity = Some MSBuildVerbosity.Minimal }

// Directories
let buildDir  = @"./build/"
let testDir  = @"./build/"

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
            "CreateVsixContainer", "true"
            "DeployExtension", "false"
            "CopyVsixExtensionFiles", "false"
         ]
    |> ignore
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

Target "Publish" (fun _ ->
    !! "build\*.nupkg"
    |> AppVeyor.PushArtifacts
)

"Clean" ?=> "Build"
"Clean" ==> "Rebuild" 
"Build" ==> "Rebuild" 
"Build" ?=> "UnitTests" ==> "Test"
"Rebuild" ==> "Test"
"Test" ==> "Publish"

// start build
RunTargetOrDefault "Test"
