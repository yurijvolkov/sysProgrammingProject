module Types
    /// <summary>
    /// Supported types
    /// </summary>
    type Type = 
        Int32 = 0uy
        |Double = 1uy
        
    /// <summary>
    /// Describe function
    /// </summary>
    type Function =  {
        fileId : int64
        nameId : int64
        localsCount : int64
        flags : int64
        argsCount : int64
        args : byte []
        byteCodeSize : int64
        code : byte []
    }

    /// <summary>
    /// Describe program that currently executing
    /// 'stringPool' : (fileId, stringNumber, string)
    /// </summary>
    type Program = {
        functions : Function list
        stringPool : (int64 * int64 * string) list 
    }

    
    module Stack = 
        type 'a stack = 'a list
        let head = function 
            |[] -> None
            |h :: _ -> Some(h) 
        let push s a = a::s
        let pop = function
            |[] -> None
            |h::t -> Some(t)
        let empty = []

    /// <summary>
    /// Describe virtual machine than currently 
    /// executing
    /// </summary>
    type VM = {
        dataStack : Stack.stack<int>;
        program : Program
    } 


