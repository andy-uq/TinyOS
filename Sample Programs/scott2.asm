Incr      r1                      ;incr r1
Add       r6          $16         ;add $16 to r6
SetP      r6                      ;set priority to 26
Add       r2          $5          ;increment r2 by 5
Add       r1          r2          ;add 1 and 2 and the result goes in 1
Add       r2          $5          ;increment r2 by 5
Mov       r3          $99         ;move 99 into r3
Mov       r4          r3          ;move r3 into r4
Print     r4                      ;print r4
Exit      r4                      ;exit
