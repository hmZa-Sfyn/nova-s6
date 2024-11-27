using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Diagnostics;
using System.Collections;

using nova.Utils;
using nova.Command;
using nova.Command.env;
using System.Runtime.ExceptionServices;
using System.Reflection.Metadata.Ecma335;
using Novaf_Dokr.Customization.configuration;
using Novaf_Dokr.Customization;
using System.Diagnostics.SymbolStore;
using Novaf_Dokr.Customization.lang.xMake;
using Novaf_Dokr.Command.env.u_f_net;
using System.Xml.Linq;
//using nova.Tests;


namespace novaf
{
    public class Program
    {
        public static string __version__ = "4.1.xy.24";
        public static string __shell__ = "hqsh";
        static void Main(string[] args)
        {
            //Console.WriteLine("(c) nova Initial Developers | Fri3nds .G");
            DesignFormat.Banner();

            Initnova(false);
        }

        public class Shell
        { 
            public int Num { get; set; }
            public string Name { get; set; }
            public string Desc { get; set; }
            public string Version { get; set; }
        } 

        // Helper method for recursive directory copy
        public void CopyDirectory(string sourceDir, string destDir)
        {
            Directory.CreateDirectory(destDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                File.Copy(file, Path.Combine(destDir, Path.GetFileName(file)), true);
            }

            foreach (var subDir in Directory.GetDirectories(sourceDir))
            {
                CopyDirectory(subDir, Path.Combine(destDir, Path.GetFileName(subDir)));
            }
        }

