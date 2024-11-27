using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace Novaf_Dokr.ScriptingLanguage
{
    public class ScriptingRuntime
    {
        public class VariableScope
        {
            private Dictionary<string, dynamic> variables = new Dictionary<string, dynamic>();
            public VariableScope ParentScope { get; private set; }

            public VariableScope(VariableScope parentScope = null)
            {
                ParentScope = parentScope;
            }

            public void SetVariable(string name, dynamic value)
            {
                variables[name] = value;
            }

            public dynamic GetVariable(string name)
            {
                if (variables.ContainsKey(name))
                    return variables[name];

                if (ParentScope != null)
                    return ParentScope.GetVariable(name);

                throw new Exception($"Variable '{name}' not defined");
            }

            public bool VariableExists(string name)
            {
                return variables.ContainsKey(name) ||
                       (ParentScope != null && ParentScope.VariableExists(name));
            }
        }

        public class Function
        {
            public List<string> Parameters { get; set; }
            public List<string> Body { get; set; }
            public VariableScope DefinitionScope { get; set; }

            public dynamic Invoke(List<dynamic> arguments, ScriptingRuntime runtime)
            {
                var functionScope = new VariableScope(DefinitionScope);

                for (int i = 0; i < Parameters.Count; i++)
                {
                    functionScope.SetVariable(Parameters[i], arguments[i]);
                }

                dynamic lastResult = null;
                foreach (var line in Body)
                {
                    lastResult = runtime.ExecuteLine(line, functionScope);
                }

                return lastResult;
            }
        }

        private VariableScope globalScope = new VariableScope();
        private Dictionary<string, Function> functions = new Dictionary<string, Function>();
        private List<string> scriptLines = new List<string>();
        private int currentLineIndex = 0;
        private static HttpClient httpClient = new HttpClient();

        public void LoadScript(string script)
        {
            scriptLines = script.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            currentLineIndex = 0;
        }

        public void LoadScriptFromFile(string filePath)
        {
            scriptLines = File.ReadAllLines(filePath).ToList();
            currentLineIndex = 0;
        }

        public dynamic ExecuteScript()
        {
            dynamic lastResult = null;
            while (currentLineIndex < scriptLines.Count)
            {
                string line = scriptLines[currentLineIndex];
                lastResult = ExecuteLine(line, globalScope);
                currentLineIndex++;
            }
            return lastResult;
        }

        public dynamic ExecuteLine(string line, VariableScope scope)
        {
            line = line.Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                return null;

            var tokens = Tokenize(line);
            return ProcessTokens(tokens, scope);
        }

        private List<string> Tokenize(string line)
        {
            var tokens = new List<string>();
            var regex = new Regex(@"(""[^""]*""|\S+)");
            foreach (Match match in regex.Matches(line))
            {
                tokens.Add(match.Value);
            }
            return tokens;
        }

        private dynamic ProcessTokens(List<string> tokens, VariableScope scope)
        {
            if (tokens.Count == 0) return null;

            string command = tokens[0].ToLower();
            tokens.RemoveAt(0);

            switch (command)
            {
                case "var":
                    return HandleVariableDeclaration(tokens, scope);
                case "set":
                    return HandleVariableAssignment(tokens, scope);
                case "print":
                    return HandlePrint(tokens, scope);
                case "input":
                    return HandleInput(tokens, scope);
                case "if":
                    return HandleConditional(tokens, scope);
                case "while":
                    return HandleWhileLoop(tokens, scope);
                case "func":
                    return HandleFunctionDefinition(tokens, scope);
                case "call":
                    return HandleFunctionCall(tokens, scope);
                case "return":
                    return HandleReturn(tokens, scope);
                case "math":
                    return HandleMathOperation(tokens, scope);
                case "list":
                    return HandleListOperation(tokens, scope);
                case "dict":
                    return HandleDictionaryOperation(tokens, scope);
                case "file":
                    return HandleFileOperation(tokens, scope);
                case "shell":
                    return HandleShellCommand(tokens, scope);
                case "import":
                    return HandleImport(tokens, scope);
                case "json":
                    return HandleJsonOperation(tokens, scope);
                case "http":
                    return HandleHttpRequest(tokens, scope);
                case "datetime":
                    return HandleDateTimeOperation(tokens, scope);
                case "regex":
                    return HandleRegexOperation(tokens, scope);
                case "try":
                    return HandleTryCatch(tokens, scope);
                case "random":
                    return HandleRandomOperation(tokens, scope);
                case "string":
                    return HandleStringOperation(tokens, scope);
                case "type":
                    return HandleTypeOperation(tokens, scope);
                default:
                    throw new Exception($"Unknown command: {command}");
            }
        }

        private dynamic HandleVariableDeclaration(List<string> tokens, VariableScope scope)
        {
            if (tokens.Count < 2)
                throw new Exception("Invalid variable declaration");

            string varName = tokens[0];
            tokens.RemoveAt(0);
            string value = string.Join(" ", tokens);
            dynamic parsedValue = ParseValue(value);
            scope.SetVariable(varName, parsedValue);
            return parsedValue;
        }

        private dynamic HandleVariableAssignment(List<string> tokens, VariableScope scope)
        {
            if (tokens.Count < 2)
                throw new Exception("Invalid variable assignment");

            string varName = tokens[0];
            tokens.RemoveAt(0);
            string value = string.Join(" ", tokens);
            dynamic parsedValue = ParseValue(value);

            if (scope.VariableExists(varName))
            {
                scope.SetVariable(varName, parsedValue);
            }
            else
            {
                throw new Exception($"Variable '{varName}' not defined");
            }
            return parsedValue;
        }

        private dynamic HandlePrint(List<string> tokens, VariableScope scope)
        {
            string output = string.Join(" ", tokens.Select(t =>
                t.StartsWith("\"") && t.EndsWith("\"")
                    ? t.Trim('"')
                    : (scope.VariableExists(t) ? scope.GetVariable(t).ToString() : t)
            ));
            Console.WriteLine(output);
            return output;
        }

        private dynamic HandleInput(List<string> tokens, VariableScope scope)
        {
            if (tokens.Count < 1)
                throw new Exception("Input requires a variable name");

            string prompt = tokens.Count > 1 ? string.Join(" ", tokens.Skip(1)) : "> ";
            Console.Write(prompt);
            string input = Console.ReadLine();
            dynamic parsedInput = ParseValue(input);
            scope.SetVariable(tokens[0], parsedInput);
            return parsedInput;
        }
        private dynamic HandleShellCommand(List<string> tokens, VariableScope scope)
        {
            if (tokens.Count < 1)
                throw new Exception("Invalid shell command");

            string command = string.Join(" ", tokens);
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "bash",
                Arguments = $"-c \"{command}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi))
            {
                string result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return result.Trim();
            }
        }

        private dynamic HandleImport(List<string> tokens, VariableScope scope)
        {
            if (tokens.Count < 1)
                throw new Exception("Invalid import");

            string moduleName = tokens[0];
            try
            {
                Assembly assembly = Assembly.Load(moduleName);
                return assembly;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to import module {moduleName}: {ex.Message}");
            }
        }
        private dynamic HandleConditional(List<string> tokens, VariableScope scope)
        {
            if (tokens.Count < 3)
                throw new Exception("Invalid if statement");

            dynamic left = ParseValue(tokens[0], scope);
            string op = tokens[1];
            dynamic right = ParseValue(tokens[2], scope);

            bool conditionResult = false;
            switch (op)
            {
                case "==": conditionResult = left == right; break;
                case "!=": conditionResult = left != right; break;
                case "<": conditionResult = left < right; break;
                case "<=": conditionResult = left <= right; break;
                case ">": conditionResult = left > right; break;
                case ">=": conditionResult = left >= right; break;
                default: throw new Exception($"Invalid comparison operator: {op}");
            }

            return conditionResult;
        }

        private dynamic HandleWhileLoop(List<string> tokens, VariableScope scope)
        {
            if (tokens.Count < 3)
                throw new Exception("Invalid while loop");

            dynamic lastResult = null;
            while (HandleConditional(tokens, scope))
            {
                foreach (var line in tokens.Skip(3))
                {
                    lastResult = ExecuteLine(line, new VariableScope(scope));
                }
            }
            return lastResult;
        }

        private dynamic HandleFunctionDefinition(List<string> tokens, VariableScope scope)
        {
            if (tokens.Count < 2)
                throw new Exception("Invalid function definition");

            string funcName = tokens[0];
            tokens.RemoveAt(0);

            int bodyStart = tokens.IndexOf("{");
            int bodyEnd = tokens.IndexOf("}");

            if (bodyStart == -1 || bodyEnd == -1)
                throw new Exception("Invalid function body");

            var parameters = tokens.Take(bodyStart).ToList();
            var body = tokens.Skip(bodyStart + 1).Take(bodyEnd - bodyStart - 1).ToList();

            functions[funcName] = new Function
            {
                Parameters = parameters,
                Body = body,
                DefinitionScope = scope
            };

            return funcName;
        }

        private dynamic HandleFunctionCall(List<string> tokens, VariableScope scope)
        {
            if (tokens.Count < 1)
                throw new Exception("Invalid function call");

            string funcName = tokens[0];
            tokens.RemoveAt(0);

            if (!functions.ContainsKey(funcName))
                throw new Exception($"Function '{funcName}' not defined");

            var func = functions[funcName];
            var arguments = tokens.Select(t => ParseValue(t, scope)).ToList();

            if (arguments.Count != func.Parameters.Count)
                throw new Exception($"Function '{funcName}' expects {func.Parameters.Count} arguments");

            return func.Invoke(arguments, this);
        }

        private dynamic HandleListOperation(List<string> tokens, VariableScope scope)
        {
            if (tokens.Count < 2)
                throw new Exception("Invalid list operation");

            string op = tokens[0];
            string listName = tokens[1];

            switch (op)
            {
                case "create":
                    var newList = new List<dynamic>();
                    scope.SetVariable(listName, newList);
                    return newList;
                case "add":
                    var list = scope.GetVariable(listName);
                    dynamic value = ParseValue(tokens[2], scope);
                    list.Add(value);
                    return list;
                case "remove":
                    var removeList = scope.GetVariable(listName);
                    dynamic removeValue = ParseValue(tokens[2], scope);
                    removeList.Remove(removeValue);
                    return removeList;
                case "get":
                    var getList = scope.GetVariable(listName);
                    int index = int.Parse(tokens[2]);
                    return getList[index];
                default:
                    throw new Exception($"Unknown list operation: {op}");
            }
        }

        private dynamic HandleDictionaryOperation(List<string> tokens, VariableScope scope)
        {
            if (tokens.Count < 2)
                throw new Exception("Invalid dictionary operation");

            string op = tokens[0];
            string dictName = tokens[1];

            switch (op)
            {
                case "create":
                    var newDict = new Dictionary<string, dynamic>();
                    scope.SetVariable(dictName, newDict);
                    return newDict;
                case "set":
                    var dict = scope.GetVariable(dictName);
                    string key = tokens[2];
                    dynamic value = ParseValue(tokens[3], scope);
                    dict[key] = value;
                    return dict;
                case "get":
                    var getDict = scope.GetVariable(dictName);
                    string getKey = tokens[2];
                    return getDict[getKey];
                default:
                    throw new Exception($"Unknown dictionary operation: {op}");
            }
        }

        private dynamic HandleFileOperation(List<string> tokens, VariableScope scope)
        {
            if (tokens.Count < 2)
                throw new Exception("Invalid file operation");

            string op = tokens[0];
            string filePath = tokens[1];

            switch (op)
            {
                case "write":
                    string content = string.Join(" ", tokens.Skip(2));
                    File.WriteAllText(filePath, content);
                    return filePath;
                case "append":
                    string appendContent = string.Join(" ", tokens.Skip(2));
                    File.AppendAllText(filePath, appendContent);
                    return filePath;
                case "read":
                    return File.ReadAllText(filePath);
                case "delete":
                    File.Delete(filePath);
                    return filePath;
                default:
                    throw new Exception($"Unknown file operation: {op}");
            }
        }

        private dynamic HandleReturn(List<string> tokens, VariableScope scope)
        {
            if (tokens.Count < 1)
                return null;

            return ParseValue(tokens[0], scope);
        }

        private dynamic HandleMathOperation(List<string> tokens, VariableScope scope)
        {
            if (tokens.Count < 3)
                throw new Exception("Invalid math operation");

            string op = tokens[0];
            dynamic a = ParseValue(tokens[1], scope);
            dynamic b = ParseValue(tokens[2], scope);

            switch (op)
            {
                case "add": return a + b;
                case "sub": return a - b;
                case "mul": return a * b;
                case "div": return a / b;
                case "mod": return a % b;
                case "pow": return Math.Pow(a, b);
                default: throw new Exception($"Unknown math operation: {op}");
            }
        }

        private dynamic HandleJsonOperation(List<string> tokens, VariableScope scope)
        {
            if (tokens.Count < 2)
                throw new Exception("Invalid JSON operation");

            string op = tokens[0];
            string jsonData = string.Join(" ", tokens.Skip(1));

            switch (op)
            {
                case "parse":
                    return System.Text.Json.JsonSerializer.Deserialize<dynamic>(jsonData);
                case "stringify":
                    return System.Text.Json.JsonSerializer.Serialize(ParseValue(jsonData, scope));
                default:
                    throw new Exception($"Unknown JSON operation: {op}");
            }
        }

        private dynamic HandleHttpRequest(List<string> tokens, VariableScope scope)
        {
            if (tokens.Count < 2)
                throw new Exception("Invalid HTTP request");

            string method = tokens[0].ToUpper();
            string url = tokens[1];

            switch (method)
            {
                case "GET":
                    return Task.Run(() => httpClient.GetStringAsync(url)).Result;
                case "POST":
                    if (tokens.Count < 3)
                        throw new Exception("POST requires body");
                    string body = string.Join(" ", tokens.Skip(2));
                    var content = new StringContent(body, Encoding.UTF8, "application/json");
                    return Task.Run(() => httpClient.PostAsync(url, content).Result.Content.ReadAsStringAsync()).Result;
                default:
                    throw new Exception($"Unsupported HTTP method: {method}");
            }
        }

        private dynamic HandleDateTimeOperation(List<string> tokens, VariableScope scope)
        {
            if (tokens.Count < 1)
                throw new Exception("Invalid DateTime operation");

            string op = tokens[0];

            switch (op)
            {
                case "now":
                    return DateTime.Now;
                case "today":
                    return DateTime.Today;
                case "format":
                    if (tokens.Count < 3)
                        throw new Exception("DateTime format requires date and format string");
                    DateTime date = DateTime.Parse(tokens[1]);
                    string format = tokens[2].Trim('"');
                    return date.ToString(format);
                case "parse":
                    if (tokens.Count < 2)
                        throw new Exception("DateTime parse requires a date string");
                    return DateTime.Parse(tokens[1]);
                default:
                    throw new Exception($"Unknown DateTime operation: {op}");
            }
        }

        private dynamic HandleRegexOperation(List<string> tokens, VariableScope scope)
        {
            if (tokens.Count < 3)
                throw new Exception("Invalid Regex operation");

            string op = tokens[0];
            string pattern = tokens[1];
            string input = tokens[2];

            switch (op)
            {
                case "match":
                    return Regex.IsMatch(input, pattern);
                case "find":
                    var matches = Regex.Matches(input, pattern);
                    return matches.Cast<Match>().Select(m => m.Value).ToList();
                case "replace":
                    if (tokens.Count < 4)
                        throw new Exception("Regex replace requires replacement");
                    return Regex.Replace(input, pattern, tokens[3]);
                default:
                    throw new Exception($"Unknown Regex operation: {op}");
            }
        }

        private dynamic HandleTryCatch(List<string> tokens, VariableScope scope)
        {
            try
            {
                string line = string.Join(" ", tokens);
                return ExecuteLine(line, scope);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return ex.Message;
            }
        }

        private dynamic HandleRandomOperation(List<string> tokens, VariableScope scope)
        {
            if (tokens.Count < 1)
                throw new Exception("Invalid Random operation");

            Random rnd = new Random();
            string op = tokens[0];

            switch (op)
            {
                case "int":
                    if (tokens.Count < 3)
                        throw new Exception("Random int requires min and max");
                    int min = int.Parse(tokens[1]);
                    int max = int.Parse(tokens[2]);
                    return rnd.Next(min, max);
                case "double":
                    return rnd.NextDouble();
                case "choice":
                    if (tokens.Count < 2)
                        throw new Exception("Random choice requires at least one option");
                    var choices = tokens.Skip(1).ToList();
                    return choices[rnd.Next(choices.Count)];
                default:
                    throw new Exception($"Unknown Random operation: {op}");
            }
        }

        private dynamic HandleStringOperation(List<string> tokens, VariableScope scope)
        {
            if (tokens.Count < 2)
                throw new Exception("Invalid String operation");

            string op = tokens[0];
            string input = tokens[1];

            switch (op)
            {
                case "length":
                    return input.Length;
                case "upper":
                    return input.ToUpper();
                case "lower":
                    return input.ToLower();
                case "trim":
                    return input.Trim();
                case "split":
                    if (tokens.Count < 3)
                        throw new Exception("String split requires a delimiter");
                    return input.Split(tokens[2].ToCharArray()).ToList();
                case "join":
                    if (tokens.Count < 3)
                        throw new Exception("String join requires a delimiter");
                    var list = ParseValue(tokens[2], scope) as List<string>;
                    return string.Join(tokens[1], list);
                case "substring":
                    if (tokens.Count < 4)
                        throw new Exception("String substring requires start and length");
                    int start = int.Parse(tokens[2]);
                    int length = int.Parse(tokens[3]);
                    return input.Substring(start, length);
                default:
                    throw new Exception($"Unknown String operation: {op}");
            }
        }

        private dynamic HandleTypeOperation(List<string> tokens, VariableScope scope)
        {
            if (tokens.Count < 1)
                throw new Exception("Invalid Type operation");

            string op = tokens[0];
            dynamic value = ParseValue(tokens[1], scope);

            switch (op)
            {
                case "of":
                    return value.GetType().Name;
                case "convert":
                    if (tokens.Count < 3)
                        throw new Exception("Type conversion requires target type");
                    string targetType = tokens[2];
                    switch (targetType.ToLower())
                    {
                        case "int": return Convert.ToInt32(value);
                        case "double": return Convert.ToDouble(value);
                        case "string": return value.ToString();
                        case "bool": return Convert.ToBoolean(value);
                        default: throw new Exception($"Unsupported conversion type: {targetType}");
                    }
                default:
                    throw new Exception($"Unknown Type operation: {op}");
            }
        }

        // The ParseValue method remains the same as in the previous implementation
        private dynamic ParseValue(string value, VariableScope scope = null)
        {
            // Same implementation as before
            if (value == null) return null;

            value = value.Trim();

            if (value.StartsWith("\"") && value.EndsWith("\""))
                return value.Trim('"');

            if (int.TryParse(value, out int intValue))
                return intValue;

            if (double.TryParse(value, out double doubleValue))
                return doubleValue;

            if (bool.TryParse(value, out bool boolValue))
                return boolValue;

            if (scope != null && scope.VariableExists(value))
                return scope.GetVariable(value);

            return value;
        }
    }
}