module Funcs

open Types


let fst = function 
    |(a,b,c) -> a
let snd = function
    |(a,b,c) -> b
let thd = function
    |(a,b,c) -> c

let getString vmProg fileId strId = 
     let s = List.find (fun t  -> (fst t)= fileId && (snd t)= strId) 
                        vmProg.stringPool
     thd s

let getFuncByName vmProg name = 
    List.find (fun f -> (getString vmProg f.fileId f.nameId) = name)
                         vmProg.functions
