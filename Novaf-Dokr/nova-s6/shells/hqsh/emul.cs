using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.IO;

using nova_s6.Utils;
using nova_s6.Command;

using System.Diagnostics;
using System.ComponentModel.Design;
using System.Text.RegularExpressions;
using Novaf_Dokr.Command.env.user;


using System.Net.Sockets;
using System.Net.FtpClient;

using System;
using System.Net;
using System.Net.NetworkInformation;
using System.IO;
using System.Threading;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates; // For X509Certificate2
using MimeKit; // For creating email message in SmtpClient context


namespace nova_s6.shells.hqsh
{
    public class emul
    {
        private static Dictionary<string, string> macros = new Dictionary<string, string>();
        private static string okayMacrosPath = Path.Combine(CommandEnv.UserHomeDir, "vin_env\\third_party\\nova\\hqsh\\macros\\okay_macros.json"); //Path.Combine(AppContext.BaseDirectory, "okay_macros.json");
        private static string notOkayMacrosPath = Path.Combine(CommandEnv.UserHomeDir, "vin_env\\third_party\\nova\\hqsh\\macros\\not_okay_macros.json");  //Path.Combine(AppContext.BaseDirectory, "not_okay_macros.json");

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

            else if (pcS[0].ToLower() == "mind")
            {
                if (pcS.Count < 2)
                    return -1;

                //if (pcS[1].ToLower() == "-s")
                //{
                //    List<string> DataPoints = new List<string>();
                //    for (int i = 2; i < pcS.Count; i++)
                //    {
                //        DataPoints.Add(pcS[i]);
                //    }
                //    mind.attr.store(DataPoints);
                //}
                else if (pcS[1].ToLower() == "-add")
                {
                    List<string> DataPoints = new List<string>();
                    for (int i = 2; i < pcS.Count; i++)
                    {
                        DataPoints.Add(pcS[i]);
                    }
                    mind.attr.storeAdd(DataPoints);
                }
                else if (pcS[1].ToLower() == "-get")
                {
                    Console.WriteLine(mind.attr.get(int.Parse(pcS[2])));
                }
                else if (pcS[1].ToLower() == "-list")
                {
                    mind.attr.show();
                }
                else
                {
                    //errs.CacheClean();
                    errs.New($"`{pcS[1]}`: is not a well-known command, valid commands are: `-add <item>` , `-get <int>` , `-list`");
                    //errs.ListThemAll();
                    //errs.CacheClean();
                }
            }
            else if (pcS[0].ToLower().StartsWith("reg"))
            {
                List<string> Params = pcS[0].Split(">>").ToList();



                if (Params[1].ToLower() == "set")
                {
                    if (Params.Count < 4)
                        return -1;

                    shells.hqsh.Register.rtta.RegisterStorage[Params[2].ToString()] = Params[3];
                }
                else if (Params[1].ToLower() == "get")
                {
                    if (Params.Count < 3)
                        return -1;

                    if (shells.hqsh.Register.rtta.RegisterStorage.ContainsKey(Params[2].ToString()))
                        Console.WriteLine(shells.hqsh.Register.rtta.RegisterStorage[Params[2].ToString()]);
                }
                else if (Params[1].ToLower() == "list")
                {
                    foreach (var (k, v) in shells.hqsh.Register.rtta.RegisterStorage)
                    {
                        Console.WriteLine($"{k,-10} ==== {v,-20}");
                    }
                }
                else
                {
                    //errs.CacheClean();
                    errs.New($"`{Params[1]}`: is not a well-known command, valid commands are: `>>set>>(item)>>(val)` , `>>get>>(item)` , `>>list`");
                    //errs.ListThemAll();
                    //errs.CacheClean();
                }
            }



