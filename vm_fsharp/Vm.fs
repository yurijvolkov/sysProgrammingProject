module Vm

open Types
open Loader
open Exec

[<EntryPoint>]
let main argv = 
    let vm = vmInit ["tests/test.vtc"]
    execute vm
    0 