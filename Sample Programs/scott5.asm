Mov       r2          $255        ;move 255 into r2
Mov       r1          $244        ;move 244 into r1
Mov       [r1]        r2          ;move contents of r2 into memory pointed to by r1 (now address 244)
Mov       r7          $128        ;Assign a register a value
Output    r7          [r1]        ;print memory pointed to by r1
Mov       [r2]        [r1]        ;move memory pointed to by r1 into memory pointed to by r2
Mov       r7          $128        ;Assign a register a value
Output    r7          r2          ;print memory pointed to by r2
Mov       r3          [r2]        ;move memory pointed to by r2 into contents of r3
Mov       r7          $128        ;Assign a register a value
Output    r7          r3          ;print r3
Mov       r4          $4          ;move 4 into r4
Clear     r2          r4          ;clear memory at r2 for length r4
Mov       r6          $12         ;move 12 into r6
Cmp       r6          $14         ;compare r6 and 14
Cmp       r6          $11         ;compare r6 and 11
Cmp       r6          $12         ;compare r6 and 12
Mov       r7          $1          ;jump over this exit
Jmp       r7                      ;Jump to an instruction relative to the current instruction. Value may be negative.
Exit      r6                      ;this is exit.
Mov       r7          $128        ;Assign a register a value
Output    r7          r1          ;Output a value to the device pointed to by the register
Mov       r6          $12         ;move 12 into r6
Cmp       r6          $14         ;compare r6 and 14
Jlt       r7                      ;jlt over the exit
Exit      r6                      ;Terminates the current process
Mov       r7          $128        ;Assign a register a value
Output    r7          r1          ;Output a value to the device pointed to by the register
Mov       r6          $19         ;move 12 into r6
Cmp       r6          $14         ;compare r6 and 18
Jgt       r6                      ;jgt over the exit
Exit      r7                      ;Terminates the current process
Mov       r7          $128        ;Assign a register a value
Output    r7          r1          ;Output a value to the device pointed to by the register
Mov       r6          $14         ;move 12 into r6
Cmp       r6          $14         ;compare r6 and 18
Je        r7                      ;je over the exit
Exit      r6                      ;Terminates the current process
Mov       r7          $128        ;Assign a register a value
Output    r7          r1          ;Output a value to the device pointed to by the register
Exit      r6                      ;Terminates the current process
