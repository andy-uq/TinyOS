Mov       r1          $1          ;move 1 into r2
Print     r1                      ;Print the value in a register
Incr      r1                      ;Increase the value of a register by 1
Print     r1                      ;Print the value in a register
Incr      r1                      ;Increase the value of a register by 1
Print     r1                      ;Print the value in a register
Incr      r1                      ;Increase the value of a register by 1
Print     r1                      ;Print the value in a register
Mov       r2          $43         ;jump 43 bytes forward
Jmp       r2                      ;Jump to an instruction relative to the current instruction. Value may be negative.
Incr      r1                      ;Increase the value of a register by 1
Print     r1                      ;Print the value in a register
Mov       r3          $150        ;move 150 into r3
Mov       r4          $11         ;move 11 into r4
Mov       [r3]        r4          ;move r4 into memory at r3
Call      [r3]                    ;Call the function offset from the current instruction by a register; The address of the next instruction to execute after a RET is pushed on the stack.
Exit      r3                      ;ext
Mov       r1          $55         ;move 55 into r2
Ret                               ;ret
Print     r1                      ;Print the value in a register
Ret                               ;Returns control to the next instruction after the last call
