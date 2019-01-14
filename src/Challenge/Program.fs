(* Auto-generated code below aims at helping you parse *)
(* the standard input according to the problem statement. *)
open System


(* game loop *)
while true do
    eprintfn "What is your name?"
    Console.ReadLine ()
    |> Sample.hello
    |> printfn "%s\n"

    
    (* Write an action using printfn *)
    (* To debug: eprintfn "Debug message" *)