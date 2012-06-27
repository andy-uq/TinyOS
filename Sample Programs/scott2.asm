Incr      r1                      ;incr r1
Addi      r6          $16         ;add $16 to r6
SetP      r6                      ;set priority to 26
Addi      r2          $5          ;increment r2 by 5
Addr      r1          $1          ;add 1 and 2 and the result goes in 1
Addi      r2          $5          ;increment r2 by 5
Movi      r3          $99         ;move 99 into r3
Movr      r4          r3          ;move r3 into r4
Printr    r4                      ;print r4
Exit      r4                      ;exit
