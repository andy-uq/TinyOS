Movi      r1          $99         ;Assign a register to a constant value
Pushr     r1                      ;Push the value of a register onto the stack
Movi      r1          $11         ;Assign a register to a constant value
Pushr     r1                      ;Push the value of a register onto the stack
Incr      r1                      ;Increase the value of a register by 1
Popr      r1                      ;Pop a value off the stack into a register
Movi      r3          $252        ;Assign a register to a constant value
Printm    r3                      ;Print a value in memory
Movi      r3          $150        ;Assign a register to a constant value
Popm      r3                      ;Pop a value off the stack and into a memory location pointed to by a register
Printm    r3                      ;Print a value in memory
Pushi     $88                     ;Push the value of a constant onto the stack
Pushi     $77                     ;Push the value of a constant onto the stack
Pushi     $66                     ;Push the value of a constant onto the stack
Popr      r2                      ;Pop a value off the stack into a register
Printr    r2                      ;Print the value in a register
Popr      r2                      ;Pop a value off the stack into a register
Printr    r2                      ;Print the value in a register
Popr      r2                      ;Pop a value off the stack into a register
Printr    r2                      ;Print the value in a register
Exit      r2                      ;this is exit.
