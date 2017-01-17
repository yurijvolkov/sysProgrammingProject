module Types
    type Type = 
        Int32 = 0
        |Double = 1
        
    type Function =  {
        id : int
        nameId : int;
        argsCount : int;
        localsCount : int;
        returnType : Type;
        code : byte list
    }

    type Program = {
        functions : Function list
        stringPool : string list
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

    type VM = {
        dataStack : Stack.stack<int>;
        instrNumber : int
        program : Program
    }

