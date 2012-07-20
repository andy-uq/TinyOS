Movi      r1          $1          ;move 1 into r2
Printr    r1                      ;Print the value in a register
Incr      r1                      ;Increase the value of a register by 1
Printr    r1                      ;Print the value in a register
Incr      r1                      ;Increase the value of a register by 1
Printr    r1                      ;Print the value in a register
Incr      r1                      ;Increase the value of a register by 1
Printr    r1                      ;Print the value in a register
Movi      r2          $43         ;jump 43 bytes forward
Call      r2                      ;Call the function offset from the current instruction by a register; The address of the next instruction to execute after a RET is pushed on the stack.
Incr      r1                      ;Increase the value of a register by 1
Printr    r1                      ;Print the value in a register
Movi      r3          $150        ;move 150 into r3
Movi      r4          $11         ;move 11 into r4
Movrm     r3          r4          ;move r4 into memory at r3
Callm     r3                      ;Call the function offset from the current instruction by a memory address pointed to by a register; The address of the next instruction to execute after a RET is pushed on the stack.
Exit      r3                      ;ext
Movi      r1          $55         ;move 55 into r2
Ret                               ;ret
Printr    r1                      ;Print the value in a register
Ret                               ;Returns control to the next instruction after the last call
