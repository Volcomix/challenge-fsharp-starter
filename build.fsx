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

Target.create "Clean" (fun _ ->
    !! "src/**/bin"
    ++ "src/**/obj"
    |> Shell.cleanDirs)

Target.create "Build" (fun _ ->
    !! "src/**/*.*proj"
    |> Seq.iter (DotNet.build id))

Target.create "Merge" (fun _ ->
    let files =
        Xml.read true "src/Challenge/Challenge.fsproj" "" "" "//Compile/@Include"
        |> Seq.map ((</>) "src/Challenge")

    Trace.logItems "Merging: " files
    Trace.traceLine ()

    let opens, lines =
        files
        |> Seq.fold (fun (opens, lines) file ->
            let opens, lines, _ =
                File.read file
                |> Seq.fold (fun (opens, lines, isModule) line ->
                    match line with
                    | "" when isModule && List.head lines = "    " -> opens, lines, isModule
                    | _ when String.startsWith "open " line -> Set.add line opens, lines, isModule
                    | _ when String.startsWith "module " line -> opens, line + " =" :: lines, true
                    | _ when isModule -> opens, "    " + line :: lines, isModule
                    | _ -> opens, line :: lines, isModule) (opens, lines, false)
            opens, "" :: lines) (Set.empty, [])

    Set.toSeq opens
    |> Trace.logItems "Opens: "

    Trace.trace ""

    List.rev lines
    |> Trace.logItems "Lines: ")

Target.create "All" ignore

"Clean"
  ==> "Build"
  ==> "All"

Target.runOrDefault "All"
