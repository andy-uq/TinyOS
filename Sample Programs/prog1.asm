Mov       r7          $128        ;print to r7
Output    r7          r8          ;Print r8
Mov       r1          $10         ;Move 10 into r1
Mov       r2          $11         ;Move 11 into r2
Mov       r3          $0          ;Move 0 into r3
Acquire   r1                      ;Acquire lock in r1 (currently lock 10)
Mov       r7          $128        ;print to r7
Output    r7          r2          ;Print r2
Sleep     r3                      ;Sleep r3 (current 0, will sleep forever)
Mov       r7          $128        ;print to r7
Output    r7          r3          ;Print r3
Release   r1                      ;Release r1
Exit      r1                      ;Exit
