# `emza-POSIX`

## **Assembler:**

_Assembler Syntax_

# Just a 128-bit computer.
# Enjoy the 128^th^ ness

### 00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001
### higest possible number `3.4028236692093846346337460743177` ^`38`^

```zasm
	nop --> No Operation.
	rem --> Just A Comment.

	regs $register_identifier $value --> To assign $value to $register_identifier.
	regl $register_identifier        --> To load $register_identifier register.

	add   -> Some basic (A.L.U) operations 
	subt  -|
	mult  -|
	div   -|

	dspr $register_identifier --> To print the register's value.
	dspv $value --> To  print $value.

```

# Example Program
```

rem TEST

regs reg2 125
regl reg2

dspr reg2

```
