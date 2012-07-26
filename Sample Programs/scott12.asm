Mov       r1          $1          ;move 1 into r1
Map       r1          r2          ;map shared mem region #r1 return addr inr2
Print     r2                      ;print r2
Mov       r5          $1          ;event 1
Wait      r5                      ;wait on 1
Print     r2                      ;print out shared mem
Add       r2          $4          ;add 4
Print     r2                      ;Print the value in a register
Add       r2          $4          ;add 4
Print     r2                      ;Print the value in a register
Add       r2          $4          ;add 4
Print     r2                      ;Print the value in a register
Add       r2          $4          ;add 4
Exit      r2                      ;Terminates the current process
