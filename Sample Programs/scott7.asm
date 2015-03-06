Mov       r1          $1          ;move 1 into r2
Mov       r5          $11         ;move 11 into r5
Sleep     r5                      ;sleep 11 - because we sleep, this program must be launch with another app like idle to take up those sleep cycles
Mov       r7          $128        ;Assign a register a value
Output    r7          r1          ;Output a value to the device pointed to by the register
Incr      r1                      ;Increase the value of a register by 1
Mov       r7          $128        ;Assign a register a value
Output    r7          r1          ;Output a value to the device pointed to by the register
Incr      r1                      ;Increase the value of a register by 1
Mov       r7          $128        ;Assign a register a value
Output    r7          r1          ;Output a value to the device pointed to by the register
Incr      r1                      ;Increase the value of a register by 1
Mov       r7          $128        ;Assign a register a value
Output    r7          r1          ;Output a value to the device pointed to by the register
Jmp       $7                      ;jump 7 instructions forward
Call      r2                      ;Call the function absolute from the current instruction by a register; The address of the next instruction to execute after a RET is pushed on the stack.
Incr      r1                      ;Increase the value of a register by 1
Mov       r7          $128        ;Assign a register a value
Output    r7          r1          ;Output a value to the device pointed to by the register
Mov       [r3]        $150        ;move 150 into r3
Mov       r4          $11         ;move 11 into r4
Mov       [r3]        r4          ;move r4 into memory at r3
Call      r3                      ;Call the function absolute from the current instruction by a register; The address of the next instruction to execute after a RET is pushed on the stack.
Exit      r1                      ;ext
Mov       r1          $55         ;move 55 into r2
Ret                               ;ret
Mov       r7          $128        ;Assign a register a value
Output    r7          r1          ;Output a value to the device pointed to by the register
Ret                               ;Returns control to the next instruction after the last call
