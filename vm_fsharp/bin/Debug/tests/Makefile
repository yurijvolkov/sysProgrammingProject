# ---------------------------------------------------------------------------
#
#                 Dwarf, an educational stack virtual machine
#
#
#  See README.md for additional information.
# 
#                                (c) Igor Zhirkov, igorjirkov [at] gmail.com
# ---------------------------------------------------------------------------



ASM         = nasm
ASMFLAGS    = -fbin 

TESTS       = $(wildcard *.vt)
TESTTARGETS = $(TESTS:%.vt=%.vtc)

all: $(TESTTARGETS)

$(TESTTARGETS) :
	$(ASM) $(ASMFLAGS) -o $@ $(@:%.vtc=%.vt)
 
