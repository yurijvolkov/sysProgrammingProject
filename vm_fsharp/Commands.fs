module Commands

    /// <summary>
    /// Commands supported by virtual machine
    /// Some of them not implemented
    /// </summary>
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
        |IPRINT=14uy    //Pop and print integer on TOS
        |DPRINT=15uy    //Pop and print double  on TOS
        |SPRINT=16uy    //Pop and print string on TOS
        |I2D=17uy       //Convert int on TOS to double
        |D2I=18uy       //Convert double on Tos to int
        |S2I=19uy       //Converts string pointer to int 
        |SWAP=20uy      //Swap 2 topmost elements
        |POP=21uy       //Remove value from TOS
        |LOADVAR=22uy   //Load variable whose 4-byte Id is inlined 
        |LOADSVAR=23uy  //Same as LOADVAR
        |LOADCTXVAR=24uy//Load variable from context which inlined (NOT IMPLEMENTED)
        |STOREVAR=25uy  //Store value form TOS in variable whose 4-byte id is inlined
        |STORECTXVAR=26uy//Store value form TOS in variable to context whose 4-byte id is inlined (NOT IMPLEMENTED)
        |DCMP=27uy      //Compares two doubles on TOS. if(arg1-arg2 !=0) then sign(arg1-arg2) else 0
        |ICMP=28uy      //Compares two int's on TOS. if(arg1-arg2 !=0) then sign(arg1-arg2) else 0
        |JA=29uy        //Jump for give 4-byte offset
        |JZI=30uy       //Jump if Zero Integer
        |JNZI=31uy      //Jump if Non-Zero Integer
        |JSI=32uy       //Jump if Sign Integer
        |JNSI=33uy      //Jump if Non-Sign Integer
        |JMI=34uy       //Jump if TOS More than second Integer
        |JMEI=35uy      //Jump if TOS More or Equal than second Integer
        |JLI=36uy       //Jump if TOS Less than second Integer
        |JLEI=37uy      //Jump if TOS Less or Equal than second Integer
        |JZD=38uy       //Jump if Zero Double
        |JNZD=39uy      //Jump if Non-Zero Double
        |JSD=40uy       //Jump if Sign Double
        |JNSD=41uy      //Jump if Non-Sign Double
        |JMD=42uy       //Jump if TOS More than second Double
        |JMED=43uy      //Jump if TOS More or Equal than second Double
        |JLD=44uy       //Jump if TOS Less than second Double
        |JLED=45uy      //Jump if TOS Less or Equal than second Double
        |DUMP=46uy      //Dublicate TOS
        |STOP=47uy      //Stops virtual machine
        |CALL=48uy      //Call function with inlined 4bytes fileId, 4bytes nameId
        |RETURN=49uy    //Return to call location
        |BREAK=50uy     //Breakpoint (NOT IMPLEMENTED)

