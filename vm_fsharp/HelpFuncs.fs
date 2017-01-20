module HelpFuncs

open Types

let head = function
    |h::t -> h
let tail = function
    |h::t -> t

let fst3 = function 
    |(a,b,c) -> a
let snd3 = function
    |(a,b,c) -> b
let thd3 = function
    |(a,b,c) -> c

/// <summary>
/// Returns string from pool with given parameters : fileId and nameId
/// </summary>
/// <param name="pool">String pool</param>
/// <param name="fileId">File Id that contains string</param>
/// <param name="nameId">Id of string in file</param>
let getStr pool fileId nameId =
    let s = List.find( fun s -> fst3 s = fileId && snd3 s = nameId ) pool
    thd3 s