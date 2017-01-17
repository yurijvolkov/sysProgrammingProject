module Vm

open System
open Microsoft.FSharp.Core
open System.Reflection
open Types
open System.IO
open Loader

    
[<EntryPoint>]
let main argv = 
    let fs = new FileStream("tests/call.dwc", FileMode.Open)
    let br = new BinaryReader(fs) 
    br.ReadBytes(10) |> ignore
    let count = br.ReadInt64()
    
    printfn "%d" (getVersion fs)
    printfn "%b" (checkFileSignature fs)
    printfn "%d" count
    for x in (getStringPool fs) do
        printfn "%s" x 
    
    0 // возвращение целочисленного кода выхода
