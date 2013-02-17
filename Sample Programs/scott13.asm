Mov       r4          $4          ;we'll need 4 bytes
Alloc     r4          r5          ;ask for 4 bytes
Mov       r2          r5          ;save address in r2
Mov       r4          $33         ;we'll need 12 bytes
Alloc     r4          r5          ;ask for 12 bytes
Mov       r1          r5          ;save address in r1
Free      r2                      ;release
Mov       r6          $11         ;put 11 in r6
Mov       r5          r6          ;put 11 in the new memory
Add       r5          $4          ;Add a value to a register
Mov       r5          r6          ;put 11 in the new memory
Add       r5          $4          ;Add a value to a register
Mov       r5          r6          ;put 11 in the new memory
Mov       r4          $17         ;we'll need 17 bytes
Alloc     r4          r5          ;ask for 17 bytes
Mov       r3          r5          ;save address in r3
Free      r1                      ;Free memory previously allocated, pointed to by a register 
Free      r3                      ;Free memory previously allocated, pointed to by a register 
Exit      $0                      ;this is exit.
