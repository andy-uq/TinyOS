Movi      r1          $99         ;move 99 into r1
Pushr     r1                      ;push r1 onto the stack
Movi      r1          $11         ;move 11 into r1
Pushr     r1                      ;push r1 onto the stack
Incr      r1                      ;incr r1
Popr      r1                      ;pop off the stack into r1
Movi      r3          $252        ;move 252 into r3
Printm    r3                      ;print memory at r3
Movi      r3          $150        ;put 150 into r3
Popm      r3                      ;pop off the stack into memory at r3
Printm    r3                      ;print memory at r3
Pushi     $88                     ;push 88 onto the stack
Pushi     $77                     ;push 77 onto the stack
Pushi     $66                     ;push 66 onto the stack
Popr      r2                      ;pop off the stack into r2
Printr    r2                      ;print r2
Popr      r2                      ;pop off the stack into r2
Printr    r2                      ;print r2
Popr      r2                      ;pop off the stack into r2
Printr    r2                      ;print r2
Exit      r2                      ;exit
