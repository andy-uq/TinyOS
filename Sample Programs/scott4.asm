Mov       r1          $99         ;Assign a register a value
Push      r1                      ;Push the value of a register onto the stack
Mov       r1          $11         ;Assign a register a value
Push      r1                      ;Push the value of a register onto the stack
Incr      r1                      ;Increase the value of a register by 1
Pop       r1                      ;Pop a value off the stack into a register
Mov       r3          $252        ;Assign a register a value
Mov       r7          $128        ;print to r7
Output    r7          r3          ;Output a value to the device pointed to by the register
Mov       r3          $150        ;Assign a register a value
Pop       [r3]                    ;Pop a value off the stack into a register
Mov       r7          $128        ;print to r7
Output    r7          [r3]        ;Output a value to the device pointed to by the register
Push      $88                     ;Push the value of a register onto the stack
Push      $77                     ;Push the value of a register onto the stack
Push      $66                     ;Push the value of a register onto the stack
Pop       r2                      ;Pop a value off the stack into a register
Mov       r7          $128        ;print to r7
Output    r7          r2          ;Output a value to the device pointed to by the register
Pop       r2                      ;Pop a value off the stack into a register
Mov       r7          $128        ;print to r7
Output    r7          r2          ;Output a value to the device pointed to by the register
Pop       r2                      ;Pop a value off the stack into a register
Mov       r7          $128        ;print to r7
Output    r7          r2          ;Output a value to the device pointed to by the register
Exit      r2                      ;this is exit.
