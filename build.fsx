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

Target.create "Clean" (fun _ ->
    !! "src/**/bin"
    ++ "src/**/obj"
    ++ buildDir
    |> Shell.cleanDirs)

Target.create "Build" (fun _ ->
    !! "src/Challenge/*.*proj"
    |> Seq.iter (DotNet.build id))

Target.create "BuildTest" (fun _ ->
    !! "src/Challenge.Tests/*.*proj"
    |> Seq.iter (DotNet.build id))

Target.create "Merge" (fun _ ->
    let files =
        Xml.read true "src/Challenge/Challenge.fsproj" "" "" "//Compile/@Include"
        |> Seq.map ((</>) "src/Challenge")
    Trace.logItems "Merging: " files
    let opens, lines = Seq.fold parseFile (Set.empty, []) files
    Shell.mkdir buildDir
    List.rev lines
    |> Seq.append opens
    |> File.writeNew (buildDir </> "Challenge.fs"))

Target.create "All" ignore

"Clean" ==> "All"
"Build"  ==> "All"
"BuildTest" ==> "All"
"Merge" ==> "All"

Target.runOrDefault "All"
