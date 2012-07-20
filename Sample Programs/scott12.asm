Movi      r1          $1          ;move 1 into r1
MapShared r1          r2          ;map shared mem region #r1, return addr inr2
Printr    r2                      ;print r2
Movi      r5          $1          ;event 1
Wait      r5                      ;wait on 1
Printm    r2                      ;print out shared mem
Addi      r2          $4          ;add 4
Printm    r2                      ;Print a value in memory
Addi      r2          $4          ;add 4
Printm    r2                      ;Print a value in memory
Addi      r2          $4          ;add 4
Printm    r2                      ;Print a value in memory
Addi      r2          $4          ;add 4
Exit      r2                      ;Terminates the current process