            else if (pcS[0].ToLower().StartsWith("$$"))
            {
                if (pcS[0].ToLower().StartsWith("$$env"))
                {
                    if (pcS[0].ToLower().StartsWith("$$env::"))
                    {
                        List<string> after_commands = pcS[0].Split("::").ToList();

                        for (int i = 1; i < after_commands.Count; i++)
                        {
                            if (after_commands[i] != null)
                            {
                                novaOutput.result(CommandEnv.EnvironmentVariables[after_commands[i]]);
                            }
                        }
                    }
                    else if (pcS[0].ToLower().StartsWith("$$env>>"))
                    {
                        List<string> after_commands = pcS[0].Split(">>").ToList();

                        if (after_commands[1].ToLower() == "$all")
                        {
                            if (CommandEnv.EnvironmentVariables.Count == 0)
                            {
                                errs.New("No environment variables set.");
                                errs.ListThem();
                                //errs.CacheClean();
                            }
                            else
                            {
                                foreach (var kvp in CommandEnv.EnvironmentVariables)
                                {
                                    Console.WriteLine($" {kvp.Key,-11} =>> {kvp.Value}");
                                }
                            }
                            return 0;
                        }
                        else if (after_commands[1].ToLower() == "$rem")
                        {
                            try
                            {
                                CommandEnv.EnvironmentVariables.Remove(after_commands[2]);
                            }
                            catch (Exception)
                            {
                                //errs.CacheClean();
                                errs.New($"hqsh: something went wrong!");
                                //errs.ListThemAll();
                                //errs.CacheClean();
                            }
                            return 0;
                        }

                        if (after_commands.Count >= 2)
                        {
                            try
                            {
                                if (after_commands.Count >= 2)
                                {
                                    CommandEnv.EnvironmentVariables[after_commands[1]] = string.Join(" ", after_commands.Skip(2));
                                    CommandEnv.SaveEnvironmentVariables();
                                    //Console.WriteLine($"Environment variable '{parts[2]}' set.");
                                    novaOutput.result($"Environment variable '{after_commands[1]}' set to `{string.Join(" ", after_commands.Skip(2))}`.");
                                }
                            }
                            catch
                            {
                                //errs.CacheClean();
                                errs.New($"hqsh: something went wrong!");
                                //errs.ListThemAll();
                                //errs.CacheClean();

                            }
                        }
                        else
                        {
                            string name = after_commands[1].ToString();
                            novaOutput.result(CommandEnv.EnvironmentVariables[name]);
                        }
                    }
                    else
                    {
                        //errs.CacheClean();
                        errs.New($"hqsh: `{pcS[0]}`: something went wrong!");
                        //errs.ListThemAll();
                        //errs.CacheClean();
                    }
                }
            }
            else if (pcS[0].ToLower() == "macro")
            {
                LoadMacros();
                if (pcS.Count < 2)
                {
                    Console.WriteLine("Usage: macro [add|remove|update|list|restore] [alias] [command]");
                    return -1;
                }

                switch (pcS[1].ToLower())
                {
                    case "add":
                        if (pcS.Count < 4)
                        {
                            Console.WriteLine("Usage: macro add <alias> <command>");
                            return -1;
                        }
                        string newCommand = string.Join(" ", pcS.Skip(3));
                        if (!macros.ContainsKey(pcS[2].ToLower()))
                        {
                            macros.Add(pcS[2].ToLower(), newCommand);
                            SaveMacros(okayMacrosPath);
                            Console.WriteLine($"Macro '{pcS[2]}' added successfully.");
                        }
                        else
                        {
                            Console.WriteLine($"Macro '{pcS[2]}' already exists. Use 'macro update' to modify it.");
                        }
                        break;

                    case "remove":
                        if (pcS.Count < 3)
                        {
                            Console.WriteLine("Usage: macro remove <alias>");
                            return -1;
                        }
                        if (macros.ContainsKey(pcS[2].ToLower()))
                        {
                            // Save to not_okay.json before removing
                            var removedMacros = new Dictionary<string, string>
                {
                    { pcS[2].ToLower(), macros[pcS[2].ToLower()] }
                };
                            string json = JsonSerializer.Serialize(removedMacros, new JsonSerializerOptions { WriteIndented = true });
                            File.AppendAllText(notOkayMacrosPath, json);

                            macros.Remove(pcS[2].ToLower());
                            SaveMacros(okayMacrosPath);
                            Console.WriteLine($"Macro '{pcS[2]}' removed successfully.");
                        }
                        else
                        {
                            Console.WriteLine($"Macro '{pcS[2]}' not found.");
                        }
                        break;

                    case "update":
                        if (pcS.Count < 4)
                        {
                            Console.WriteLine("Usage: macro update <alias> <new_command>");
                            return -1;
                        }
                        string updatedCommand = string.Join(" ", pcS.Skip(3));
                        if (macros.ContainsKey(pcS[2].ToLower()))
                        {
                            macros[pcS[2].ToLower()] = updatedCommand;
                            SaveMacros(okayMacrosPath);
                            Console.WriteLine($"Macro '{pcS[2]}' updated successfully.");
                        }
                        else
                        {
                            Console.WriteLine($"Macro '{pcS[2]}' not found.");
                        }
                        break;

                    case "list":
                        if (macros.Count == 0)
                        {
                            Console.WriteLine("No macros defined.");
                        }
                        else
                        {
                            Console.WriteLine("Defined macros:");
                            foreach (var macro in macros)
                            {
                                Console.WriteLine($"{macro.Key} => {macro.Value}");
                            }
                        }
                        break;

                    default:
                        Console.WriteLine("Unknown macro command. Available commands: add, remove, update, list");
                        break;
                }
                return 1;
            }



