Print     r8                      ;Print r8
Mov       r1          $10         ;Move 10 into r1
Acquire   r1                      ;Acquire Lock in r1 (currently 10)
Add       r1          $1          ;Addi 1 to r1
Sleep     r1                      ;Sleep r1 (currently 25)
Acquire   r1                      ;Acquire r1 (currently 11, INVALID -> NOOP)
Add       r1          $1          ;Addi 1 to r1
Sleep     r1                      ;Sleep r1 (currently 12)
Acquire   r1                      ;Acquire r1 (currently 12, INVALUD -> NOOP)
Print     r1                      ;Print r1 (currently 12)
Mov       r2          $100        ;Move 100 into r2
Sleep     r2                      ;Sleep r2 (currently 100)
Print     r2                      ;Print r2
Exit      r3                      ;Exit
