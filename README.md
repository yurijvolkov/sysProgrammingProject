# Stack virtual machine : VT-machine

## Technologies :
* Main language : F# 
* Tests written in ASM (NASM)


## Type system :
Current version contains commands to work with 3 types :
- [X] Int32
- [X] Double
- [X] String
- [ ] [U]Int(16|64)
- [ ] Single
- [ ] Char

In main type "VmValue" implemented convertartion to all of listed types. So to use them you have only to provide commands.

## Supported commands :
### For full list and description look ***vm_fsharp/Commands.fs***
* Arithmetic : [ID]ADD, [ID]SUB, [ID]MUL, [ID]DIV, IMOD, [ID]NEG, and others 
* Output : [ISD]Print
* Convert : I2D, D2I, S2I
* Stack : LOAD, LOADS, POP, SWAP, DUMP
* Variables : LOAD[S]VAR, STOREVAR
* Comparing : [DI]CMP
* Branching : JA, JZ[I], JNZ[I], JMI, JNMI, and others
* Functions : CALL, RETURN, STOP
* Not implemented : STORECTXVAR, LOADCTXVAR, BREAK 
