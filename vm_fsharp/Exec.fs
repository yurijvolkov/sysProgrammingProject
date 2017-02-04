module Exec

open Types
open Loader
open HelpFuncs
open Commands
open System

/// <summary>
/// Function executes next command of head-context from VM list of contexts
/// </summary>
/// <param name="vm">Virtual machine</param>
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
            printf "%i" ((Stack.head vm.dataStack).Value.ToInt32())
            let ctx = {command=tail h.command; func=h.func; locals=h.locals}
            execute {context=ctx::t; dataStack=Stack.pop vm.dataStack; stringPool=vm.stringPool; functions=vm.functions}
        | Commands.DPRINT ->
            printf "%f" ((Stack.head vm.dataStack).Value.ToDouble())
            let ctx = {command=tail h.command; func=h.func; locals=h.locals}
            execute {context=ctx::t; dataStack=Stack.pop vm.dataStack; stringPool=vm.stringPool; functions=vm.functions}
        | Commands.SPRINT -> 
            let ptr = (Stack.head vm.dataStack).Value.ToStrPtr()
            printf "%s" (getStr vm.stringPool (int64 (fst ptr)) (int64 (snd ptr))) 
            let ctx = {command=tail h.command; func=h.func; locals=h.locals}
            execute {context=ctx::t; dataStack=Stack.pop vm.dataStack; stringPool=vm.stringPool; functions=vm.functions}
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
        | Commands.LOADVAR | Commands.LOADSVAR ->
            let split = List.splitAt 8 (tail h.command)
            let varId = BitConverter.ToInt32(List.toArray(fst split), 0)
            let dataStack = Stack.push vm.dataStack (h.locals.[varId])
            let ctx = {command=snd split; func=h.func;locals=h.locals}
            execute {context=ctx::t; dataStack=dataStack; stringPool=vm.stringPool; functions=vm.functions}
        | Commands.STOREVAR ->
            let split = List.splitAt 8 (tail h.command)
            let varId = BitConverter.ToInt32(List.toArray(fst split), 0)
            let dataStack = Stack.pop vm.dataStack
            h.locals.[varId] <- (Stack.head vm.dataStack).Value
            let ctx = {command=snd split; func=h.func;locals=h.locals}
            execute {context=ctx::t; dataStack=dataStack; stringPool=vm.stringPool; functions=vm.functions}        
        | Commands.DCMP | Commands.ICMP ->
            let res = match command with
                | Commands.DCMP ->
                    let arg1 = (Stack.head vm.dataStack).Value.ToDouble()
                    let arg2 = (Stack.head (Stack.pop vm.dataStack)).Value.ToDouble()
                    match arg1 - arg2 with
                        |v when v>0. -> 1
                        |v when v<0. -> -1
                        |0. -> 0
                | Commands.ICMP ->
                    let arg1 = (Stack.head vm.dataStack).Value.ToInt32()
                    let arg2 = (Stack.head (Stack.pop vm.dataStack)).Value.ToInt32()
                    match arg1 - arg2 with
                        |v when v>0 -> 1
                        |v when v<0 -> -1
                        |0 -> 0
            let dataStack = Stack.push (Stack.pop2 vm.dataStack) (VmValue.Cons (BitConverter.GetBytes(res)))
            let ctx = {command=tail h.command; func=h.func; locals=h.locals}
            execute {context=ctx::t; dataStack=dataStack; stringPool=vm.stringPool; functions=vm.functions}
        | Commands.JA   | 
          Commands.JZI  | Commands.JNZI | Commands.JSI  | Commands.JNSI |
          Commands.JMI  | Commands.JMEI | Commands.JLI  | Commands.JLEI |
          Commands.JZD  | Commands.JNZD | Commands.JSD  | Commands.JNSD |
          Commands.JMD  | Commands.JMED | Commands.JLD  | Commands.JLED  ->
            let split = List.splitAt 8 (tail h.command)
            let offset = BitConverter.ToInt32(List.toArray (fst split), 0)
            let newPosition = (List.length h.func.code) -  (List.length (snd split)) + offset 
            let isJump = match command with
                | Commands.JA -> true
                | Commands.JZI -> match (head vm.dataStack).ToInt32() with
                    | 0 -> true
                    | _ -> false
                |  Commands.JNZI -> match (head vm.dataStack).ToInt32() with
                    | 0 -> false
                    | _ -> true
                | Commands.JSI -> match (head vm.dataStack).ToInt32() with
                    | v when v < 0 -> true
                    | _ -> false
                | Commands.JNSI -> match (head vm.dataStack).ToInt32() with
                    | v when v >= 0 -> true
                    | _ -> false
                | Commands.JMI ->  match ((head vm.dataStack).ToInt32() - (head (Stack.pop vm.dataStack)).ToInt32()) with
                    | v when v > 0 -> true
                    | _ -> false
                | Commands.JMEI ->  match ((head vm.dataStack).ToInt32() - (head (Stack.pop vm.dataStack)).ToInt32()) with
                    | v when v >= 0 -> true
                    | _ -> false
                | Commands.JLI ->  match ((head vm.dataStack).ToInt32() - (head (Stack.pop vm.dataStack)).ToInt32()) with
                        | v when v < 0 -> true
                        | _ -> false
                | Commands.JLEI ->  match ((head vm.dataStack).ToInt32() - (head (Stack.pop vm.dataStack)).ToInt32()) with
                    | v when v <= 0 -> true
                    | _ -> false
                | Commands.JZD -> match (head vm.dataStack).ToDouble() with
                    | 0. -> true
                    | _ -> false
                |  Commands.JNZD -> match (head vm.dataStack).ToDouble() with
                    | 0. -> false
                    | _ -> true
                | Commands.JSD -> match (head vm.dataStack).ToDouble() with
                    | v when v < 0. -> true
                    | _ -> false
                | Commands.JNSD -> match (head vm.dataStack).ToDouble() with
                    | v when v >= 0. -> true
                    | _ -> false
                | Commands.JMD ->  match ((head vm.dataStack).ToDouble() - (head (Stack.pop vm.dataStack)).ToDouble()) with
                    | v when v > 0. -> true
                    | _ -> false
                | Commands.JMED ->  match ((head vm.dataStack).ToDouble() - (head (Stack.pop vm.dataStack)).ToDouble()) with
                    | v when v >= 0. -> true
                    | _ -> false
                | Commands.JLD ->  match ((head vm.dataStack).ToDouble() - (head (Stack.pop vm.dataStack)).ToDouble()) with
                    | v when v < 0. -> true
                    | _ -> false
                | Commands.JLED ->  match ((head vm.dataStack).ToDouble() - (head (Stack.pop vm.dataStack)).ToDouble()) with
                    | v when v <= 0. -> true
                    | _ -> false
                
            let newCommand = match isJump with
                | true ->  snd (List.splitAt newPosition h.func.code )
                | false -> snd split
            let ctx = {command=newCommand; func=h.func; locals=h.locals}
            execute {context=ctx::t; dataStack=vm.dataStack; stringPool=vm.stringPool; functions=vm.functions}
        | Commands.DUMP -> 
            let value = head vm.dataStack
            let dataStack = Stack.push vm.dataStack value
            let ctx = {command=tail h.command; func=h.func;locals=h.locals}
            execute {context=ctx::t; dataStack=dataStack; stringPool=vm.stringPool; functions=vm.functions}
        | Commands.STOP -> printfn "Execution was stoped : command STOP"
        | Commands.CALL ->
            let split = List.splitAt 8 (tail h.command)
            let fileId = int64 ((VmValue.Cons( List.toArray( fst (List.splitAt 4 (fst split))))).ToInt32())
            let nameId = int64 (VmValue.Cons( List.toArray( snd (List.splitAt 4 (fst split)))).ToInt32())
            let func = List.find (fun f -> f.fileId=fileId && f.nameId=nameId) vm.functions
            let oldCtx = {command = snd split; func=h.func;locals=h.locals}
            let newCtx = vmCtxInit func
            execute {context=newCtx::oldCtx::t; dataStack=vm.dataStack; stringPool=vm.stringPool; functions=vm.functions}
        | Commands.RETURN ->
            execute {context=t; dataStack=vm.dataStack; stringPool=vm.stringPool; functions=vm.functions}
        | _ -> printfn "Undefined command %s" (command.ToString())
        

