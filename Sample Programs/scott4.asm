Mov       r1          $99         ;Assign a register a value
Push      r1                      ;Push the value of a register onto the stack
Mov       r1          $11         ;Assign a register a value
Push      r1                      ;Push the value of a register onto the stack
Incr      r1                      ;Increase the value of a register by 1
Pop       r1                      ;Pop a value off the stack into a register
Mov       r3          $252        ;Assign a register a value
Print     r3                      ;Print the value in a register
Mov       r3          $150        ;Assign a register a value
Pop       [r3]                    ;Pop a value off the stack into a register
Print     [r3]                    ;Print the value in a register
Push      $88                     ;Push the value of a register onto the stack
Push      $77                     ;Push the value of a register onto the stack
Push      $66                     ;Push the value of a register onto the stack
Pop       r2                      ;Pop a value off the stack into a register
Print     r2                      ;Print the value in a register
Pop       r2                      ;Pop a value off the stack into a register
Print     r2                      ;Print the value in a register
Pop       r2                      ;Pop a value off the stack into a register
Print     r2                      ;Print the value in a register
Exit      r2                      ;this is exit.
