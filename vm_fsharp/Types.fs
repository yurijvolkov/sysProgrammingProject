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
        args : byte list
        byteCodeSize : int64
        code : byte list
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

    type VmCtx = {
        command : byte list;
        func : Function;
        locals : int64[];
    }

    /// <summary>
    /// Describe virtual machine than currently 
    /// executing
    /// </summary>
    type Vm = {
        context : VmCtx list;
        dataStack : Stack.stack<int>;
        stringPool : (int64 * int64 * string) list ;
        functions : Function list;
     } 
    
