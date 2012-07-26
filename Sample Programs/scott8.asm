Mov       r1          $1          ;move 1 into r1
Acquire   r1                      ;acquire lock #1
Mov       r2          $22         ;move 22 into r2
Mov       r3          $33         ;move 33 into r3
Mov       r4          $44         ;move 44 into r4
Mov       r5          $55         ;move 55 into r5
Mov       r6          $66         ;move 66 into r6
Mov       r2          $22         ;move 22 into r2
Mov       r3          $33         ;move 33 into r3
Mov       r4          $44         ;move 44 into r4
Mov       r5          $55         ;move 55 into r5
Mov       r6          $66         ;move 66 into r6
Mov       r2          $22         ;move 22 into r2
Mov       r3          $33         ;move 33 into r3
Mov       r4          $44         ;move 44 into r4
Mov       r5          $55         ;move 55 into r5
Mov       r6          $66         ;move 66 into r6
Release   r1                      ;Release the lock whose number is provided in the register; If the lock is not held by the current process, the instruction is a no-op
Exit      r1                      ;Terminates the current process
