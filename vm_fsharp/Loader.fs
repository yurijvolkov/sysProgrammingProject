﻿module Loader

open System.IO
open System
open HelpFuncs
open Types


/// <summary>
/// Read and return one byte from 'fs' with given offset 
/// from the start of file 
/// </summary>
/// <param name="fs">Filestream to read from</param>
/// <param name="offset">Offset from begin of stream</param>
let getByte (fs : FileStream) (offset : int64) =
    let curPos = fs.Position
    fs.Seek(offset, SeekOrigin.Begin) |> ignore
    let br = new BinaryReader(fs)
    let value = br.ReadByte()
    fs.Seek(curPos, SeekOrigin.Begin) |> ignore
    value

/// <summary>
/// Read and return 'count' bytes from 'fs' with given offset
/// </summary>
/// <param name="fs">Filestream to read from</param>
/// <param name="offset">Offset from start of stream</param>
/// <param name="count">Number of bytes to read</param>
let getBytes (fs : FileStream) (offset : int64) (count : int) = 
    let curPos = fs.Position
    fs.Seek(offset, SeekOrigin.Begin) |> ignore
    let br = new BinaryReader(fs)
    let value = br.ReadBytes(count)
    fs.Seek(curPos, SeekOrigin.Begin) |> ignore
    Array.toList value

/// <summary>
/// Read and return 8byte number from 'fs' with given offset 
/// </summary>
/// <param name="fs">Filestream to read from</param>
/// <param name="offset">Offset from begin of stream</param>
let getInt64 (fs : FileStream) (offset : int64) =
    let curPos = fs.Position
    fs.Seek(offset, SeekOrigin.Begin) |> ignore
    let br = new BinaryReader(fs)
    let value = br.ReadInt64()
    fs.Seek(curPos, SeekOrigin.Begin) |> ignore
    value

/// <summary>
/// Returns true if magic numbers ok
/// </summary>
/// <param name="fs">Filestream to read from</param>
let checkFileSignature (fs : FileStream) = 
    let magics = [getByte fs (int64 ByteCodeOffsets.SignatureOffset) ;
                  getByte fs (int64 ByteCodeOffsets.SignatureOffset + 1L) ]
    match magics with 
        |[0xBAuy; 0xBAuy] -> true
        |_ -> false

/// <summary>
/// Returns version number of VM to load 
/// </summary>
/// <param name="fs">Filestream to read from</param>
let getVersion (fs : FileStream) =
    getInt64 fs (int64 ByteCodeOffsets.VersionOffset)

/// <summary>
/// Returns offset where functions are
/// </summary>
/// <param name="fs">Filestream to read from</param>
let getFunctionsOffset (fs : FileStream) =
    getInt64 fs (int64 ByteCodeOffsets.FunctionsStartOffset)

/// <summary>
/// Returns offset where sting pool starts
/// </summary>
/// <param name="fs">Filestream to read from</param>
let getStringPoolCount (fs : FileStream) = 
    getInt64 fs (int64 ByteCodeOffsets.StringPoolCountOffset)

/// <summary>
/// Returns list contains all strings in pool
/// </summary>
/// <param name="fs">Filestream to read from</param>
let getStringPool (fs : FileStream) (fileId : int64) = 
    let curPos = fs.Position
    fs.Seek(int64 ByteCodeOffsets.StringsOffset, SeekOrigin.Begin) |> ignore
    let br = new BinaryReader(fs)
    
    let rec _getStringPool (fs : FileStream) acc  (count : Int64)  =
        if(count > 0L) then 
            let rec nextStr cur =
                let c = br.ReadChar()
                if(int c<>0) then nextStr (cur + c.ToString())
                else cur
            _getStringPool fs ((fileId, getStringPoolCount fs - count, nextStr "")::acc) (count-1L)
        else acc
    let pool = _getStringPool fs [] (getStringPoolCount fs)
    fs.Seek(curPos, SeekOrigin.Begin) |> ignore
    pool

/// <summary>
/// Returns count of all functions in current program
/// </summary>
/// <param name="fs">Filestream to read from</param>
let getFunctionsCount (fs : FileStream) = 
    let offset = getFunctionsOffset fs
    (getInt64 fs offset)

/// <summary>
/// Parse and returns function with current offset from 'fs'
/// </summary>
/// <param name="fs">Filestream to read frome</param>
let getFunction (fs  : FileStream) (fileId : int64) =
    let nameId = getInt64 fs (int64 FunctionElementsOffsets.Name + fs.Position)
    let localsCount = getInt64 fs (int64 FunctionElementsOffsets.LocalsCount + fs.Position)
    let flags = getInt64 fs (int64 FunctionElementsOffsets.Flags + fs.Position)
    let argsCount = getInt64 fs (int64 FunctionElementsOffsets.ArgsCount + fs.Position)
    let args = getBytes fs (int64 FunctionElementsOffsets.ArgsTypes + fs.Position) (int argsCount)
    let byteCodeSize = getInt64 fs (argsCount +
                                (int64 FunctionElementsOffsets.ArgsTypes + fs.Position))
    let code = getBytes fs (argsCount + 8L +
                                (int64 FunctionElementsOffsets.ArgsTypes + fs.Position))
                                (int byteCodeSize)
    let endOffset = (argsCount + 8L +
                                (int64 FunctionElementsOffsets.ArgsTypes + fs.Position))
                                + byteCodeSize
    fs.Seek(endOffset, SeekOrigin.Begin) |> ignore
    { fileId = fileId; 
    nameId=nameId; localsCount=localsCount; flags=flags; argsCount = argsCount;
     args=args; byteCodeSize=byteCodeSize; code=code}

/// <summary>
/// Returns all functions of correcponding program
/// </summary>
/// <param name="fs">Filestream to read from</param>
let getFunctions (fs : FileStream) (fileId : int64) =
    let count = getFunctionsCount fs
    let curPos = fs.Position
    fs.Seek((getFunctionsOffset fs) + 8L,SeekOrigin.Begin) |> ignore
    let rec getFuncs acc curCount = 
        if(curCount > 0) then
            getFuncs ((getFunction fs fileId)::acc) (curCount-1)
        else acc
    let result = getFuncs [] (int count)
    fs.Seek(curPos, SeekOrigin.Begin) |> ignore
    result

/// <summary>
/// Parse file into two parts : list of functions and string pool
/// Returns tuple of them.
/// </summary>
/// <param name="files">List of pathes to source files</param>
let parseFiles files = 
    let rec _parseFiles files (curProg,curPool) fileId =
        match files with
        |[] -> (curProg, curPool)
        |h::t -> 
            use fs = new FileStream(h, FileMode.Open)
            let funcs = getFunctions fs fileId
            let pool = getStringPool fs fileId
            _parseFiles t (funcs@curProg, pool@curPool) (fileId + 1L)
    let res = _parseFiles files ([],[]) 0L
    (fst res, List.rev(snd res) )
       

/// <summary>
/// Initialize context with given function
/// </summary>
/// <param name="func">Function to use</param>
let vmCtxInit func =
    {command=func.code; func=func; locals=Array.zeroCreate (int func.localsCount)}

/// <summary>
/// Initialize virtual machine to execute one
/// </summary>
/// <param name="files">Files to read source code from</param>
let vmInit files = 
    let parse = parseFiles files
    let functions = fst parse
    let pool = snd parse
    let main = List.find (fun f -> getStr pool f.fileId f.nameId = "main" ) functions
    {context=[vmCtxInit main]; dataStack=Stack.empty;
     stringPool=pool; functions=functions}
