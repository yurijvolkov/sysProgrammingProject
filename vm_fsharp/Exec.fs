module Exec

open Types
open Loader
open System

let head = function
    |h::t -> h

let tail = function
    |h::t -> t

let rec execute (vm : Vm) = 
    match vm with
    |{context=[]} -> ignore 1
    |{context=h::t} -> 
        let command : Commands = LanguagePrimitives.EnumOfValue (head h.command)
        match command with 
        | Commands.INVALID  -> printfn "Runtime error : invalid byte command."
        | Commands.LOAD -> 
            let split = List.splitAt 8 (tail h.command)
            let dataStack = Stack.push vm.dataStack (VmValue.Cons (List.toArray(fst split)))
            let ctx = {command=snd split; func=h.func; locals=h.locals}
            execute {context=ctx::t; dataStack=dataStack; stringPool=vm.stringPool; functions=vm.functions}
        | Commands.LOADS -> 
            let split = List.splitAt 8 (tail h.command)
            let strId = List.toArray(fst ( List.splitAt 4 (fst split) ))
            let fileId = BitConverter.GetBytes(int h.func.fileId)
            let dataStack = Stack.push vm.dataStack (VmValue.Cons (Array.concat [fileId;strId]))
            let ctx = {command=snd split; func=h.func; locals=h.locals}
            execute {context=ctx::t; dataStack=dataStack; stringPool=vm.stringPool; functions=vm.functions}
        | ( Commands.DADD | Commands.DSUB | Commands.DMUL | Commands.DDIV ) -> 
            let arg1 = (Stack.head vm.dataStack).Value.ToDouble()
            let arg2 = (Stack.head ( Stack.pop vm.dataStack )).Value.ToDouble()
            let res = match command with
                | Commands.DADD -> BitConverter.GetBytes(arg1+arg2)
                | Commands.DSUB -> BitConverter.GetBytes(arg1-arg2)
                | Commands.DMUL -> BitConverter.GetBytes(arg1*arg2)
                | Commands.DDIV -> BitConverter.GetBytes(arg1/arg2)
            let dataStack = Stack.push (Stack.pop2 vm.dataStack) (VmValue.Cons res)
            let ctx = {command=tail h.command; func=h.func; locals=h.locals}
            execute {context=ctx::t; dataStack=dataStack; stringPool=vm.stringPool;functions=vm.functions}
        | ( Commands.IADD | Commands.ISUB | Commands.IMUL | Commands.IDIV | Commands.IMOD ) -> 
            let arg1 = (Stack.head vm.dataStack).Value.ToInt32()
            let arg2 = (Stack.head ( Stack.pop vm.dataStack )).Value.ToInt32()
            let res = match command with
                | Commands.IADD -> BitConverter.GetBytes(arg1+arg2)
                | Commands.ISUB -> BitConverter.GetBytes(arg1-arg2)
                | Commands.IMUL -> BitConverter.GetBytes(arg1*arg2)
                | Commands.IDIV -> BitConverter.GetBytes(arg1/arg2)
                | Commands.IMOD -> BitConverter.GetBytes(arg1%arg2)
            let dataStack = Stack.push (Stack.pop2 vm.dataStack) (VmValue.Cons res)
            let ctx = {command=tail h.command; func=h.func; locals=h.locals}
            execute {context=ctx::t; dataStack=dataStack; stringPool=vm.stringPool;functions=vm.functions}
        | Commands.DNEG | Commands.INEG | Commands.I2D | Commands.D2I | Commands.S2I->
            let arg = match command with
                |Commands.DNEG ->  BitConverter.GetBytes(-1. * (Stack.head vm.dataStack).Value.ToDouble())
                |Commands.INEG ->  BitConverter.GetBytes(-1 * (Stack.head vm.dataStack).Value.ToInt32())
                |Commands.I2D -> BitConverter.GetBytes( double ((Stack.head vm.dataStack).Value.ToInt32()))
                |Commands.D2I -> BitConverter.GetBytes( int ((Stack.head vm.dataStack).Value.ToDouble()))
                |Commands.S2I -> BitConverter.GetBytes(snd ((Stack.head vm.dataStack).Value.ToStrPtr()))
            let dataStack = Stack.push (Stack.pop vm.dataStack) (VmValue.Cons arg)
            let ctx = {command=tail h.command; func=h.func; locals=h.locals}
            execute {context=ctx::t; dataStack=dataStack; stringPool=vm.stringPool;functions=vm.functions}
        | Commands.IPRINT -> 
            printfn "%i" ((Stack.head vm.dataStack).Value.ToInt32())
            let ctx = {command=tail h.command; func=h.func; locals=h.locals}
            execute {context=ctx::t; dataStack=vm.dataStack; stringPool=vm.stringPool; functions=vm.functions}
        | Commands.DPRINT ->
            printfn "%f" ((Stack.head vm.dataStack).Value.ToDouble())
            let ctx = {command=tail h.command; func=h.func; locals=h.locals}
            execute {context=ctx::t; dataStack=vm.dataStack; stringPool=vm.stringPool; functions=vm.functions}
        | Commands.SPRINT -> 
            let ptr = (Stack.head vm.dataStack).Value.ToStrPtr()
            printfn "%s" (getStr vm.stringPool (int64 (fst ptr)) (int64 (snd ptr))) 
            let ctx = {command=tail h.command; func=h.func; locals=h.locals}
            execute {context=ctx::t; dataStack=vm.dataStack; stringPool=vm.stringPool; functions=vm.functions}
        | Commands.SWAP ->
            let arg1 = VmValue.Cons (BitConverter.GetBytes((Stack.head vm.dataStack).Value.ToDouble()))
            let arg2 = VmValue.Cons (BitConverter.GetBytes((Stack.head(Stack.pop vm.dataStack)).Value.ToDouble()))
            let dataStack = Stack.push (Stack.push (Stack.pop2 vm.dataStack) arg1) arg2
            let ctx = {command=tail h.command; func=h.func; locals=h.locals}
            execute {context=ctx::t; dataStack=dataStack; stringPool=vm.stringPool; functions=vm.functions}
        | Commands.POP ->
            let dataStack = Stack.pop vm.dataStack
            let ctx = {command=tail h.command; func=h.func; locals=h.locals}
            execute {context=ctx::t; dataStack=dataStack; stringPool=vm.stringPool; functions=vm.functions}
        | Commands.LOADVAR ->
            let split = List.splitAt 8 (tail h.command)
            let varId = BitConverter.ToInt32(List.toArray(fst split), 0)
            let dataStack = Stack.push vm.dataStack (VmValue.Cons (BitConverter.GetBytes(h.locals.[varId].ToDouble())))
            let ctx = {command=snd split; func=h.func;locals=h.locals}
            execute {context=ctx::t; dataStack=dataStack; stringPool=vm.stringPool; functions=vm.functions}
        | Commands.STOREVAR ->
            let split = List.splitAt 8 (tail h.command)
            let varId = BitConverter.ToInt32(List.toArray(fst split), 0)
            let dataStack = Stack.pop vm.dataStack
            h.locals.[varId] <- (Stack.head vm.dataStack).Value
            let ctx = {command=snd split; func=h.func;locals=h.locals}
            execute {context=ctx::t; dataStack=dataStack; stringPool=vm.stringPool; functions=vm.functions}        
        | Commands.STOP -> printfn "Execution was stoped : command STOP"
        | _ -> ignore 1
        

