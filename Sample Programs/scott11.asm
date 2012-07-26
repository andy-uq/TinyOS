Mov       r1          $1          ;move 1 into r1
Map       r1          r2          ;map shared mem region #r1 return addr inr2
Print     r2                      ;print r2 (the memory address)
Mov       r3          r2          ;copy the memory address into r3 (we might need it)
Mov       r4          $1          ;lock 1
Acquire   r4                      ;lock r4 (lock #1)
Mov       r3          $99         ;put 99 in r3
Mov       [r2]        r3          ;put r3 at memory r2
Add       r2          $4          ;add 4
Mov       [r2]        r3          ;put r3 at memory r2
Add       r2          $4          ;add 4
Mov       [r2]        r3          ;put r3 at memory r2
Add       r2          $4          ;add 4
Mov       [r2]        r3          ;put r3 at memory r2
Add       r2          $4          ;add 4
Release   r4                      ;release 1
Mov       r5          $1          ;event 1
Signal    r5                      ;signal 1
Exit      r2                      ;Terminates the current process
