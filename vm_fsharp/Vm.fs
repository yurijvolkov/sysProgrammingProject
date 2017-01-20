module Vm

open Types
open Loader
open Exec

[<EntryPoint>]
let main argv = 
    let vm = vmInit (Array.toList argv)
    execute vm
    0 