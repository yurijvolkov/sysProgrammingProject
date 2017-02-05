
ASM         = nasm
ASMFLAGS    = -fbin 

TESTS       = $(wildcard *.vt)
TESTTARGETS = $(TESTS:%.vt=%.vtc)

all: $(TESTTARGETS)

$(TESTTARGETS) :
	$(ASM) $(ASMFLAGS) -o $@ $(@:%.vtc=%.vt)
 
