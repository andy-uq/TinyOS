Movi      r4          $4          ;we'll need 4 bytes
Alloc     r4          r5          ;ask for 4 bytes
Movr      r2          r5          ;save address in r2
Movi      r4          $33         ;we'll need 12 bytes
Alloc     r4          r5          ;ask for 12 bytes
Movr      r1          r5          ;save address in r1
Free      r2                      ;release
Movi      r6          $11         ;put 11 in r6
Movrm     r5          r6          ;put 11 in the new memory
Addi      r5          $4          
Movrm     r5          r6          ;put 11 in the new memory
Addi      r5          $4          
Movrm     r5          r6          ;put 11 in the new memory
Movi      r4          $17         ;we'll need 17 bytes
Alloc     r4          r5          ;ask for 17 bytes
Movr      r3          r5          ;save address in r3
Free      r1                      
Free      r3                      
Exit      r0                      ;this is exit.
