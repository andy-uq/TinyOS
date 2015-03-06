Mov       r1          $1          ;move 1 into r1
Map       r1          r2          ;map shared mem region #r1 return addr inr2
Mov       r7          $128        ;print to r7
Output    r7          r2          ;mov r7 $128 ; print to r7
Output    r7          r2          ;Output a value to the device pointed to by the register
