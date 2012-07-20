Movi      r1          $1          ;move 1 into r1
MapShared r1          r2          ;map shared mem region #r1, return addr inr2
Printr    r2                      ;print r2 (the memory address)
Movr      r3          r2          ;copy the memory address into r3 (we might need it)
Movi      r4          $1          ;lock 1
Acquire   r4                      ;lock r4 (lock #1)
Movi      r3          $99         ;put 99 in r3
Movrm     r2          r3          ;put r3 at memory r2
Addi      r2          $4          ;add 4
Movrm     r2          r3          ;put r3 at memory r2
Addi      r2          $4          ;add 4
Movrm     r2          r3          ;put r3 at memory r2
Addi      r2          $4          ;add 4
Movrm     r2          r3          ;put r3 at memory r2
Addi      r2          $4          ;add 4
Release   r4                      ;release 1
Movi      r5          $1          ;event 1
Signal    r5                      ;signal 1
Exit      r5                      ;Terminates the current process