            else if (pcS[0].ToLower() == "file")
            {
                switch (pcS[1].ToLower())
                {
                    case "create":
                        try
                        {
                            File.WriteAllText(pcS[2], string.Join(" ", pcS.Skip(3)));
                            Console.WriteLine($"File '{pcS[2]}' created successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error creating file: {ex.Message}");
                        }
                        break;

                    case "read":
                        try
                        {
                            string content = File.ReadAllText(pcS[2]);
                            Console.WriteLine(content);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error reading file: {ex.Message}");
                        }
                        break;

                    case "delete":
                        try
                        {
                            File.Delete(pcS[2]);
                            Console.WriteLine($"File '{pcS[2]}' deleted successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deleting file: {ex.Message}");
                        }
                        break;

                    case "append":
                        try
                        {
                            File.AppendAllText(pcS[2], string.Join(" ", pcS.Skip(3)) + Environment.NewLine);
                            Console.WriteLine($"Data appended to file '{pcS[2]}'.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error appending to file: {ex.Message}");
                        }
                        break;

                    case "rename":
                        try
                        {
                            File.Move(pcS[2], pcS[3]);
                            Console.WriteLine($"File '{pcS[2]}' renamed to '{pcS[3]}'.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error renaming file: {ex.Message}");
                        }
                        break;

                    case "copy":
                        try
                        {
                            File.Copy(pcS[2], pcS[3], overwrite: true);
                            Console.WriteLine($"File '{pcS[2]}' copied to '{pcS[3]}'.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error copying file: {ex.Message}");
                        }
                        break;

                    case "info":
                        try
                        {
                            var fileInfo = new FileInfo(pcS[2]);
                            Console.WriteLine($"File Info: \nName: {fileInfo.Name}\nSize: {fileInfo.Length} bytes\nCreated: {fileInfo.CreationTime}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error fetching file info: {ex.Message}");
                        }
                        break;

                    case "exists":
                        Console.WriteLine(File.Exists(pcS[2]) ? $"File '{pcS[2]}' exists." : $"File '{pcS[2]}' does not exist.");
                        break;

                    case "lines-count":
                        try
                        {
                            int lineCount = File.ReadLines(pcS[2]).Count();
                            Console.WriteLine($"File '{pcS[2]}' has {lineCount} lines.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error counting lines: {ex.Message}");
                        }
                        break;

                    case "search":
                        try
                        {
                            string content = File.ReadAllText(pcS[2]);
                            if (content.Contains(pcS[3]))
                            {
                                Console.WriteLine($"'{pcS[3]}' found in '{pcS[2]}'.");
                            }
                            else
                            {
                                Console.WriteLine($"'{pcS[3]}' not found in '{pcS[2]}'.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error searching in file: {ex.Message}");
                        }
                        break;

                    case "encrypt":
                        try
                        {
                            byte[] content = File.ReadAllBytes(pcS[2]);
                            byte[] key = GenerateRandomKey();
                            byte[] encrypted = EncryptData(content, key);

                            // Save the encrypted data
                            File.WriteAllBytes(pcS[2] + ".encrypted", encrypted);

                            // Save the key in a separate file
                            File.WriteAllBytes(pcS[2] + ".key", key);

                            Console.WriteLine($"File '{pcS[2]}' encrypted successfully.");
                            Console.WriteLine($"Key saved to '{pcS[2]}.key' - Keep this safe!");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error encrypting file: {ex.Message}");
                        }
                        break;

                    case "decrypt":
                        try
                        {
                            string keyPath = pcS[2] + ".key";
                            if (!File.Exists(keyPath))
                            {
                                Console.WriteLine("Error: Key file not found. Please provide the key file path.");
                                break;
                            }

                            byte[] key = File.ReadAllBytes(keyPath);
                            byte[] encrypted = File.ReadAllBytes(pcS[2]);
                            byte[] decrypted = DecryptData(encrypted, key);

                            string outputPath = pcS[2].EndsWith(".encrypted") ?
                                pcS[2].Substring(0, pcS[2].Length - 10) : pcS[2] + ".decrypted";

                            File.WriteAllBytes(outputPath, decrypted);
                            Console.WriteLine($"File decrypted to '{outputPath}'.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error decrypting file: {ex.Message}");
                        }
                        break;
                    case "compress":
                        try
                        {
                            using (FileStream originalFile = File.OpenRead(pcS[2]))
                            using (FileStream compressedFile = File.Create(pcS[2] + ".gz"))
                            using (System.IO.Compression.GZipStream compress = new System.IO.Compression.GZipStream(compressedFile, System.IO.Compression.CompressionMode.Compress))
                            {
                                originalFile.CopyTo(compress);
                            }
                            Console.WriteLine($"File '{pcS[2]}' compressed successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error compressing file: {ex.Message}");
                        }
                        break;

                    case "decompress":
                        try
                        {
                            string outputFile = pcS[2].EndsWith(".gz") ?
                                pcS[2].Substring(0, pcS[2].Length - 3) : pcS[2] + ".decompressed";
                            using (FileStream compressedFile = File.OpenRead(pcS[2]))
                            using (FileStream outputStream = File.Create(outputFile))
                            using (System.IO.Compression.GZipStream decompress = new System.IO.Compression.GZipStream(compressedFile, System.IO.Compression.CompressionMode.Decompress))
                            {
                                decompress.CopyTo(outputStream);
                            }
                            Console.WriteLine($"File decompressed to '{outputFile}'.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error decompressing file: {ex.Message}");
                        }
                        break;

                    case "hash":
                        try
                        {
                            using (var sha256 = SHA256.Create())
                            {
                                byte[] fileBytes = File.ReadAllBytes(pcS[2]);
                                byte[] hashBytes = sha256.ComputeHash(fileBytes);
                                string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                                Console.WriteLine($"SHA-256 hash of '{pcS[2]}': {hash}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error computing file hash: {ex.Message}");
                        }
                        break;

                    case "watch":
                        try
                        {
                            using var watcher = new FileSystemWatcher(Path.GetDirectoryName(pcS[2]), Path.GetFileName(pcS[2]));
                            watcher.NotifyFilter = NotifyFilters.LastWrite;
                            watcher.Changed += (s, e) => Console.WriteLine($"File changed at: {DateTime.Now}");
                            watcher.EnableRaisingEvents = true;
                            Console.WriteLine($"Watching '{pcS[2]}' for changes. Press any key to stop.");
                            Console.ReadKey();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error watching file: {ex.Message}");
                        }
                        break;

                    case "temp":
                        try
                        {
                            string tempFile = Path.GetTempFileName();
                            if (pcS.Count > 2)
                            {
                                File.WriteAllText(tempFile, string.Join(" ", pcS.Skip(2)));
                            }
                            Console.WriteLine($"Temporary file created: {tempFile}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error creating temp file: {ex.Message}");
                        }
                        break;

                    // Add 40 more commands such as encrypt, decrypt, create temp, clear content, etc.
                    default:
                        Console.WriteLine($"Unknown file command: '{pcS[1]}'.");
                        break;
                }
            }
            else if (pcS[0].ToLower() == "dir")
            {
                switch (pcS[1].ToLower())
                {
                    case "create":
                        try
                        {
                            Directory.CreateDirectory(pcS[2]);
                            Console.WriteLine($"Directory '{pcS[2]}' created successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error creating directory: {ex.Message}");
                        }
                        break;

                    case "list":
                        try
                        {
                            var entries = Directory.GetFileSystemEntries(pcS[2]);
                            foreach (var entry in entries)
                            {
                                Console.WriteLine(entry);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error listing directory: {ex.Message}");
                        }
                        break;

                    case "delete":
                        try
                        {
                            Directory.Delete(pcS[2], true);
                            Console.WriteLine($"Directory '{pcS[2]}' deleted successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deleting directory: {ex.Message}");
                        }
                        break;

                    case "rename":
                        try
                        {
                            Directory.Move(pcS[2], pcS[3]);
                            Console.WriteLine($"Directory '{pcS[2]}' renamed to '{pcS[3]}'.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error renaming directory: {ex.Message}");
                        }
                        break;

                    case "exists":
                        Console.WriteLine(Directory.Exists(pcS[2]) ? $"Directory '{pcS[2]}' exists." : $"Directory '{pcS[2]}' does not exist.");
                        break;

                    case "info":
                        try
                        {
                            var dirInfo = new DirectoryInfo(pcS[2]);
                            Console.WriteLine($"Directory Info: \nName: {dirInfo.Name}\nCreated: {dirInfo.CreationTime}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error fetching directory info: {ex.Message}");
                        }
                        break;

                    case "size":
                        try
                        {
                            long dirSize = Directory.GetFiles(pcS[2], "*", SearchOption.AllDirectories).Sum(f => new FileInfo(f).Length);
                            Console.WriteLine($"Directory '{pcS[2]}' size: {dirSize} bytes.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error calculating directory size: {ex.Message}");
                        }
                        break;

                    case "count-files":
                        try
                        {
                            int fileCount = Directory.GetFiles(pcS[2]).Length;
                            Console.WriteLine($"Directory '{pcS[2]}' contains {fileCount} files.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error counting files: {ex.Message}");
                        }
                        break;

                    case "count-dirs":
                        try
                        {
                            int dirCount = Directory.GetDirectories(pcS[2]).Length;
                            Console.WriteLine($"Directory '{pcS[2]}' contains {dirCount} subdirectories.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error counting directories: {ex.Message}");
                        }
                        break;

                    case "backup":
                        try
                        {
                            string backupName = $"{pcS[2]}_backup_{DateTime.Now:yyyyMMdd_HHmmss}";
                            Directory.CreateDirectory(backupName);
                            foreach (string dirPath in Directory.GetDirectories(pcS[2], "*", SearchOption.AllDirectories))
                            {
                                Directory.CreateDirectory(dirPath.Replace(pcS[2], backupName));
                            }
                            foreach (string filePath in Directory.GetFiles(pcS[2], "*.*", SearchOption.AllDirectories))
                            {
                                File.Copy(filePath, filePath.Replace(pcS[2], backupName), true);
                            }
                            Console.WriteLine($"Directory backed up to '{backupName}'.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error backing up directory: {ex.Message}");
                        }
                        break;

                    case "clean":
                        try
                        {
                            foreach (string file in Directory.GetFiles(pcS[2]))
                            {
                                File.Delete(file);
                            }
                            foreach (string dir in Directory.GetDirectories(pcS[2]))
                            {
                                Directory.Delete(dir, true);
                            }
                            Console.WriteLine($"Directory '{pcS[2]}' cleaned successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error cleaning directory: {ex.Message}");
                        }
                        break;

                    case "find":
                        try
                        {
                            var pattern = pcS[3];
                            var files = Directory.GetFiles(pcS[2], pattern, SearchOption.AllDirectories);
                            Console.WriteLine($"Found {files.Length} files matching pattern '{pattern}':");
                            foreach (var file in files)
                            {
                                Console.WriteLine(file);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error searching directory: {ex.Message}");
                        }
                        break;

                    case "monitor":
                        try
                        {
                            using var watcher = new FileSystemWatcher(pcS[2]);
                            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                            watcher.Created += (s, e) => Console.WriteLine($"Created: {e.FullPath}");
                            watcher.Deleted += (s, e) => Console.WriteLine($"Deleted: {e.FullPath}");
                            watcher.Changed += (s, e) => Console.WriteLine($"Changed: {e.FullPath}");
                            watcher.Renamed += (s, e) => Console.WriteLine($"Renamed: {e.OldFullPath} to {e.FullPath}");
                            watcher.EnableRaisingEvents = true;
                            Console.WriteLine($"Monitoring directory '{pcS[2]}'. Press any key to stop.");
                            Console.ReadKey();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error monitoring directory: {ex.Message}");
                        }
                        break;

                    // Add 40 more commands such as compress, decompress, create temp dir, clear contents, etc.
                    default:
                        Console.WriteLine($"Unknown directory command: '{pcS[1]}'.");
                        break;
                }
            }

            else if (pcS[0].ToLower() == "net")
            {
                switch (pcS[1].ToLower())
                {
                    case "ping":
                        try
                        {
                            using (Ping ping = new Ping())
                            {
                                PingReply reply = ping.Send(pcS[2]);
                                Console.WriteLine($"Reply from {pcS[2]}: time={reply.RoundtripTime}ms status={reply.Status}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error pinging host: {ex.Message}");
                        }
                        break;

                    case "gethost":
                        try
                        {
                            IPHostEntry hostEntry = Dns.GetHostEntry(pcS[2]);
                            Console.WriteLine($"Host name: {hostEntry.HostName}");
                            Console.WriteLine("IP Addresses:");
                            foreach (IPAddress ip in hostEntry.AddressList)
                            {
                                Console.WriteLine(ip);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error getting host info: {ex.Message}");
                        }
                        break;

                    case "getip":
                        try
                        {
                            IPHostEntry hostEntry = Dns.GetHostEntry(pcS[2]);
                            Console.WriteLine($"IP Addresses for {pcS[2]}:");
                            foreach (IPAddress ip in hostEntry.AddressList)
                            {
                                Console.WriteLine(ip);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error resolving IP: {ex.Message}");
                        }
                        break;

                    case "scan-ports":
                        try
                        {
                            int startPort = int.Parse(pcS[3]);
                            int endPort = int.Parse(pcS[4]);
                            Console.WriteLine($"Scanning ports {startPort}-{endPort} on {pcS[2]}...");

                            for (int port = startPort; port <= endPort; port++)
                            {
                                using (TcpClient client = new TcpClient())
                                {
                                    try
                                    {
                                        client.Connect(pcS[2], port);
                                        Console.WriteLine($"Port {port} is open");
                                    }
                                    catch
                                    {
                                        // Port is closed or filtered
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error scanning ports: {ex.Message}");
                        }
                        break;

                    case "whois":
                        try
                        {
                            using (TcpClient client = new TcpClient("whois.internic.net", 43))
                            using (NetworkStream stream = client.GetStream())
                            using (StreamWriter writer = new StreamWriter(stream))
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                writer.WriteLine(pcS[2]);
                                writer.Flush();
                                Console.WriteLine(reader.ReadToEnd());
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error performing WHOIS lookup: {ex.Message}");
                        }
                        break;

                    case "download":
                        try
                        {
                            using (WebClient client = new WebClient())
                            {
                                client.DownloadFile(pcS[2], pcS[3]);
                                Console.WriteLine($"File downloaded successfully to {pcS[3]}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error downloading file: {ex.Message}");
                        }
                        break;

                    case "traceroute":
                        try
                        {
                            using (Ping ping = new Ping())
                            {
                                for (int ttl = 1; ttl <= 30; ttl++)
                                {
                                    PingOptions options = new PingOptions(ttl, true);
                                    byte[] buffer = Encoding.ASCII.GetBytes("traceroute");
                                    PingReply reply = ping.Send(pcS[2], 1000, buffer, options);

                                    Console.WriteLine($"{ttl}: {reply.Address ?? (IPAddress)null} - {reply.Status}");

                                    if (reply.Status == IPStatus.Success)
                                        break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error performing traceroute: {ex.Message}");
                        }
                        break;

                    case "listen":
                        try
                        {
                            int port = int.Parse(pcS[2]);
                            TcpListener listener = new TcpListener(IPAddress.Any, port);
                            listener.Start();
                            Console.WriteLine($"Listening on port {port}. Press any key to stop.");

                            while (!Console.KeyAvailable)
                            {
                                if (listener.Pending())
                                {
                                    using (TcpClient client = listener.AcceptTcpClient())
                                    {
                                        Console.WriteLine($"Connection from: {((IPEndPoint)client.Client.RemoteEndPoint).Address}");
                                    }
                                }
                                Thread.Sleep(100);
                            }
                            listener.Stop();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error listening on port: {ex.Message}");
                        }
                        break;

                    case "dns-lookup":
                        try
                        {
                            IPHostEntry entry = Dns.GetHostEntry(pcS[2]);
                            Console.WriteLine($"DNS information for {pcS[2]}:");
                            Console.WriteLine($"Hostname: {entry.HostName}");
                            Console.WriteLine("IP Addresses:");
                            foreach (IPAddress ip in entry.AddressList)
                            {
                                Console.WriteLine(ip);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error performing DNS lookup: {ex.Message}");
                        }
                        break;

                    case "http-get":
                        try
                        {
                            using (HttpClient client = new HttpClient())
                            {
                                string response = client.GetStringAsync(pcS[2]).Result;
                                Console.WriteLine(response);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error performing HTTP GET: {ex.Message}");
                        }
                        break;

                    case "check-connection":
                        try
                        {
                            if (NetworkInterface.GetIsNetworkAvailable())
                            {
                                Console.WriteLine("Network is available");
                                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                                foreach (NetworkInterface ni in interfaces)
                                {
                                    Console.WriteLine($"Name: {ni.Name}");
                                    Console.WriteLine($"Status: {ni.OperationalStatus}");
                                    Console.WriteLine($"Speed: {ni.Speed} bps");
                                    Console.WriteLine();
                                }
                            }
                            else
                            {
                                Console.WriteLine("Network is not available");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error checking network connection: {ex.Message}");
                        }
                        break;

                    case "bandwidth-test":
                        try
                        {
                            using (WebClient client = new WebClient())
                            {
                                Stopwatch sw = Stopwatch.StartNew();
                                byte[] data = client.DownloadData("http://speedtest.ftp.otenet.gr/files/test1Mb.db");
                                sw.Stop();

                                double speedMbps = (data.Length * 8.0 / 1000000.0) / (sw.ElapsedMilliseconds / 1000.0);
                                Console.WriteLine($"Download speed: {speedMbps:F2} Mbps");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error performing bandwidth test: {ex.Message}");
                        }
                        break;

                    case "netstat":
                        try
                        {
                            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
                            TcpConnectionInformation[] connections = properties.GetActiveTcpConnections();

                            Console.WriteLine("Active TCP Connections:");
                            foreach (TcpConnectionInformation connection in connections)
                            {
                                Console.WriteLine($"Local: {connection.LocalEndPoint} <-> Remote: {connection.RemoteEndPoint} [{connection.State}]");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error getting network statistics: {ex.Message}");
                        }
                        break;

                    case "send-packet":
                        try
                        {
                            using (UdpClient udpClient = new UdpClient())
                            {
                                byte[] data = Encoding.ASCII.GetBytes(string.Join(" ", pcS.Skip(3)));
                                udpClient.Send(data, data.Length, pcS[2], int.Parse(pcS[3]));
                                Console.WriteLine("Packet sent successfully");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error sending packet: {ex.Message}");
                        }
                        break;

                    case "route":
                        try
                        {
                            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                            {
                                IPInterfaceProperties ipProps = ni.GetIPProperties();
                                foreach (GatewayIPAddressInformation gateway in ipProps.GatewayAddresses)
                                {
                                    Console.WriteLine($"Interface: {ni.Name}");
                                    Console.WriteLine($"Gateway: {gateway.Address}");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error getting routing information: {ex.Message}");
                        }
                        break;

                    case "mac":
                        try
                        {
                            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                            {
                                PhysicalAddress address = ni.GetPhysicalAddress();
                                byte[] bytes = address.GetAddressBytes();
                                Console.WriteLine($"Interface: {ni.Name}");
                                Console.WriteLine($"MAC: {BitConverter.ToString(bytes)}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error getting MAC addresses: {ex.Message}");
                        }
                        break;
                    case "tcp-connect":
                        try
                        {
                            using (TcpClient client = new TcpClient())
                            {
                                client.Connect(pcS[2], int.Parse(pcS[3]));
                                Console.WriteLine($"Successfully connected to {pcS[2]}:{pcS[3]}");

                                using (NetworkStream stream = client.GetStream())
                                using (StreamWriter writer = new StreamWriter(stream))
                                using (StreamReader reader = new StreamReader(stream))
                                {
                                    if (pcS.Count > 4)
                                    {
                                        writer.WriteLine(string.Join(" ", pcS.Skip(4)));
                                        writer.Flush();
                                        Console.WriteLine("Data sent. Awaiting response...");
                                        Console.WriteLine(reader.ReadLine());
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"TCP connection error: {ex.Message}");
                        }
                        break;

                    case "tcp-server":
                        try
                        {
                            TcpListener server = new TcpListener(IPAddress.Any, int.Parse(pcS[2]));
                            server.Start();
                            Console.WriteLine($"TCP server listening on port {pcS[2]}. Press Enter to stop.");

                            while (!Console.KeyAvailable)
                            {
                                if (server.Pending())
                                {
                                    using (TcpClient client = server.AcceptTcpClient())
                                    using (NetworkStream stream = client.GetStream())
                                    using (StreamReader reader = new StreamReader(stream))
                                    using (StreamWriter writer = new StreamWriter(stream))
                                    {
                                        string data = reader.ReadLine();
                                        Console.WriteLine($"Received: {data}");
                                        writer.WriteLine("Message received");
                                        writer.Flush();
                                    }
                                }
                                Thread.Sleep(100);
                            }
                            server.Stop();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"TCP server error: {ex.Message}");
                        }
                        break;

                    case "smtp-send":
                        try
                        {
                            using (SmtpClient client = new SmtpClient(pcS[2]))
                            {
                                client.Port = int.Parse(pcS[3]);
                                client.Credentials = new NetworkCredential(pcS[4], pcS[5]);
                                client.EnableSsl = true;

                                MailMessage mail = new MailMessage();
                                mail.From = new MailAddress(pcS[4]);
                                mail.To.Add(pcS[6]);
                                mail.Subject = pcS[7];
                                mail.Body = pcS[8];

                                client.Send(mail);
                                Console.WriteLine("Email sent successfully");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"SMTP error: {ex.Message}");
                        }
                        break;

                    case "rdp-check":
                        try
                        {
                            using (TcpClient client = new TcpClient())
                            {
                                var result = client.BeginConnect(pcS[2], 3389, null, null);
                                bool success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
                                if (success)
                                {
                                    Console.WriteLine("RDP port is open and accepting connections");
                                    client.EndConnect(result);
                                }
                                else
                                {
                                    Console.WriteLine("RDP port is closed or filtered");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"RDP check error: {ex.Message}");
                        }
                        break;
                    default:
                        Console.WriteLine($"Unknown network command: '{pcS[1]}'");
                        Console.WriteLine("Available commands: ping, gethost, getip, scan-ports, whois, download, traceroute, listen, " +
                                        "dns-lookup, http-get, check-connection, bandwidth-test, netstat, send-packet, route, mac");
                        break;
                }
            }



            else if (pcS[0].ToLower() == "pwd")
            {
                Console.WriteLine(CommandEnv.CurrentDirDest);
            }
            else if (pcS[0].ToLower() == "cd")
            {
                if (pcS.Count < 2)
                {
                    errs.New($"Usage: cd <directory> - Change the current working directory.");
                    errs.ListThem();
                    //errs.CacheClean();
                    return -1;
                }

                string newDir = pcS[1];

                try
                {
                    if (newDir == "..")
                    {
                        // Move up one directory level
                        newDir = Path.GetDirectoryName(CommandEnv.CurrentDirDest);
                    }
                    else if (newDir == "~")
                    {
                        newDir = CommandEnv.UserHomeDir;
                    }
                    else if (!Path.IsPathRooted(newDir))
                    {
                        newDir = Path.Combine(CommandEnv.CurrentDirDest, newDir);
                    }

                    if (newDir == null)
                    {
                        errs.New($"Error: Cannot navigate above the root directory.");
                        errs.ListThem();
                        //errs.CacheClean();
                        return -1;
                    }

                    newDir = Path.GetFullPath(newDir);

                    if (Directory.Exists(newDir))
                    {
                        CommandEnv.CurrentDirDest = newDir;
                        Environment.CurrentDirectory = newDir;  // Ensure the process working directory is also updated
                                                                //Console.WriteLine($"Changed directory to: {CurrentDirDest}");
                    }
                    else
                    {
                        errs.New($"Error: Directory '{newDir}' does not exist.");
                        errs.ListThem();
                        //errs.CacheClean();
                    }
                }
                catch (Exception ex)
                {
                    errs.New($"Error: {ex.Message}");
                    errs.ListThem();
                    //errs.CacheClean();
                }
            }



            else if (pcS[0].ToLower() == "help")
            {
                List<string> help_lines = [
                    "Builtin:",
                    " echo     ==> echo stuff ==> to echo stuff.",
                    " echoln   ==> to echo something on seperate lines.",
                    " mind     ==> to store something in MindStorage[int][string].",
                    "  mind -add item1 item2 item3    // Adds items to storage",
                    "  mind -get 1                    // Gets item at index 1",
                    "  mind -list                     // Shows all stored items",
                    " reg      ==> to store something in RegisterStorage[string][string].",
                    "  reg>>set>>key>>value     // Sets a key-value pair",
                    "  reg>>get>>key            // Gets value for a key",
                    "  reg>>list                // Lists all registered key-value pairs",
                    " cd",
                    "  cd directory_path  // Changes the current-dir",
                    " exit",
                    "  exit // Exit the application",
                    " macro",
                    "  macro add alias command     // To add a new macro",
                    "  macro remove alias command  // To remove a macro",
                    "  macro update alias command  // To update an existing macro",
                    "  macro list                  // To list all macros",
                    "  macro restore               // To restore the deleted macros",
                  "\n$$:",
                    " $$env::variableName           // Gets value of specific variable",
                    " $$env>>variableName>>value    // Sets environment variable",
                    " $$env>>$all                   // Lists all environment variables",
                    " $$env>>$rem>>variableName     // Removes environment variable",
                  "\nUtils:",
                    " File:",
                    "  file create filename content  // Creates new file with content",
                    "  file read filename            // Displays file content",
                    "  file delete filename          // Deletes specified file",
                    "  file append filename content  // Appends content to file",
                    "  file rename oldname newname   // Renames file",
                    "  file copy source destination  // Copies file",
                    "  file info filename            // Shows file information",
                    "  file exists filename          // Checks if file exists",
                    "  file lines-count filename     // Counts lines in file",
                    "  file search filename text     // Searches for text in file",
                    "  file encrypt filename         // Encrypts file",
                    "  file decrypt filename         // Decrypts file",
                    "  file compress filename        // Compresses file",
                    "  file decompress filename      // Decompresses file",
                    "  file hash filename            // Generates SHA-256 hash",
                    "  file watch filename           // Monitors file for changes",
                    "  file temp [content]           // Creates temporary file",
                    " Directory:",
                    "  dir create dirname            // Creates new directory",
                    "  dir list dirname              // Lists directory contents",
                    "  dir delete dirname            // Deletes directory",
                    "  dir rename oldname newname    // Renames directory",
                    "  dir exists dirname            // Checks if directory exists",
                    "  dir info dirname              // Shows directory information",
                    "  dir size dirname              // Calculates directory size",
                    "  dir count-files dirname       // Counts files in directory",
                    "  dir count-dirs dirname        // Counts subdirectories",
                    "  dir backup dirname            // Creates directory backup",
                    "  dir clean dirname             // Removes all contents",
                    "  dir find dirname pattern      // Finds files matching pattern",
                    "  dir monitor dirname           // Monitors directory changes",
                    " Network:",
                    "  net tcp-connect hostname port [message]      // Establishes TCP connection to a host and port",
                    "  net tcp-server port                          // Creates a TCP server on the specified port",
                    "  net smtp-send server port username password to subject body // Sends email using SMTP",
                    "  net rdp-check hostname                       // Checks if RDP port is accessible",
                    "  net ping hostname                            // Tests connectivity to a host",
                    "  net gethost hostname                         // Gets host information",
                    "  net getip hostname                           // Resolves domain name to IP address",
                    "  net scan-ports hostname startPort endPort    // Scans port range on a host",
                    "  net traceroute hostname                      // Traces route to a destination",
                    "  net whois domain                             // Performs WHOIS lookup",
                    "  net listen port                              // Creates a network listener",
                    "  net netstat                                  // Shows active connections",
                    "  net check-connection                         // Verifies network connectivity",
                    "  net bandwidth-test                           // Tests download speed",
                    "  net download url localfile                   // Downloads file from a URL",
                    "  net http-get url                             // Performs HTTP GET request",
                    "  net mac                                      // Displays MAC addresses",
                    "  net route                                    // Shows routing information",
                    "  net send-packet host port message            // Sends a UDP packet",

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
                //errs.CacheClean();
                errs.New($"hqsh: `{pcS[0]}`: something went wrong!, type `help` for help!");
                //errs.ListThemAll();
                //errs.CacheClean();
                return 0;
            }

            // return the result code

            return 1;
        }


        // Add these macro-related functions
        private static void LoadMacros()
        {
            try
            {
                if (File.Exists(okayMacrosPath))
                {
                    string json = File.ReadAllText(okayMacrosPath);
                    macros = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading macros: {ex.Message}");
                macros = new Dictionary<string, string>();
            }
        }

        private static void SaveMacros(string path)
        {
            try
            {
                string json = JsonSerializer.Serialize(macros, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving macros: {ex.Message}");
            }
        }


        /// <summary>
        ///  DATA ENCRYPTION AND DECRYPTION
        /// </summary>
        /// <returns></returns>

        private static byte[] GenerateRandomKey()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] key = new byte[32]; // 256 bits
                rng.GetBytes(key);
                return key;
            }
        }

        private static byte[] EncryptData(byte[] data, byte[] key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.GenerateIV();

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // Write the IV first
                    msEncrypt.Write(aes.IV, 0, aes.IV.Length);

                    using (ICryptoTransform encryptor = aes.CreateEncryptor())
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(data, 0, data.Length);
                        csEncrypt.FlushFinalBlock();
                    }

                    return msEncrypt.ToArray();
                }
            }
        }

        private static byte[] DecryptData(byte[] encryptedData, byte[] key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;

                // Get the IV from the beginning of the encrypted data
                byte[] iv = new byte[16];
                Array.Copy(encryptedData, 0, iv, 0, iv.Length);
                aes.IV = iv;

                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (ICryptoTransform decryptor = aes.CreateDecryptor())
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(encryptedData, iv.Length, encryptedData.Length - iv.Length);
                        csDecrypt.FlushFinalBlock();
                    }

                    return msDecrypt.ToArray();
                }
            }
        }
    }
}