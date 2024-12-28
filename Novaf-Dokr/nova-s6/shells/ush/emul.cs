using nova_s6.Command;
using nova_s6.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace nova_s6.shells.ush
{
    public class emul
    {
        public static string UshShellVersion = "24/12/2024";

        private static Dictionary<string, string> macros = new Dictionary<string, string>();
        private static string okayMacrosPath = Path.Combine(CommandEnv.UserHomeDir, "vin_env\\third_party\\nova\\ush\\macros\\okay_macros.json"); //Path.Combine(AppContext.BaseDirectory, "okay_macros.json");
        private static string notOkayMacrosPath = Path.Combine(CommandEnv.UserHomeDir, "vin_env\\third_party\\nova\\ush\\macros\\not_okay_macros.json");  //Path.Combine(AppContext.BaseDirectory, "not_okay_macros.json");

        public static void EnsureMacrosExist()
        {
            try
            {
                // Ensure the directory for macros exists
                string macrosDirectory = Path.GetDirectoryName(okayMacrosPath);
                if (!Directory.Exists(macrosDirectory))
                {
                    Directory.CreateDirectory(macrosDirectory);
                    Console.WriteLine($"Directory created: {macrosDirectory}");
                }

                // Check and create the "okay_macros.json" file if not present
                if (!File.Exists(okayMacrosPath))
                {
                    File.WriteAllText(okayMacrosPath, "{}"); // Empty JSON object
                    Console.WriteLine($"File created: {okayMacrosPath}");
                }

                // Check and create the "not_okay_macros.json" file if not present
                if (!File.Exists(notOkayMacrosPath))
                {
                    File.WriteAllText(notOkayMacrosPath, "{}"); // Empty JSON object
                    Console.WriteLine($"File created: {notOkayMacrosPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ensuring macro files exist: {ex.Message}");
            }
        }

        public static int ate(List<string> pcS)
        {
            for (int i = 0; i < pcS.Count; i++)
            {
                if (macros.ContainsKey(pcS[i]))
                {
                    // Split the macro's value into a list and replace the current input with it
                    var expandedMacro = macros[pcS[i]].Split(" ").ToList();

                    // Remove the current macro key from pcS
                    pcS.RemoveAt(i);

                    // Insert the expanded macro at the same index
                    pcS.InsertRange(i, expandedMacro);

                    // Adjust the loop index to skip over the inserted elements
                    i += expandedMacro.Count - 1;
                }
            }

            #region pre-processing
            if (pcS == null)
                return 0;

            if (pcS.Count <= 0)
                return 0;

            // the main stuff

            CommandEnv.CommandHistory.Add(pcS.ToString());
            var parts = pcS;

            // Check if it is a get env arg value ` $(var_name) `
            //// List<string> parts = new List<string>();
            //// parts = ["@bin", "ls", "--path", "$(BinPath)"];
            for (int i = 0; i < parts.Count; i++)
            {
                parts[i] = CommandEnv.ReplaceEnvironmentVariables(parts[i]);
            }

            for (int i = 0; i < parts.Count; i++)
            {
                if (parts[i].StartsWith("\\"))
                {
                    parts[i] = parts[i].Replace("\\n", "\n");
                    parts[i] = parts[i].Replace("\\t", "\t");
                }
            }

            pcS = parts;
            #endregion


            if (pcS[0].ToLower() == "echo")
            {
                for (int i = 1; i < pcS.Count; i++)
                {
                    Console.Write($"{pcS[i]} ");
                }
            }
            else if (pcS[0].ToLower() == "echoln")
            {
                for (int i = 1; i < pcS.Count; i++)
                {
                    Console.WriteLine(pcS[i]);
                }
            }

            else if (pcS[0].ToLower() == "$hsh")
            {
                List<string> Commands = [];

                for (int i = 1; i < pcS.Count; i++)
                {
                    Commands.Add(pcS[i]);
                }

                PleaseCommandEnv.TheseCommands(Commands);
            }
            else if (pcS[0].ToLower() == "$hqsh")
            {
                List<string> Commands = [];

                for (int i = 1; i < pcS.Count; i++)
                {
                    Commands.Add(pcS[i]);
                }

                nova_s6.shells.hqsh.emul.ate(Commands);
            }

            else if (pcS[0].ToLower() == "help")
            {
                List<string> help_lines = [
                    "Builtin:",
                    " echo     ==> echo stuff ==> to echo stuff.",
                    " echoln   ==> to echo something on seperate lines.",
                    " help     ==> to show this basic help message.",
                    "More:",
                    " hsh:",
                    "  $hsh <cmmand>   // Executes the hsh shell commands.",
                    " hqsh:",
                    "  $hqsh <command> // Executes the hqsh shell commands."
                    ];

                foreach (var line in help_lines)
                {
                    Console.WriteLine(line);
                }
            }
            else if (pcS[0].ToLower() == "exit")
            {
                Environment.Exit(0);
            }
            else
            {
                ProcessBinCommand(pcS);

                //errs.CacheClean();
                //errs.New($"ush: `{pcS[0]}`: something went wrong!, type `help` for help!");
                //errs.ListThemAll();
                //errs.CacheClean();
                return 0;
            }

            // return the result code

            return 1;
        }

        public static void ProcessBinCommand(List<string> parts)
        {
            if (parts[0] == "@bin")
            {
                if (parts.Count == 1 && parts[0] == "@bin")
                {
                    //errs.New($"Usage: @bin <command> - Display file contents in binary format");
                    //errs.ListThem();
                    //errs.CacheClean();
                    return;
                }

                string executable = parts.Count > 1 ? parts[1] : parts[0];
                string[] args = parts.Skip(2).ToArray();

                string filePath = "";

                if (executable.StartsWith("./") || executable.StartsWith(".\\"))
                    if (executable.EndsWith(".exe"))
                        filePath = $"{Environment.CurrentDirectory}\\{executable}";
                    else
                        filePath = $"{Environment.CurrentDirectory}\\{executable}.exe";
                else
                    filePath = $"C:\\Users\\{Environment.UserName}\\vin_env\\bin\\{executable}\\{executable}.exe";

                try
                {
                    using (Process process = Process.Start(filePath, string.Join(" ", args)))
                    {
                        process.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    errs.New($"e-s-p: Something wrong with `{filePath}`.");
                    //errs.ListThem();
                    //errs.CacheClean();
                }
            }
            else
            {
                string executable = parts.Count > 1 ? parts[0] : parts[0];
                string[] args = parts.Skip(1).ToArray();

                string filePath = "";

                if (executable.StartsWith("./") || executable.StartsWith(".\\"))
                    if (executable.EndsWith(".exe"))
                        filePath = $"{Environment.CurrentDirectory}\\{executable}";
                    else
                        filePath = $"{Environment.CurrentDirectory}\\{executable}.exe";
                else
                    filePath = $"C:\\Users\\{Environment.UserName}\\vin_env\\bin\\{executable}\\{executable}.exe";

                try
                {
                    using (Process process = Process.Start(filePath, string.Join(" ", args)))
                    {
                        process.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    errs.New($"e-s-p: Something wrong with `{filePath}`.");
                    //errs.ListThem();
                    //errs.CacheClean();
                }
            }
        }
    }
}
