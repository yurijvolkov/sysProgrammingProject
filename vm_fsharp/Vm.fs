module Vm

open System
open Microsoft.FSharp.Core
open System.Reflection
open Types
open System.IO
open Loader
open Funcs


[<EntryPoint>]
let main argv = 
    let files = ["tests/call.dwc"; "tests/print.dwc"];
    let vm = vmInit files
    printfn "%A" vm
    0 // возвращение целочисленного кода выхода
