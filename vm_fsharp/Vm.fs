﻿module Vm

open System
open Microsoft.FSharp.Core
open System.Reflection
open Types
open System.IO
open Loader
open Funcs
open Exec

[<EntryPoint>]
let main argv = 
    let files = [ "tests/2nd.dwc"];
    let vm = vmInit files
    //printfn "%A" vm
    //printfn "%A" (List.length vm.context.Head.command)
    execute vm
    0 // возвращение целочисленного кода выхода
