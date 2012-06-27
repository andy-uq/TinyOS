Movi      r1          $1          ;move 1 into r2
Movi      r5          $11         ;move 11 into r5
Sleep     r5                      ;sleep 11 - because we sleep, this program must be launch with another app like idle to take up those sleep cycles
Printr    r1                      
Incr      r1                      
Printr    r1                      
Incr      r1                      
Printr    r1                      
Incr      r1                      
Printr    r1                      
Movi      r2          $43         ;jump 43 bytes forward
Call      r2                      
Incr      r1                      
Printr    r1                      
Movi      r3          $150        ;move 150 into r3
Movi      r4          $11         ;move 11 into r4
Movrm     r3          r4          ;move r4 into memory at r3
Callm     r3                      
Exit      r1                      ;ext
Movi      r1          $55         ;move 55 into r2
Ret                               ;ret
Printr    r1                      
Ret                               
