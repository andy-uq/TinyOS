Mov       r1          $99         ;move 99 into r1
Push      r1                      ;push r1 onto the stack
Mov       r1          $11         ;move 11 into r1
Push      r1                      ;push r1 onto the stack
Incr      r1                      ;incr r1
Pop       r1                      ;pop off the stack into r1
Mov       r3          $252        ;move 252 into r3
Print     [r3]                    ;print memory at r3
Mov       r3          $150        ;put 150 into r3
Pop       [r3]                    ;pop off the stack into memory at r3
Print     [r3]                    ;print memory at r3
Push      $88                     ;push 88 onto the stack
Push      $77                     ;push 77 onto the stack
Push      $66                     ;push 66 onto the stack
Pop       r2                      ;pop off the stack into r2
Print     r2                      ;print r2
Pop       r2                      ;pop off the stack into r2
Print     r2                      ;print r2
Pop       r2                      ;pop off the stack into r2
Print     r2                      ;print r2
Exit      r2                      ;exit
