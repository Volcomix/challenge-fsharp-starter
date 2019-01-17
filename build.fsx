#r "paket:
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem
nuget Fake.Core.Xml
nuget Fake.Core.Target //"
#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

let buildDir = "build"
let outFile = buildDir </> "Challenge.fs"
let srcDir = "src/Challenge"
let projFile = srcDir </> "Challenge.fsproj"
let testsProjFile = srcDir + ".Tests/Challenge.Tests.fsproj"

let parseLine (opens, lines, isModule) line =
    match line with
    | "" when isModule && List.head lines = "    " -> opens, lines, isModule
    | _ when String.startsWith "open " line -> Set.add line opens, lines, isModule
    | _ when String.startsWith "module " line -> opens, line + " =" :: lines, true
    | _ when isModule -> opens, "    " + line :: lines, isModule
    | _ -> opens, line :: lines, isModule

let parseFile (opens, lines) file =
    let opens, lines, _ =
        File.read file
        |> Seq.fold parseLine (opens, "" :: lines, false)
    opens, lines

let merge _ =
    let files =
        Xml.read true projFile "" "" "//Compile/@Include"
        |> Seq.map ((</>) srcDir)
    Trace.logItems "Merging: " files
    let opens, lines = Seq.fold parseFile (Set.empty, []) files
    List.rev lines
    |> Seq.append opens
    |> File.writeNew outFile
    Trace.tracefn "Output: %s" outFile

Target.create "Clean" (fun _ ->
    !! "src/**/bin"
    ++ "src/**/obj"
    ++ buildDir
    |> Shell.cleanDirs)

Target.create "Build" (fun _ ->
    DotNet.build id projFile)

Target.create "BuildTest" (fun _ ->
    DotNet.build id testsProjFile)

Target.create "Test" (fun _ ->
    DotNet.test id testsProjFile)

Target.create "Merge" (fun _ ->
    Shell.mkdir buildDir
    merge ())

Target.create "Watch" (fun _ ->
    use __ =
        !! (srcDir </> "**/*.fs")
        |> ChangeWatcher.run merge
    System.Console.ReadLine() |> ignore)

Target.create "All" ignore

"Clean" ==> "All"
"Build"  ==> "All"
"BuildTest" ==> "All"
"Test" ==> "All"
"Merge" ==> "All"
"Merge" ==> "Watch"

"Clean"
    ?=> "Build"
    ?=> "BuildTest"
    ?=> "Test"
    ?=> "Merge"

Target.runOrDefault "Watch"
