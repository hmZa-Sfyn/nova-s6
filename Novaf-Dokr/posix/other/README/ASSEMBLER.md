# `emza-POSIX`

## **Assembler:**

_Assembler Syntax_

# Just a post-bit computer.

### A bit is comprised of 26 things (or values) called alphabatz.
### ```a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z``` not like a normal computer which words on just ```0,1```.

| ## | Instruction    | Translates To | Desc                                                       | Example                |                   
|----|----------------|---------------|------------------------------------------------------------|------------------------|
|1|nop             | aaaa          | No Operation.                                              |`nop`                   |
|2|rem             | aaab          | A normal comment, Compiler ignores the rest of the line.   |`rem abc`               |
|3||                     
|4|regs $reg $val  | baaa          | To assign a value to a register you name.                  |`regs reg1 125000`      |
|5|regl $reg       | baab          | To load a value from a registered register.                |`regl reg1`             |
|-||
|6|add $r1 $r2     | caaa          | To add, and return. (r1 + r2)                              |`add reg19 reg43`       | 
|7|subt $r1 $r2    | caab          | To subtract, and return. (r1 - r2)                         |`subt reg45 reg19`      |
|8|mult $r2 $r2    | caac          | To multiply, and return. (r1 * r2)                         |`mult reg19 reg45`      |
|9|divd $r2 $r2    | caad          | To divide, and return. (r1 / r2)                           |`divd reg19 reg67`      | 
|-||
|10|dspr $r1        | daaa          | To display the value of r1. (print r1)                     |`dspr reg56`            |
|11|dspv $hello     | daab          | To diaplay that value `hello`. (print "hello")             |`dspr hello worlds!`    |
|-||
|12|jmp $point      | eaaa          | To jump to $point. (goto abc)                              |`abc:` ............... `jmp <abc>`
|-||
|13|ift $cond then $do | faaa       | To match the condition `True` $cond and then $do according.|`if <RET >= 125> then jmp <abc>`|
|14|asr $cond       | faab          | Just a normal assert operater.                             |`asr <RET == 125>`      |
|-||
|15|drf $r1 $r2     | gaaa          | (r1 - r2), and return.                                     |`drf reg45 reg16`       |
|16|frt $r1 _num_   | gaab          | (_num_ root of $r1) and return.                            |(if _num_ = 2), then `frt reg16` returns `4`|
|17|pow $r1 _num_   | gaac          | (value of $r1 ^ _num_) and return.                         |(if _num_ = 2), then `pow reg4 2` returns `16`|
|18|mod $r1 _num_   | gaad          | ($r1 % _num_) and return.                                  |(if _num_ = 2), then `mod reg24 2` returns `0`|
|-||
|19|call $func $arg... | haaa       | To call special function $func, args can be more then `1`  |`call _add_2_to_this_ reg15`|





# Example Program
```zasm

rem TEST

regs reg2 125
regl reg2

dspr reg2

```
