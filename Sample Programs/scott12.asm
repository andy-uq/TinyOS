Mov       r1          $1          ;move 1 into r1
Map       r1          r2          ;map shared mem region #r1 return addr inr2
Mov       r7          $128        ;Assign a register a value
Output    r7          r2          ;print r2
Mov       r5          $1          ;event 1
Wait      r5                      ;wait on 1
Mov       r7          $128        ;Assign a register a value
Output    r7          r2          ;print out shared mem
Add       r2          $4          ;add 4
Mov       r7          $128        ;Assign a register a value
Output    r7          r2          ;Output a value to the device pointed to by the register
Add       r2          $4          ;add 4
Mov       r7          $128        ;Assign a register a value
Output    r7          r2          ;Output a value to the device pointed to by the register
Add       r2          $4          ;add 4
Mov       r7          $128        ;Assign a register a value
Output    r7          r2          ;Output a value to the device pointed to by the register
Add       r2          $4          ;add 4
Exit      r2                      ;Terminates the current process
