Mov       r1          $3276834    ;move a big number into r1
Mov       r2          r1          ;move r1 to r2
Mov       r7          $128        ;print to r7
Output    r7          r2          ;mov r7 $128 ; print to r7
Output    r7          r2          ;Output a value to the device pointed to by the register
Add       r2          $5          ;increment r2 by 5
Add       r2          $5          ;increment r2 by 5
Add       r2          $5          ;increment r2 by 5
Add       r2          $5          ;increment r2 by 5
Add       r2          $5          ;increment r2 by 5
Add       r2          $5          ;increment r2 by 5
Add       r2          $5          ;increment r2 by 5
Mov       r7          $128        ;print to r7
Output    r7          r2          ;mov r7 $128 ; print to r7
Output    r7          r2          ;Output a value to the device pointed to by the register
Exit      r2                      ;exit.
