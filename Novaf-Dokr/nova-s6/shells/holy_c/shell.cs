using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace nova_s6.shells.holy_c
{
    public class HolyArray
    {
        public string ElementType { get; set; }
        public List<object> Elements { get; set; } = new();
        public int Length => Elements.Count;
    }

    public class HolyVariable
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public object Value { get; set; }
        public bool IsPublic { get; set; }
        public string Owner { get; set; }
        public bool IsArray => Value is HolyArray;
    }

    public class HolyFunction
    {
        public string Name { get; set; }
        public string ReturnType { get; set; }
        public List<string> Parameters { get; set; } = new();
        public List<string> Body { get; set; } = new();
    }

    public class LoopContext
    {
        public int Start { get; set; }
        public int End { get; set; }
        public string Iterator { get; set; }
        public List<string> Body { get; set; }
    }

    public class SwitchContext
    {
        public string Variable { get; set; }
        public Dictionary<string, List<string>> Cases { get; set; } = new();
        public List<string> DefaultCase { get; set; }
    }

    public static class Env
    {
        public static string CurrentDir => Environment.CurrentDirectory;
        public static string OSVersion => Environment.OSVersion.ToString();
        public static string UserName => Environment.UserName;
    }

    public class shell
    {
        public static string HolyC_Version = "~.6";
        private static Dictionary<string, HolyVariable> variables = new();
        private static Dictionary<string, HolyFunction> functions = new();
        private static Dictionary<string, LoopContext> loops = new();
        private static Dictionary<string, SwitchContext> switches = new();
        private static Stack<bool> conditionStack = new();

        public static int ate(List<string> pcS, string Filename)
        {
            if (pcS == null || pcS.Count <= 0) return 0;

            var line = string.Join(" ", pcS);

            // Control structures
            if (line.StartsWith("if") || line.StartsWith("elif") || line.StartsWith("else"))
            {
                return HandleConditional(line);
            }

            if (line.StartsWith("switch"))
            {
                return ParseSwitch(line);
            }

            if (line.StartsWith("case"))
            {
                return HandleCase(line);
            }

            // Arrays and loops
            if (line.Contains("[]"))
            {
                return HandleArrayDeclaration(pcS, Filename);
            }

            if (line.StartsWith("for"))
            {
                return ParseForLoop(line);
            }

            if (line.StartsWith("foreach"))
            {
                return ParseForeachLoop(line);
            }

            if (line.Contains("[") && line.Contains("]"))
            {
                return HandleArrayAccess(pcS);
            }

            // Functions
            if (Regex.IsMatch(line, @"^(U0|U8|U32|U64|I0|I8|I32|I64)\s+\w+\s*\(.*\)\s*{"))
            {
                return ParseFunction(pcS, line);
            }

            // Execute statements
            if (pcS[0].EndsWith(";"))
            {
                string command = pcS[0].TrimEnd(';');
                if (loops.ContainsKey(command))
                {
                    ExecuteLoop(command);
                    return 1;
                }
                if (functions.ContainsKey(command))
                {
                    ExecuteFunction(command);
                    return 1;
                }
                if (switches.ContainsKey(command))
                {
                    ExecuteSwitch(command);
                    return 1;
                }
            }

            return HandleBasicCommands(pcS, Filename);
        }

        private static int HandleConditional(string line)
        {
            if (line.StartsWith("if"))
            {
                var match = Regex.Match(line, @"if\s*\((.*)\)\s*{(.+)}");
                if (!match.Success) return 0;

                bool condition = EvaluateCondition(match.Groups[1].Value);
                conditionStack.Push(condition);

                if (condition)
                {
                    ExecuteBlock(match.Groups[2].Value);
                }
                return 1;
            }
            else if (line.StartsWith("elif"))
            {
                if (conditionStack.Count == 0) return 0;

                var match = Regex.Match(line, @"elif\s*\((.*)\)\s*{(.+)}");
                if (!match.Success) return 0;

                if (!conditionStack.Peek())
                {
                    bool condition = EvaluateCondition(match.Groups[1].Value);
                    conditionStack.Pop();
                    conditionStack.Push(condition);

                    if (condition)
                    {
                        ExecuteBlock(match.Groups[2].Value);
                    }
                }
                return 1;
            }
            else if (line.StartsWith("else"))
            {
                if (conditionStack.Count == 0) return 0;

                var match = Regex.Match(line, @"else\s*{(.+)}");
                if (!match.Success) return 0;

                if (!conditionStack.Peek())
                {
                    ExecuteBlock(match.Groups[2].Value);
                }
                conditionStack.Pop();
                return 1;
            }

            return 0;
        }

        private static bool EvaluateCondition(string condition)
        {
            // Parse condition with variable substitution
            condition = ReplaceVariables(condition);

            // Handle comparison operators
            if (condition.Contains("=="))
            {
                var parts = condition.Split("==", StringSplitOptions.TrimEntries);
                return parts[0] == parts[1];
            }
            if (condition.Contains("!="))
            {
                var parts = condition.Split("!=", StringSplitOptions.TrimEntries);
                return parts[0] != parts[1];
            }
            if (condition.Contains(">="))
            {
                var parts = condition.Split(">=", StringSplitOptions.TrimEntries);
                return double.Parse(parts[0]) >= double.Parse(parts[1]);
            }
            if (condition.Contains("<="))
            {
                var parts = condition.Split("<=", StringSplitOptions.TrimEntries);
                return double.Parse(parts[0]) <= double.Parse(parts[1]);
            }
            if (condition.Contains(">"))
            {
                var parts = condition.Split(">", StringSplitOptions.TrimEntries);
                return double.Parse(parts[0]) > double.Parse(parts[1]);
            }
            if (condition.Contains("<"))
            {
                var parts = condition.Split("<", StringSplitOptions.TrimEntries);
                return double.Parse(parts[0]) < double.Parse(parts[1]);
            }

            // Boolean value
            return bool.Parse(condition);
        }

        private static string ReplaceVariables(string text)
        {
            foreach (var variable in variables)
            {
                string pattern = $"\\${variable.Key}";
                if (Regex.IsMatch(text, pattern))
                {
                    text = Regex.Replace(text, pattern, variable.Value.Value?.ToString() ?? "");
                }
            }
            return text;
        }

        private static void ExecuteBlock(string block)
        {
            var commands = block.Split(';')
                              .Select(cmd => cmd.Trim())
                              .Where(cmd => !string.IsNullOrEmpty(cmd));

            foreach (var cmd in commands)
            {
                var parts = cmd.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                ate(parts, "");
            }
        }

        private static int ParseSwitch(string line)
        {
            var match = Regex.Match(line, @"switch\s*\((\$\w+)\)\s*{");
            if (!match.Success) return 0;

            string varName = match.Groups[1].Value.TrimStart('$');

            if (!variables.ContainsKey(varName))
            {
                HolyLogger.WriteError($"Variable {varName} not found");
                return 0;
            }

            switches[varName] = new SwitchContext { Variable = varName };
            return 1;
        }

        private static int HandleCase(string line)
        {
            if (switches.Count == 0) return 0;

            var currentSwitch = switches.Values.Last();

            if (line.StartsWith("case"))
            {
                var match = Regex.Match(line, @"case\s+(.+?)\s*:\s*{(.+)}");
                if (!match.Success) return 0;

                string caseValue = match.Groups[1].Value;
                string caseBody = match.Groups[2].Value;

                currentSwitch.Cases[caseValue] = caseBody.Split(';')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();
            }
            else if (line.StartsWith("default"))
            {
                var match = Regex.Match(line, @"default\s*:\s*{(.+)}");
                if (!match.Success) return 0;

                string defaultBody = match.Groups[1].Value;
                currentSwitch.DefaultCase = defaultBody.Split(';')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();
            }

            return 1;
        }

        private static void ExecuteSwitch(string switchVar)
        {
            if (!switches.ContainsKey(switchVar)) return;

            var switchContext = switches[switchVar];
            var value = variables[switchVar].Value?.ToString();

            if (switchContext.Cases.ContainsKey(value))
            {
                foreach (var cmd in switchContext.Cases[value])
                {
                    var parts = cmd.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    ate(parts, "");
                }
            }
            else if (switchContext.DefaultCase != null)
            {
                foreach (var cmd in switchContext.DefaultCase)
                {
                    var parts = cmd.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    ate(parts, "");
                }
            }
        }

        private static int ParseFunction(List<string> pcS, string line)
        {
            var match = Regex.Match(line, @"^(U0|U8|U32|U64|I0|I8|I32|I64)\s+(\w+)\s*\((.*?)\)\s*{(.+)}");
            if (!match.Success) return 0;

            string returnType = match.Groups[1].Value;
            string functionName = match.Groups[2].Value;
            string parameters = match.Groups[3].Value;
            string body = match.Groups[4].Value;

            var function = new HolyFunction
            {
                Name = functionName,
                ReturnType = returnType,
                Parameters = parameters.Split(',')
                    .Select(p => p.Trim())
                    .Where(p => !string.IsNullOrEmpty(p))
                    .ToList(),
                Body = body.Split(';')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList()
            };

            functions[functionName] = function;
            return 1;
        }

        private static void ExecuteFunction(string functionName)
        {
            if (!functions.ContainsKey(functionName)) return;

            var function = functions[functionName];
            foreach (var line in function.Body)
            {
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                ate(parts, "");
            }
        }

        private static int HandleArrayDeclaration(List<string> pcS, string Filename)
        {
            var match = Regex.Match(string.Join(" ", pcS), @"(U8|U32|U64|I8|I32|I64)\[\]\s+\$(\w+)");
            if (!match.Success) return 0;

            string elementType = match.Groups[1].Value;
            string arrayName = match.Groups[2].Value;

            variables[arrayName] = new HolyVariable
            {
                Name = arrayName,
                Type = $"{elementType}[]",
                Value = new HolyArray { ElementType = elementType },
                Owner = Filename
            };

            return 1;
        }

        private static int HandleArrayAccess(List<string> pcS)
        {
            var match = Regex.Match(pcS[0], @"\$(\w+)\[(\d+)\]");
            if (!match.Success) return 0;

            string arrayName = match.Groups[1].Value;
            int index = int.Parse(match.Groups[2].Value);

            if (!variables.ContainsKey(arrayName) || !variables[arrayName].IsArray)
            {
                HolyLogger.WriteError($"Array {arrayName} not found");
                return 0;
            }

            var array = variables[arrayName].Value as HolyArray;

            if (pcS[1] == "=")
            {
                string value = string.Join(" ", pcS.Skip(2));
                SetArrayElement(array, index, value);
            }
            else
            {
                if (index < array.Elements.Count)
                    Console.WriteLine(array.Elements[index]);
            }

            return 1;
        }

        private static int ParseForLoop(string line)
        {
            var match = Regex.Match(line, @"for\s*\((\w+)=(\d+)\s*to\s*(\d+)\)\s*{(.+)}");
            if (!match.Success) return 0;

            string iterator = match.Groups[1].Value;
            int start = int.Parse(match.Groups[2].Value);
            int end = int.Parse(match.Groups[3].Value);
            string body = match.Groups[4].Value;

            loops[iterator] = new LoopContext
            {
                Start = start,
                End = end,
                Iterator = iterator,
                Body = body.Split(';')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList()
            };

            return 1;
        }

        private static int ParseForeachLoop(string line)
        {
            var match = Regex.Match(line, @"foreach\s*\((\w+)\s+in\s+\$(\w+)\)\s*{(.+)}");
            if (!match.Success) return 0;

            string iterator = match.Groups[1].Value;
            string arrayName = match.Groups[2].Value;

            if (!variables.ContainsKey(arrayName) || !variables[arrayName].IsArray)
            {
                HolyLogger.WriteError($"Array {arrayName} not found");
                return 0;
            }

            var array = variables[arrayName].Value as HolyArray;
            string body = match.Groups[3].Value;

            loops[iterator] = new LoopContext
            {
                Iterator = iterator,
                Body = body.Split(';').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList(),
                Start = 0,
                End = array.Elements.Count - 1
            };

            return 1;
        }

        private static void ExecuteLoop(string iterator)
        {
            var loop = loops[iterator];

            if (loop.Iterator.StartsWith("foreach_"))
            {
                string arrayName = loop.Iterator.Split('_')[1];
                var array = variables[arrayName].Value as HolyArray;

                foreach (var element in array.Elements)
                {
                    variables[loop.Iterator] = new HolyVariable
                    {
                        Name = loop.Iterator,
                        Value = element,
                        Type = array.ElementType
                    };

                    foreach (var cmd in loop.Body)
                    {
                        var parts = cmd.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        ate(parts, "");
                    }
                }
            }
            else
            {
                for (int i = loop.Start; i <= loop.End; i++)
                {
                    variables[loop.Iterator] = new HolyVariable
                    {
                        Name = loop.Iterator,
                        Value = i,
                        Type = "I32"
                    };

                    foreach (var cmd in loop.Body)
                    {
                        var parts = cmd.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        ate(parts, "");
                    }
                }
            }
        }

        private static void SetArrayElement(HolyArray array, int index, string value)
        {
            while (array.Elements.Count <= index)
            {
                array.Elements.Add(null);
            }

            array.Elements[index] = ConvertToType(value, array.ElementType);
        }

        private static object ConvertToType(string value, string type)
        {
            try
            {
                return type switch
                {
                    "U8" => value,
                    "U32" => uint.Parse(value),
                    "U64" => ulong.Parse(value),
                    "I8" => sbyte.Parse(value),
                    "I32" => int.Parse(value),
                    "I64" => long.Parse(value),
                    _ => null
                };
            }
            catch
            {
                HolyLogger.WriteError($"Invalid value for type {type}");
                return null;
            }
        }

        private static int HandleBasicCommands(List<string> pcS, string Filename)
        {
            switch (pcS[0].ToLower())
            {
                case "cls":
                case "clear":
                    Console.Clear();
                    return 1;

                case "help":
                    ShowHelp();
                    return 1;

                case "exit":
                    Environment.Exit(0);
                    return 1;

                case "printf":
                case "println":
                    HandlePrint(pcS);
                    return 1;

                case "runfilef":
                    HandleRunFileF(pcS);
                    return 1;

                default:
                    return HandleVariableOperation(pcS);
            }
        }

        private static int HandleVariableOperation(List<string> pcS)
        {
            if (pcS[0].StartsWith("$"))
            {
                string varName = pcS[0].TrimStart('$');
                if (pcS[1] == "=")
                {
                    if (variables.ContainsKey(varName))
                    {
                        string value = string.Join(" ", pcS.Skip(2));
                        SetVariableValue(varName, value);
                        return 1;
                    }
                    else
                    {
                        string value = string.Join(" ", pcS.Skip(2));
                        SetVariableValue(varName, value);
                        return 1;
                    }
                    //HolyLogger.WriteError($"Variable {varName} not found");
                }
            }
            return 0;
        }
        private static void HandlePrint(List<string> pcS)
        {
            string output = string.Join(" ", pcS.Skip(1));
            output = ReplaceVariables(output);
            Console.WriteLine(output);
        }
        private static void HandleRunFileF(List<string> pcS)
        {
            string filepath = "G:\\s-cat\\fri3nds\\v-category-projects\\Developer-Grade-Virtual-OS\\Novs-S6-repo\\nova-s6\\Novaf-Dokr\\nova-s6\\shells\\holy_c\\examples\\ex1.HC";

            try
            {
                string code = File.ReadAllText(filepath);
                ate(code.Split(";").ToList(),$"{filepath}");
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void SetVariableValue(string varName, string value)
        {
            if (!variables.ContainsKey(varName)) return;

            var variable = variables[varName];
            variable.Value = ConvertToType(value, variable.Type);
        }
        private static void ShowHelp()
        {
            var help = new[]
            {
                @"# HolyC Shell Language Documentation

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
$myVar = ""Hello""
$publicVar = 42
```

## Arrays
```holyc
// Declaration
U8[] $myArray

// Element access
$myArray[0] = ""First""
$myArray[1] = ""Second""
Println $myArray[0]
```

## Control Structures

### If-Elif-Else
```holyc
if ($value == 10) {
    Println ""Value is 10""
} elif ($value < 10) {
    Println ""Value is less than 10""
} else {
    Println ""Value is greater than 10""
}
```

### Switch
```holyc
switch ($value) {
    case 1: { Println ""One"" }
    case 2: { Println ""Two"" }
    default: { Println ""Other"" }
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
    Println ""Hello, World!""
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
- Syntax errors"
            };
            foreach (var line in help)
                Console.WriteLine(line);
        }
    }

    public class HolyLogger
    {
        public static void WriteError(string message)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {message}");
            Console.ForegroundColor = prevColor;
        }
    }
}