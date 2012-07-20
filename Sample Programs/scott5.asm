Movi      r2          $255        ;move 255 into r2
Movi      r1          $244        ;move 244 into r1
Movrm     r1          r2          ;move contents of r2 into memory pointed to by r1 (now address 244)
Printm    r1                      ;print memory pointed to by r1
Movmm     r2          r1          ;move memory pointed to by r1 into memory pointed to by r2
Printm    r2                      ;print memory pointed to by r2
Movmr     r3          r2          ;move memory pointed to by r2 into contents of r3
Printr    r3                      ;print r3
Movi      r4          $4          ;move 4 into r4
Clear     r2          r4          ;clear memory at r2 for length r4
Movi      r6          $12         ;move 12 into r6
Cmpi      r6          $14         ;compare r6 and 14
Cmpi      r6          $11         ;compare r6 and 11
Cmpi      r6          $12         ;compare r6 and 12
Movi      r7          $1          ;jump over this exit
Jmp       r7                      ;Jump to an instruction relative to the current instruction. Value may be negative.
Exit      r7                      ;this is exit.
Printr    r1                      ;Print the value in a register
Movi      r6          $12         ;move 12 into r6
Cmpi      r6          $14         ;compare r6 and 14
Jlt       r7                      ;jlt over the exit
Exit      r7                      ;Terminates the current process
Printr    r1                      ;Print the value in a register
Movi      r6          $19         ;move 12 into r6
Cmpi      r6          $14         ;compare r6 and 18
Jgt       r7                      ;jgt over the exit
Exit      r7                      ;Terminates the current process
Printr    r1                      ;Print the value in a register
Movi      r6          $14         ;move 12 into r6
Cmpi      r6          $14         ;compare r6 and 18
Je        r7                      ;je over the exit
Exit      r7                      ;Terminates the current process
Printr    r1                      ;Print the value in a register
Exit      r1                      ;Terminates the current process
