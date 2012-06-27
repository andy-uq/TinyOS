Printr    r8                      ;Print r8
Movi      r1          $10         ;Move 10 into r1
Acquire   r1                      ;Acquire Lock in r1 (currently 10)
Addi      r1          $1          ;Addi 1 to r1
Sleep     r1                      ;Sleep r1 (currently 25)
Acquire   r1                      ;Acquire r1 (currently 11, INVALID -> NOOP)
Addi      r1          $1          ;Addi 1 to r1
Sleep     r1                      ;Sleep r1 (currently 12)
Acquire   r1                      ;Acquire r1 (currently 12, INVALUD -> NOOP)
Printr    r1                      ;Print r1 (currently 12)
Movi      r2          $100        ;Move 100 into r2
Sleep     r2                      ;Sleep r2 (currently 100)
Printr    r2                      ;Print r2
Exit      r3                      ;Exit
