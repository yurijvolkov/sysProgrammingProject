module Types

open System
open Commands
    
    /// <summary>
    /// Supported types in virtual machine
    /// Some of them not used by commands, but VmValue support
    /// them, so it's easy to extend VM with commands with corresponding types
    /// </summary>
    type SupportedTypes = 
        | Int16
        | Int32
        | Int64
        | String
        | Single 
        | Double
        | Boolean
        | Char
        | UInt16
        | UInt32
        | UInt64

    /// <summary>
    /// Describe function
    /// </summary>
    type Function =  {
        fileId : int64 
        nameId : int64
        localsCount : int64 //count of local variables
        flags : int64
        argsCount : int64
        args : byte list //types of args (Look : enum SupportedTypes)
        byteCodeSize : int64
        code : byte list
    }
    exception StackError of string

    // Generic Stack
    module Stack = 
        type 'a stack = 'a list
        let head = function 
            |[] -> None
            |h :: _ -> Some(h) 
        let push s a = a::s
        let pop = function
            |h::t -> t
            |[] -> raise (StackError("Pop on empty stack"))
        let pop2 = function
            |[] -> []
            |h::[] -> []
            |h1::h2::t -> t

        let empty = []
    
    /// <summary>
    /// Common value that used in VM
    /// </summary>
    type VmValue = { arr : byte[] }
        with
            static member Cons (arr : byte[]) = {arr=arr} 
            member v.ToInt16() = BitConverter.ToInt16(v.arr,0)
            member v.ToInt32() = BitConverter.ToInt32(v.arr,0)
            member v.ToInt64() = BitConverter.ToInt64(v.arr,0)
            member v.ToBoolean() = BitConverter.ToBoolean(v.arr,0)
            member v.ToChar() = BitConverter.ToChar(v.arr, 0)
            member v.ToSingle() = BitConverter.ToSingle(v.arr, 0)
            member v.ToDouble() = BitConverter.ToDouble(v.arr, 0)
            member v.ToUInt16() = BitConverter.ToUInt16(v.arr, 0)
            member v.ToUInt32() = BitConverter.ToUInt32(v.arr, 0)
            member v.ToUInt64() = BitConverter.ToUInt64(v.arr,0)
            member v.ToStrPtr() = (BitConverter.ToInt32(v.arr,0),
                                   BitConverter.ToInt32(v.arr,4))
    
    /// <summary>
    /// Function context defines current state of execution :
    /// local variables and list of commands to execute
    /// </summary>
    type VmCtx = {
        command : byte list;
        func : Function;
        locals : VmValue[];
    }

    /// <summary>
    /// Describe virtual machine than currently 
    /// executing
    /// </summary>
    type Vm = {
        context : VmCtx list;
        dataStack : Stack.stack<VmValue>;
        stringPool : (int64 * int64 * string) list ; //(file Id * name Id * string)
        functions : Function list;
     } 

     /// <summary>
    /// Contains offsets for some elements (but not all) in byte code
    /// </summary>
    type ByteCodeOffsets = 
        |SignatureOffset = 0L
        |VersionOffset = 2L
        |FunctionsStartOffset = 10L
        |StringPoolCountOffset = 18L
        |StringsOffset = 26L

    /// <summary>
    /// Contains offsets for some elements (but not all) in part
    /// of byte code which describes Function
    /// </summary>
    type FunctionElementsOffsets = 
        |Name = 0L
        |LocalsCount = 8L
        |Flags = 16L
        |ArgsCount = 24L
        |ArgsTypes = 32L


    
