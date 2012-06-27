Printr    r8                      ;Print r8
Movi      r1          $10         ;Move 10 into r1
Movi      r2          $11         ;Move 11 into r2
Movi      r3          $0          ;Move 0 into r3
Acquire   r1                      ;Acquire lock in r1 (currently lock 10)
Printr    r2                      ;Print r2
Sleep     r3                      ;Sleep r3 (current 0, will sleep forever)
Printr    r3                      ;Print r3
Release   r1                      ;Release r1
Exit      r1                      ;Exit
