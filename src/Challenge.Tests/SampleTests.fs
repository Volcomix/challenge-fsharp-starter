module SampleTests

open System
open Xunit

[<Fact>]
let ``Hello, World!`` () =
    Assert.Equal("Hello, World!", Sample.hello "World")
