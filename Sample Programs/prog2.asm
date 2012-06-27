Printr    r8                      ;Print r8
Movi      r1          $10         ;Move 10 into r1
Movi      r2          $6          ;Move 6 into r2
Movi      r3          $25         ;Move 25 intno r3
Acquire   r1                      ;Acquire lock in r1 (currently 10)
Printr    r3                      ;Print r3 (currently 25)
Release   r1                      ;Release r4 (currently 10)
Sleep     r3                      ;Sleep r3 (currently 25)
Printr    r3                      ;Print r3 (currently 25)
Exit      r3                      ;Exit
