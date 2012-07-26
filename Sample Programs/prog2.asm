Print     r8                      ;Print r8
Mov       r1          $10         ;Move 10 into r1
Mov       r2          $6          ;Move 6 into r2
Mov       r3          $25         ;Move 25 into r3
Acquire   r1                      ;Acquire lock in r1 (currently 10)
Print     r3                      ;Print r3 (currently 25)
Release   r1                      ;Release r4 (currently 10)
Sleep     r3                      ;Sleep r3 (currently 25)
Print     r3                      ;Print r3 (currently 25)
Exit      r3                      ;Exit
