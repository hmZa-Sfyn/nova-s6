# HolyC Shell Language Documentation

## Overview
HolyC Shell is a C-like programming language implementation with support for variables, arrays, control structures, and functions.

## Data Types
- `U0`: Void type (unsigned)
- `U8`: Unsigned 8-bit / String type
- `U32`: Unsigned 32-bit integer
- `U64`: Unsigned 64-bit integer
- `I0`: Void type (signed)
- `I8`: Signed 8-bit integer
- `I32`: Signed 32-bit integer
- `I64`: Signed 64-bit integer

## Variables
```holyc
// Declaration
New U8 $myVar
New Pub U32 $publicVar

// Assignment
$myVar = "Hello"
$publicVar = 42
```

## Arrays
```holyc
// Declaration
U8[] $myArray

// Element access
$myArray[0] = "First"
$myArray[1] = "Second"
Println $myArray[0]
```

## Control Structures

### If-Elif-Else
```holyc
if ($value == 10) {
    Println "Value is 10"
} elif ($value < 10) {
    Println "Value is less than 10"
} else {
    Println "Value is greater than 10"
}
```

### Switch
```holyc
switch ($value) {
    case 1: { Println "One" }
    case 2: { Println "Two" }
    default: { Println "Other" }
}
```

## Loops

### For Loop
```holyc
for(i=0 to 5) {
    Println $i
} i;
```

### Foreach Loop
```holyc
foreach(item in $myArray) {
    Println item
} item;
```

## Functions
```holyc
U0 PrintHello() {
    Println "Hello, World!"
}

U32 Add(U32 a, U32 b) {
    return a + b
}
```

## Environment Variables
- `*Env.CurrentDir`: Current directory
- `*Env.OSVersion`: Operating system version
- `*Env.UserName`: Current username

## Built-in Commands
- `Printf`: Print without newline
- `Println`: Print with newline
- `Cls`/`Clear`: Clear screen
- `Exit`: Exit shell

## Error Handling
Errors are displayed in red with descriptive messages for:
- Invalid types
- Undefined variables
- Array bounds
- Syntax errors