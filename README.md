# Stack virtual machine : VT-machine

## Technologies :
* **Main language** : F# 
* **Tests** : ASM (NASM)

## Program structure :
* **Magic numbers** : 0xba 0xba (2 bytes)
* **Required machine version** : 1 (8 bytes)
* **Functions offset** : (8 bytes)
* **Constant pool** :
  -  **Constant pool size** : (8 bytes)
  -  **Constants divided by \0**
* **Functions count** : (8 bytes)
* **Functions** :   
  -  **Function name ID (from constant pool)** : (8 bytes)
  -  **Local variables count** : (8 bytes)
  -  **Flags** : (8 bytes)
  -  **Count of args** : (8 bytes)
  -  **Type of args** : (1 byte for each arg)
  -  **Byte code length** : (8 bytes)
  -  **Byte code** : (1 byte for each command or required amount for parameter)
 
## Building :
To build virtual machine required : F# compiler (fsc)
```F#
 fsc --out:vm.exe --doc:doc.xml Commands.fs Types.fs HelpFuncs.fs Loader.fs Exec.fs Vm.fs
```
#### Order of files is important!
====
## Tests
Tests are located in directory ***vm_fsharp/bin/Debug/tests***

To build tests required : Nasm utility

Supported Makefile translates ****.vt*** into ****.vtc***

Currently implemented :
* Fibonaci recursive calculation : ***vm_fsharp/bin/Debug/tests/fib.vt****

===

## Documentation
XML documentation located in ***vm_fsharp/doc.xml***

## Type system :
Current version contains commands to work with 3 types :
- [X] Int32
- [X] Double
- [X] String
- [ ] [U]Int(16|64)
- [ ] Single
- [ ] Char

In main type "VmValue" implemented convertion to all of listed types. So to use them you have only to provide commands.

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
