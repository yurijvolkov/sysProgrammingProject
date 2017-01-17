module Loader

open System.IO
open System

let checkFileSignature (fs : FileStream) = 
    let curPos = fs.Position
    let br = new BinaryReader(fs)
    fs.Seek(0L,SeekOrigin.Begin) |> ignore
    let magics = br.ReadBytes(2)
    fs.Seek(curPos, SeekOrigin.Begin) |> ignore 
    match magics with 
        |[|0xBAuy; 0xBAuy|] -> true
        |_ -> false

let getVersion (fs : FileStream) =
    let curPos = fs.Position
    let br = new BinaryReader(fs)
    fs.Seek(2L, SeekOrigin.Begin) |> ignore
    let version = br.ReadInt64()
    fs.Seek(curPos, SeekOrigin.Begin) |> ignore
    version

let getStringPoolCount (fs : FileStream) = 
    let curPos = fs.Position
    let br = new BinaryReader(fs)
    fs.Seek(10L, SeekOrigin.Begin) |> ignore
    let count = br.ReadInt64()
    fs.Seek(curPos, SeekOrigin.Begin) |> ignore
    count

let getStringPool (fs : FileStream) = 
    let curPos = fs.Position
    fs.Seek(18L, SeekOrigin.Begin) |> ignore
    let br = new BinaryReader(fs)
    
    let rec _getStringPool (fs : FileStream) (acc : string list) (count : Int64)  =
        if(count > 0L) then 
            let rec nextStr cur =
                let c = br.ReadChar()
                if(int c<>0) then nextStr (cur + c.ToString())
                else cur
            _getStringPool fs ((nextStr "")::acc) (count-1L)
        else acc
    let pool = _getStringPool fs [] (getStringPoolCount fs)
    fs.Seek(curPos, SeekOrigin.Begin) |> ignore
    pool