        // Helper method for find command
        private static void FindFiles(string path, string pattern, bool recursive)
        {
            try
            {
                if (pattern == null)
                {
                    Console.WriteLine(path);
                }
                else
                {
                    var matchingFiles = Directory.GetFiles(path, pattern,
                        recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                    foreach (var file in matchingFiles)
                    {
                        Console.WriteLine(file);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching {path}: {ex.Message}");
            }
        }
        // Helper methods for du command
        private static long CalculateDirSize(DirectoryInfo dir)
        {
            long size = 0;
            try
            {
                foreach (var file in dir.GetFiles())
                    size += file.Length;

                foreach (var subdir in dir.GetDirectories())
                    size += CalculateDirSize(subdir);
            }
            catch { }
            return size;
        }

        private static string FormatSize(long bytes, bool humanReadable)
        {
            if (!humanReadable) return bytes.ToString();

            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;

            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }
        public static void Initnova(bool isctrlc)
        {
            #region UnitTests
            //UnitTests.Test1();
            //UnitTests.Test2();
            //UnitTests.Test3();
            //UnitTests.Test4();
            //UnitTests.Test_RunCmd();

            //nova.Command.env.Vars.main_test();
            //UnitTests.Test_Exit();
            #endregion

            #region shall tests
            //abc:
            //List<string> parts= new List<string>();
            //parts = ["@bin","ls","--path", "$(BinPath)"];
            //for (int i = 0; i < parts.Count; i++)
            //{
            //    if (parts[i].StartsWith("$("))
            //    {
            //        parts[i] = parts[i].Replace("$(", "");
            //        parts[i] = parts[i].Replace(")", "");

            //        Console.WriteLine($":: {parts[i]}");
            //    }
            //    else
            //    {

            //    }
            //}
            //Console.ReadLine();
            //goto abc;

            //RunOnWindows.RunPythonFile(["C:\\Users\\Hamza\\vin_env\\ibin\\python\\usmnh\\tabl.py"]);
            //Environment.Exit(0);

            //{
            //    List<string> commands = UserInput.Prepare("@sudev do run d:G:\\fri3nds\\v-category-projects\\Developer-Grade-Virtual-OS\\Ju-hind-F\\Ju-Hind-F\\Ju-Hind-F\\Command\\env\\doker_lang\\test.dokr");

            //    IdentifyCommand.Identify(commands);
            //    List<string> parsed_commands = IdentifyCommand.ReturnThemPlease();

            //    PleaseCommandEnv.TheseCommands(parsed_commands);

            //    IdentifyCommand.CacheClean();

            //    Environment.Exit(0);
            //}

            //{
            //    nova_vm.Command.env.doker_lang.Interpreter.Interpret.Helper.ThisCode();


            //    Environment.Exit(0);

            //}

            //string Code = """
            //    rem TEST ;

            //    regs $reg2 125 ;
            //    regl $reg2 ;

            //    dspr $reg2 ;
            //    """;

            //foreach (var item in Novaf_Dokr.posix.emulater.compiler.To_Bytecode(Code))
            //{
            //    Console.WriteLine(item);
            //}

            //Environment.Exit(0);
            #endregion

            #region Actual Init

            CommandEnv.LoadEnvironmentPointers();
            CommandEnv.LoadEnvironmentVariables();

            master.conf.EnsureEnvironmentSetup();

            //foreach (master.conf.ConfigurationData _cd in master._Conf_Files_List)
            //{
            //    Console.WriteLine(_cd.File.Path.ToString());
            //}

            List<Shell> Shells = new List<Shell>();

            Shells.Add(new Shell
            {
                Num = 0,
                Name = "hqsh",
                Desc = "The default shell emulater, for `nova-s6` systems, not in previous distros.",
                Version = __version__
            });

            Shells.Add(new Shell
            {
                Num = 1,
                Name = "hsh",
                Desc = "The secondary/customized shell for `Novaf-Dokr` and other newer distros.",
                Version = "27/11/2024"
            });

            Console.WriteLine($"\n(*) sh: type: `!sh help` for `sh-help`.");

            try
            {
                while (true)
                {
                    _entry_point_main:
                    try
                    {
                        // Handle CTRL+C key press to prevent quitting
                        Console.CancelKeyPress += (sender, e) =>
                        {
                            e.Cancel = false; // Prevent the app from closing
                            Console.WriteLine("\n<CTRL-C>: type `@exit` to exit!");
                            Console.ForegroundColor = XmInterpreter.__CurrentForegroundColor;
                            Console.BackgroundColor = XmInterpreter.__CurrentBackgroundColor;
                            Initnova(true);
                        };

                        if (!isctrlc)
                        {
                            // The rest of your code
                            DesignFormat.TakeInput([$"\n{CommandEnv.CurrentDirDest}",$"\n [{DesignFormat.get_shell_icon(__shell__)}] {CommandEnv.CURRENT_USER_NAME}",$"> "]);
                            List<string> commands = UserInput.Prepare(UserInput.Input());
                            IdentifyCommand.Identify(commands);
                            List<string> parsed_commands = IdentifyCommand.ReturnThemPlease();

                            if (parsed_commands[0].StartsWith("!"))
                            {
                                if (parsed_commands[0] == "!csh")
                                {
                                    if (parsed_commands.Count < 1)
                                        return;

                                    if (parsed_commands[1].ToLower() == "--list")
                                    {

                                        foreach (var shell in Shells)
                                        {
                                            Console.WriteLine($"{shell.Num,-3}: {shell.Name,-6} :{DesignFormat.get_shell_icon(shell.Name),-2} {shell.Desc,-90} {shell.Version,-20} ");
                                        }
                                    }
                                    else if (parsed_commands[1].ToLower() == "+d")
                                    {
                                        if (parsed_commands.Count < 2)
                                            return;

                                        __shell__ = parsed_commands[2];
                                    }
                                    else if (parsed_commands[1].ToLower() == "--help")
                                    {
                                        Console.WriteLine(" !csh --list    :: To list shells. " +
                                                        "\n !csh --help    :: To show this help message." +
                                                        "\n !csh +d $shell :: To deploy `$shell` and logout from the previoes one.");
                                    }
                                    else
                                    {
                                        errs.CacheClean();
                                        errs.New($"sh: csh: type: `!csh --help` for help.");
                                        errs.ListThemAll();
                                        errs.CacheClean();
                                    }
                                }
                                if (parsed_commands[0] == "!sh")
                                {
                                    if (parsed_commands.Count < 2)
                                        return;

                                    else if (parsed_commands[1].ToLower() == "--help")
                                    {
                                        Console.WriteLine("Shell Commands Help:\n" +
                                                          "=====================\n" +
                                                          " !sh      :: Manage the built-in shell emulator.\n" +
                                                          " !csh     :: Change and manage custom shells.\n" +
                                                          " !pwd     :: Print the current working directory.\n" +
                                                          " !ls      :: List directory contents.\n" +
                                                          "            Options: -a/--all (show hidden), -l (long format)\n" +
                                                          " !cat     :: Display contents of a file.\n" +
                                                          " !touch   :: Create an empty file or update its timestamp.\n" +
                                                          " !mkdir   :: Create directories.\n" +
                                                          "            Options: -p/--parents (create parent directories as needed).\n" +
                                                          " !rm      :: Remove files or directories.\n" +
                                                          "            Options: -r/--recursive, -f/--force.\n" +
                                                          " !cp      :: Copy files or directories.\n" +
                                                          "            Options: -r/--recursive (for directories).\n" +
                                                          " !grep    :: Search for patterns in files.\n" +
                                                          "            Options: -i (case-insensitive search).\n" +
                                                          " !wc      :: Count words, lines, and characters in a file.\n" +
                                                          "            Options: -l (lines), -w (words), -c (characters).\n" +
                                                          " !head    :: Display the first lines of a file.\n" +
                                                          "            Options: -n <number> (number of lines).\n" +
                                                          " !tail    :: Display the last lines of a file.\n" +
                                                          "            Options: -n <number> (number of lines).\n" +
                                                          " !echo    :: Print arguments to the console.\n" +
                                                          "            Options: -n (suppress newline).\n" +
                                                          " !which   :: Locate the path of an executable.\n");
                                    }
                                    else
                                    {
                                        errs.CacheClean();
                                        errs.New("sh: type `!sh --help` for help.");
                                        errs.ListThemAll();
                                        errs.CacheClean();
                                    }
                                }

                                // New Builtin Commands

                                // !pwd - Print Working Directory
                                if (parsed_commands[0] == "!pwd")
                                {
                                    Console.WriteLine(Directory.GetCurrentDirectory());
                                }

                                // !ls - List Directory Contents
                                if (parsed_commands[0] == "!ls")
                                {
                                    string path = ".";
                                    bool showHidden = false;
                                    bool longFormat = false;

                                    for (int i = 1; i < parsed_commands.Count; i++)
                                    {
                                        if (parsed_commands[i] == "-a" || parsed_commands[i] == "--all")
                                            showHidden = true;
                                        else if (parsed_commands[i] == "-l")
                                            longFormat = true;
                                        else if (parsed_commands[i].StartsWith("-"))
                                        {
                                            errs.New($"ls: invalid option {parsed_commands[i]}");
                                            continue;
                                        }
                                        else
                                        {
                                            path = parsed_commands[i];
                                        }
                                    }

                                    try
                                    {
                                        string[] files = Directory.GetFiles(path);
                                        string[] directories = Directory.GetDirectories(path);

                                        if (longFormat)
                                        {
                                            foreach (var dir in directories)
                                            {
                                                var dirInfo = new DirectoryInfo(dir);
                                                Console.WriteLine($"d{dirInfo.Attributes,-10} {dirInfo.CreationTime,-20} {Path.GetFileName(dir),-30}");
                                            }

                                            foreach (var file in files)
                                            {
                                                var fileInfo = new FileInfo(file);
                                                Console.WriteLine($"-{fileInfo.Attributes,-10} {fileInfo.CreationTime,-20} {fileInfo.Name,-30} {fileInfo.Length,-10} bytes");
                                            }
                                        }
                                        else
                                        {
                                            foreach (var dir in directories)
                                            {
                                                Console.WriteLine(Path.GetFileName(dir));
                                            }

                                            foreach (var file in files)
                                            {
                                                Console.WriteLine(Path.GetFileName(file));
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        errs.New($"ls: cannot access '{path}': {ex.Message}");
                                    }
                                }

                                // !cat - Display File Contents
                                if (parsed_commands[0] == "!cat")
                                {
                                    if (parsed_commands.Count < 2)
                                    {
                                        errs.New("cat: missing file operand");
                                        return;
                                    }

                                    try
                                    {
                                        for (int i = 1; i < parsed_commands.Count; i++)
                                        {
                                            string content = File.ReadAllText(parsed_commands[i]);
                                            Console.WriteLine(content);
                                        }
                                    }
                                    catch (FileNotFoundException)
                                    {
                                        errs.New($"cat: {parsed_commands[1]}: No such file or directory");
                                    }
                                    catch (Exception ex)
                                    {
                                        errs.New($"cat: error reading file: {ex.Message}");
                                    }
                                }

                                // !touch - Create Empty File or Update Timestamp
                                if (parsed_commands[0] == "!touch")
                                {
                                    if (parsed_commands.Count < 2)
                                    {
                                        errs.New("touch: missing file operand");
                                        return;
                                    }

                                    try
                                    {
                                        foreach (var filename in parsed_commands.Skip(1))
                                        {
                                            if (!File.Exists(filename))
                                            {
                                                using (File.Create(filename)) { }
                                            }
                                            else
                                            {
                                                File.SetLastWriteTime(filename, DateTime.Now);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        errs.New($"touch: cannot touch '{parsed_commands[1]}': {ex.Message}");
                                    }
                                }

                                // !mkdir - Create Directories
                                if (parsed_commands[0] == "!mkdir")
                                {
                                    if (parsed_commands.Count < 2)
                                    {
                                        errs.New("mkdir: missing operand");
                                        return;
                                    }

                                    bool parents = parsed_commands.Contains("-p") || parsed_commands.Contains("--parents");

                                    try
                                    {
                                        foreach (var dir in parsed_commands.Skip(1).Where(x => x != "-p" && x != "--parents"))
                                        {
                                            if (parents)
                                                Directory.CreateDirectory(dir);
                                            else
                                                Directory.CreateDirectory(dir);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        errs.New($"mkdir: cannot create directory: {ex.Message}");
                                    }
                                }

                                // !rm - Remove Files or Directories
                                if (parsed_commands[0] == "!rm")
                                {
                                    if (parsed_commands.Count < 2)
                                    {
                                        errs.New("rm: missing operand");
                                        return;
                                    }

                                    bool recursive = parsed_commands.Contains("-r") || parsed_commands.Contains("--recursive");
                                    bool force = parsed_commands.Contains("-f") || parsed_commands.Contains("--force");

                                    try
                                    {
                                        foreach (var path in parsed_commands.Skip(1).Where(x => x != "-r" && x != "--recursive" && x != "-f" && x != "--force"))
                                        {
                                            if (File.Exists(path))
                                            {
                                                File.Delete(path);
                                            }
                                            else if (Directory.Exists(path))
                                            {
                                                if (recursive)
                                                    Directory.Delete(path, true);
                                                else
                                                {
                                                    errs.New($"rm: cannot remove '{path}': Is a directory");
                                                }
                                            }
                                            else if (!force)
                                            {
                                                errs.New($"rm: cannot remove '{path}': No such file or directory");
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        errs.New($"rm: cannot remove: {ex.Message}");
                                    }
                                }

                                //// !cp - Copy Files
                                //if (parsed_commands[0] == "!cp")
                                //{
                                //    if (parsed_commands.Count < 3)
                                //    {
                                //        errs.New("cp: missing file operand");
                                //        return;
                                //    }

                                //    bool recursive = parsed_commands.Contains("-r") || parsed_commands.Contains("--recursive");

                                //    try
                                //    {
                                //        string destination = parsed_commands[parsed_commands.Count - 1];
                                //        var sources = parsed_commands.Skip(1).Take(parsed_commands.Count - 2)
                                //            .Where(x => x != "-r" && x != "--recursive");

                                //        foreach (var source in sources)
                                //        {
                                //            if (File.Exists(source))
                                //            {
                                //                File.Copy(source, Path.Combine(destination, Path.GetFileName(source)), true);
                                //            }
                                //            else if (Directory.Exists(source) && recursive)
                                //            {
                                //                Directory.CreateDirectory(destination);
                                //                CopyDirectory(source.ToString(), Path.Combine(destination, Path.GetFileName(source)).ToString());
                                //            }
                                //            else
                                //            {
                                //                errs.New($"cp: cannot stat '{source}': No such file or directory");
                                //            }
                                //        }
                                //    }
                                //    catch (Exception ex)
                                //    {
                                //        errs.New($"cp: cannot copy: {ex.Message}");
                                //    }
                                //}

                                // !grep - Search file contents
                                if (parsed_commands[0] == "!grep")
                                {
                                    if (parsed_commands.Count < 3)
                                    {
                                        errs.New("grep: missing operands");
                                        return;
                                    }

                                    try
                                    {
                                        string pattern = parsed_commands[1];
                                        var files = parsed_commands.Skip(2).ToList();
                                        bool caseInsensitive = files.Contains("-i");
                                        if (caseInsensitive) files.Remove("-i");

                                        foreach (var file in files)
                                        {
                                            var lines = File.ReadAllLines(file);
                                            for (int i = 0; i < lines.Length; i++)
                                            {
                                                bool match = caseInsensitive
                                                    ? lines[i].IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0
                                                    : lines[i].Contains(pattern);

                                                if (match)
                                                {
                                                    Console.WriteLine($"{file}:{i + 1}: {lines[i]}");
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        errs.New($"grep: {ex.Message}");
                                    }
                                }

                                // !wc - Word, line, and character count
                                if (parsed_commands[0] == "!wc")
                                {
                                    if (parsed_commands.Count < 2)
                                    {
                                        errs.New("wc: missing file operand");
                                        return;
                                    }

                                    try
                                    {
                                        bool lineCount = parsed_commands.Contains("-l");
                                        bool wordCount = parsed_commands.Contains("-w");
                                        bool charCount = parsed_commands.Contains("-c");

                                        // Default to all if no flags specified
                                        if (!lineCount && !wordCount && !charCount)
                                        {
                                            lineCount = wordCount = charCount = true;
                                        }

                                        foreach (var file in parsed_commands.Skip(1).Where(f => !f.StartsWith("-")))
                                        {
                                            string content = File.ReadAllText(file);
                                            int lines = content.Split(new[] { '\n' }).Length;
                                            int words = content.Split(new[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
                                            int chars = content.Length;

                                            Console.Write($"{file}: ");
                                            if (lineCount) Console.Write($"Lines: {lines} ");
                                            if (wordCount) Console.Write($"Words: {words} ");
                                            if (charCount) Console.Write($"Chars: {chars}");
                                            Console.WriteLine();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        errs.New($"wc: {ex.Message}");
                                    }
                                }

                                // !head - Display first lines of file
                                if (parsed_commands[0] == "!head")
                                {
                                    int lineCount = 10;
                                    List<string> files = new List<string>();

                                    for (int i = 1; i < parsed_commands.Count; i++)
                                    {
                                        if (parsed_commands[i] == "-n" && i + 1 < parsed_commands.Count)
                                        {
                                            if (int.TryParse(parsed_commands[i + 1], out int count))
                                            {
                                                lineCount = count;
                                                i++; // Skip next argument
                                            }
                                        }
                                        else
                                        {
                                            files.Add(parsed_commands[i]);
                                        }
                                    }

                                    try
                                    {
                                        foreach (var file in files)
                                        {
                                            var lines = File.ReadAllLines(file);
                                            if (files.Count > 1) Console.WriteLine($"==> {file} <==");
                                            for (int i = 0; i < Math.Min(lineCount, lines.Length); i++)
                                            {
                                                Console.WriteLine(lines[i]);
                                            }
                                            Console.WriteLine();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        errs.New($"head: {ex.Message}");
                                    }
                                }

                                // !tail - Display last lines of file
                                if (parsed_commands[0] == "!tail")
                                {
                                    int lineCount = 10;
                                    List<string> files = new List<string>();

                                    for (int i = 1; i < parsed_commands.Count; i++)
                                    {
                                        if (parsed_commands[i] == "-n" && i + 1 < parsed_commands.Count)
                                        {
                                            if (int.TryParse(parsed_commands[i + 1], out int count))
                                            {
                                                lineCount = count;
                                                i++; // Skip next argument
                                            }
                                        }
                                        else
                                        {
                                            files.Add(parsed_commands[i]);
                                        }
                                    }

                                    try
                                    {
                                        foreach (var file in files)
                                        {
                                            var lines = File.ReadAllLines(file);
                                            if (files.Count > 1) Console.WriteLine($"==> {file} <==");
                                            for (int i = Math.Max(0, lines.Length - lineCount); i < lines.Length; i++)
                                            {
                                                Console.WriteLine(lines[i]);
                                            }
                                            Console.WriteLine();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        errs.New($"tail: {ex.Message}");
                                    }
                                }

                                // !echo - Print arguments
                                if (parsed_commands[0] == "!echo")
                                {
                                    bool suppressNewline = parsed_commands.Contains("-n");
                                    var messageParts = parsed_commands.Skip(1)
                                        .Where(x => x != "-n")
                                        .ToList();

                                    if (suppressNewline)
                                    {
                                        Console.Write(string.Join(" ", messageParts));
                                    }
                                    else
                                    {
                                        Console.WriteLine(string.Join(" ", messageParts));
                                    }
                                }

                                // !which - Locate executable
                                if (parsed_commands[0] == "!which")
                                {
                                    if (parsed_commands.Count < 2)
                                    {
                                        errs.New("which: missing operand");
                                        return;
                                    }

                                    foreach (var command in parsed_commands.Skip(1))
                                    {
                                        string[] paths = Environment.GetEnvironmentVariable("PATH")
                                            .Split(Path.PathSeparator);

                                        bool found = false;
                                        foreach (var path in paths)
                                        {
                                            string fullPath = Path.Combine(path, command);
                                            if (File.Exists(fullPath) || File.Exists(fullPath + ".exe"))
                                            {
                                                Console.WriteLine(fullPath);
                                                found = true;
                                                break;
                                            }
                                        }

                                        if (!found)
                                        {
                                            errs.New($"which: {command} not found");
                                        }
                                    }
                                }

                                // !du - Disk usage
                                if (parsed_commands[0] == "!du")
                                {
                                    string path = ".";
                                    bool humanReadable = parsed_commands.Contains("-h");
                                    bool total = parsed_commands.Contains("-s");

                                    var dirs = parsed_commands.Where(x => !x.StartsWith("-")).ToList();
                                    if (dirs.Count > 0) path = dirs[0];

                                    try
                                    {
                                        long totalSize = CalculateDirSize(new DirectoryInfo(path));

                                        if (total)
                                        {
                                            Console.WriteLine(FormatSize(totalSize, humanReadable));
                                        }
                                        else
                                        {
                                            foreach (var dir in Directory.GetDirectories(path))
                                            {
                                                long size = CalculateDirSize(new DirectoryInfo(dir));
                                                Console.WriteLine($"{FormatSize(size, humanReadable)}\t{dir}");
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        errs.New($"du: {ex.Message}");
                                    }
                                }

                                

                                // !find - Find files
                                if (parsed_commands[0] == "!find")
                                {
                                    if (parsed_commands.Count < 2)
                                    {
                                        errs.New("find: missing starting path");
                                        return;
                                    }

                                    string startPath = ".";
                                    string name = null;
                                    bool recursive = true;

                                    for (int i = 1; i < parsed_commands.Count; i++)
                                    {
                                        if (parsed_commands[i] == "-name")
                                        {
                                            if (i + 1 < parsed_commands.Count)
                                                name = parsed_commands[++i];
                                        }
                                        else if (parsed_commands[i] == "-maxdepth")
                                        {
                                            if (i + 1 < parsed_commands.Count)
                                            {
                                                if (int.TryParse(parsed_commands[++i], out int depth))
                                                    recursive = depth > 0;
                                            }
                                        }
                                        else
                                        {
                                            startPath = parsed_commands[i];
                                        }
                                    }

                                    try
                                    {
                                        FindFiles(startPath, name, recursive);
                                    }
                                    catch (Exception ex)
                                    {
                                        errs.New($"find: {ex.Message}");
                                    }
                                }

                                

                                // Fallback for unrecognized commands
                                else
                                {
                                    errs.CacheClean();
                                    errs.New($"sh: what is this: `{parsed_commands[0]}`, that is not a macro.");
                                    errs.ListThemAll();
                                    errs.CacheClean();
                                }
                            }
                            else
                            {

                                if (__shell__.ToLower() == "hsh")
                                {
                                    PleaseCommandEnv.TheseCommands(parsed_commands);
                                }
                                else if (__shell__.ToLower() == "hqsh")
                                {
                                    PleaseCommandEnv.TheseCommands(parsed_commands);
                                }
                                else
                                {
                                    errs.CacheClean();
                                    errs.New($"`{__shell__}`: is not a well-known shell, by me. turning to default `hqsh` shell. Type: `!csh --list` for the list of builtin shells.");
                                    errs.ListThemAll();
                                    errs.CacheClean();
                                }

                            }

                            IdentifyCommand.CacheClean();
                        }
                        else
                        { 
                            isctrlc = false;
                        }

                        continue;

                    }
                    catch (Exception exp) // Exception handling block
                    {
                        errs.CacheClean();
                        errs.New(exp.ToString());
                        errs.ListThem();
                    }

                    // Handle CTRL+C key press to prevent quitting
                    Console.CancelKeyPress += (sender, e) =>
                    {
                        e.Cancel = false; // Prevent the app from closing
                        //Console.WriteLine("\nTolerating CTRL+C!");
                    };
                }
            }
            catch (Exception exx)
            {
                errs.New(exx.ToString());
                errs.ListThem();
                errs.CacheClean();
            }
        }
        #endregion
    }
}