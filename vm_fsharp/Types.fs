module Types


open System
    type Commands = 
        |INVALID=0uy    //Invalid instruction
        |LOAD=1uy       //Loads inlined value
        |LOADS=2uy      //Loads inlined Id of string
        |DADD=3uy       //Double addition
        |IADD=4uy       //Int addition
        |DSUB=5uy       //Double subtraction
        |ISUB=6uy       //Int subtraction
        |DMUL=7uy       //Double multiplication
        |IMUL=8uy       //Int multiplication
        |DDIV=9uy       //Double division
        |IDIV=10uy      //Int division
        |IMOD=11uy      //Modulo operation
        |DNEG=12uy      //Negate double
        |INEG=13uy      //Negate int
        |IPRINT=14uy    //Print integer on TOS
        |DPRINT=15uy    //Print double  on TOS
        |SPRINT=16uy    //Print string on TOS
        |I2D=17uy       //Convert int on TOS to double
        |D2I=18uy       //Convert double on Tos to int
        |S2I=19uy       //Converts string pointer to int 
        |SWAP=20uy      //Swap 2 topmost elements
        |POP=21uy       //Remove value from TOS
        |LOADVAR=22uy
        |LOADSVAR=23uy
        |LOADCTXVAR=24uy
        |STOREVAR=25uy
        |STORECTXVAR=26uy
        |DCMP=27uy
        |ICMP=28uy
        |JA=29uy
        |IFICMPNE=30uy
        |IFICMPE=31uy
        |IFICMPG=32uy
        |IFICMPGE=33uy
        |IFICMPL=34uy
        |IFICMPLE=35uy
        |DUMP=36uy
        |STOP=37uy
        |CALL=38uy
        |RETURN=39uy
        |BREAK=40uy
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
            |[] -> []
            |h::t -> t
        let pop2 = function
            |[] -> []
            |h::[] -> []
            |h1::h2::t -> t

        let empty = []
    
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
        stringPool : (int64 * int64 * string) list ;
        functions : Function list;
     } 
    
