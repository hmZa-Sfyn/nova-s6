using nova_s6.Command;
using nova_s6.Utils;
using novaf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text.Json;
using Novaf_Dokr.Customization.configuration;
using Microsoft.VisualBasic.FileIO;
using static Novaf_Dokr.Customization.configuration.master.conf;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Drawing;

namespace Novaf_Dokr.Customization
{
    public class system_command
    {
        public static void ProcessSystemCommand(string[] parts)
        {
            try
            {
                if (parts.Length > 1)
                {

                    if (parts[1].ToString().ToLower().Equals("/info"))
                    {
                        Console.WriteLine($"system: {Program.__version__}" +
                                           $"\nuser  : {CommandEnv.CURRENT_USER_NAME}" +
                                           $"\nnode  : {CommandEnv.CURRENT_NODE_NAME}" +
                                           $"\npwd   : {CommandEnv.CurrentDirDest}"); ;
                    }
                    else if (parts[1].ToString().ToLower().Equals("/help"))
                    {
                        Console.WriteLine(" Help:" +
                                        "\n  @sres /pointers -> To access system managed pointers.     (type `@sres /pointers /help` for help)" +
                                        "\n  @sres /config   -> To manage and manipulate config files. (type `@sres /config /help` for help)");
                    }
                    else if (parts[1].ToString().ToLower().Equals("/pointers"))
                    {
                        if (parts.Length >= 2)
                        {



                            if (parts[2].ToString().ToLower().StartsWith(":"))
                            {
                                parts[2] = ReplaceEnvironmentPointers(parts[2]);

                                Console.WriteLine(parts[2]);
                            }
                            else if (parts[2].ToString().ToLower().StartsWith("/set"))
                            {
                                if (parts.Length >= 4)
                                {
                                    CommandEnv.EnvironmentPointers[parts[3]] = string.Join(" ", parts.Skip(4));
                                    CommandEnv.SaveEnvironmentPointers();
                                    //Console.WriteLine($"Environment variable '{parts[2]}' set.");
                                    novaOutput.result($"Environment pointer '{parts[3]}' set.");
                                }
                            }
                            else if (parts[2].ToString().ToLower().Equals("/help"))
                            {
                                Console.WriteLine("System Pointers: (only for develeper, if you are one?)");

                                List<string> _Pointer_Help_Lines = [
                                  "\n Pointers: are digital memory addresses or paths which lead to various variables,functions,plugins,themes,paths or any thing related to vin_env.",
                                  "\n Help:",
                                    "  @sres /pointers /help         -> For this help message.",
                                    "  @sres /pointers :pointer/path -> To access that pointer obj (not a string value or a path, just a obj).",
                                    " Advanced:",
                                    "  @sres /pointers :pointer/path.function_or_arg($more_related_stuff) -> To get that pointer ogj and perform `function_or_arg($more_related_stuff)` with that obj.",
                                    "  @sres /pointers /list              -> To list all pointers with parent path. (`root/path`)",
                                    "  @sres /pointers /clear             -> To clear the pointer dict.",
                                    "  @sres /pointers /save $file/path        -> To save current pointer dict to `$file/path`, which should be a `.pointers.vin` file.",
                                    "                  /save /nerv $file/path  -> To save current pointer dict to `$file/path`, if `$file/path` does not exists, then just create one.",
                                    "  @sres /pointers /load $file/path   -> To load pointers from `$file/path`, which should be a `.pointers.vin` file."
                                    ];

                                foreach (var _line in _Pointer_Help_Lines)
                                {
                                    Console.WriteLine(_line.ToString());
                                }
                            }
                            else if (parts[2].ToString().ToLower().Equals("/list"))
                            {
                                if (CommandEnv.EnvironmentPointers.Count == 0)
                                {
                                    errs.New("No environment pointers set.");
                                    errs.ListThem();
                                    errs.CacheClean();
                                }
                                else
                                {
                                    foreach (var kvp in CommandEnv.EnvironmentPointers)
                                    {
                                        Console.WriteLine($" {kvp.Key} ----------> {kvp.Value}");
                                    }
                                }
                            }
                            else if (parts[2].ToString().ToLower().Equals("/clear"))
                            {
                                CommandEnv.EnvironmentPointers.Clear();
                            }
                            else if (parts[2].ToString().ToLower().Equals("/save"))
                            {
                                if (parts.Length > 3)
                                {
                                    string file_path = "";

                                    if (parts[3].ToString().ToLower().Equals("/nerv"))
                                    {
                                        file_path = parts[4].ToString();
                                    }
                                    else
                                    {
                                        file_path = parts[3].ToString();
                                    }

                                    CustomSaveEnvironmentPointers(file_path);

                                }
                                else
                                {
                                    errs.CacheClean();
                                    errs.New($"Error: Usage: `@sys /pointers /save $file/path`, type `@sys /pointers /help` for help");
                                    errs.ListThem();
                                    errs.CacheClean();
                                }
                            }
                            else if (parts[2].ToString().ToLower().Equals("/load"))
                            {
                                if (parts.Length > 2)
                                {
                                    string file_path = "";

                                    file_path = parts[3].ToString();

                                    CustomLoadEnvironmentPointers(file_path);

                                }
                                else
                                {
                                    errs.CacheClean();
                                    errs.New($"Error: Usage: `@sys /pointers /load $file/path`, type `@sys /pointers /help` for help");
                                    errs.ListThem();
                                    errs.CacheClean();
                                }
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            errs.CacheClean();
                            errs.New($"Error: Usage: `@sys /pointers :$pointer/path.related_command()`, type `@sys /pointers /help` for help");
                            errs.ListThem();
                            errs.CacheClean();
                        }
                    }
                    else if (parts[1].ToString().ToLower().Equals("/config"))
                    {
                        if (parts.Length >= 2)
                        {
                            if (parts[2].ToLower().Equals("/help"))
                            {
                                Console.WriteLine("System Config: (the only fun part!)");

                                List<string> _Config_Help_Lines = [
                                  "\n Config: are those system properties which can make your experiance more better (or worsen it), just by creating some embedded config files according to your needs.",
                                    "  xMake: is the default configuration language for `vin_env` config files. (python support will come soon, wait...)",
                                  "\n Help:",
                                    "  @sres /config /help             -> For this help message.",
                                    " Advanced:",
                                    "  @sres /config /files /list      -> To list all config files. (weather enabled or disabled)",
                                    //"                  /files /list{$query} -> To list all config files, which match the `$query`.",
                                    "                  /files /+ $file/path -> To add a new config file. (type: `@sres /config /files /+` for manual adding, and this is more easy!)",
                                    "                  /files /- $file/path -> To delete a config file.",
                                    //"                  /files /chmod $file/path $query -> To change permissions or states.",
                                    //"                                                      (example: `@sres /files /chmod ./config/myconfig.json.vin enabled=true` to enable a config file on startup.)",
                                    //"                                                      (example: `@sres /files /chmod ./config/myconfig.json.vin allowed=++user001` to add user001 to the allowed list.)",
                                    //"                                                      type: `@sres /files /chmod /help /mods` for query(s)/mode(s) such as `enabled`, `allowed` and more."
                                    "  @sres /config /apply $file/path -> To apply config data present in `$file/path`."
                                ];

                                foreach (var _line in _Config_Help_Lines)
                                {
                                    Console.WriteLine(_line.ToString());
                                }
                            }
                            else if (parts[2].ToLower().Equals("/files"))
                            {
                                if (parts.Length >= 3)
                                {
                                    if (parts[3].ToLower().Equals("/list"))
                                    {
                                        master.test.DisplayJsonDataAsTable();
                                    }
                                    else if (parts[3].ToLower().Equals("/+"))
                                    {
                                        if (parts.Length > 4)
                                        {
                                            Dictionary<string, string> _allowed_list = new Dictionary<string, string>();
                                            _allowed_list[CommandEnv.CURRENT_USER_NAME.ToString()] = "admin";
                                            //_allowed_list["anonymous"] = "user";

                                            var newConfigData = new ConfigurationData
                                            {
                                                Valid = bool.Parse("true"),
                                                Owner = CommandEnv.CURRENT_USER_NAME.ToString(),
                                                File = new FileData
                                                {
                                                    Path = parts[4],
                                                    FileType = "json file",
                                                    Extension = ".json"
                                                },
                                                IsEnabled = bool.Parse("true"),
                                                Creation = new CreationData
                                                {
                                                    Date = DateTime.Now.ToString(),
                                                    Time = DateTime.Now.ToString()
                                                },
                                                AllowedList = new AllowedListData
                                                {
                                                    Users = _allowed_list
                                                }
                                            };

                                            master.test.AppendToJsonFile(newConfigData);
                                        }
                                        else
                                        {
                                            master.test.CollectUserInputAndAppend();
                                        }
                                    }
                                    else if (parts[3].ToLower().Equals("/-"))
                                    {
                                        if (parts.Length >= 4)
                                        {
                                            if (parts[4].EndsWith(".json"))
                                            {
                                                if (!File.Exists(parts[4].ToString()))
                                                {
                                                    try
                                                    {
                                                        if (master.test.Deletefileentry(parts[4].ToString()))
                                                        {
                                                            Console.WriteLine("File deleted successfully!");
                                                            File.Delete(parts[4].ToString());
                                                        }
                                                        else
                                                        {
                                                            errs.CacheClean();
                                                            errs.New($"Error: file cannot be deleted.");
                                                            errs.ListThem();
                                                            errs.CacheClean();
                                                        }
                                                    }
                                                    catch (Exception exxep)
                                                    {
                                                        errs.CacheClean();
                                                        errs.New($"Error: {exxep.ToString()}");
                                                        errs.ListThem();
                                                        errs.CacheClean();
                                                    }
                                                }
                                                else
                                                {
                                                    errs.CacheClean();
                                                    errs.New($"Error: file `{parts[4]}` does not exists.");
                                                    errs.ListThem();
                                                    errs.CacheClean();
                                                }
                                            }
                                            else
                                            {
                                                errs.CacheClean();
                                                errs.New($"Error: Usage: `@sys /config /files /- $file/path.json`, config files cna only contain `.json` extension, type `@sys /config /help` for help");
                                                errs.ListThem();
                                                errs.CacheClean();
                                            }
                                        }
                                        else
                                        {
                                            errs.CacheClean();
                                            errs.New($"Error: Usage: `@sys /config /files /- $file/path`, type `@sys /config /help` for help");
                                            errs.ListThem();
                                            errs.CacheClean();
                                        }
                                    }
                                    else
                                    {
                                        errs.CacheClean();
                                        errs.New($"Error: Usage: `@sys /config /files /more /arguments /if /needed`, type `@sys /config /help` for help");
                                        errs.ListThem();
                                        errs.CacheClean();
                                    }
                                }
                                else
                                {
                                    errs.CacheClean();
                                    errs.New($"Error: Usage: `@sys /config /argument_name $arg1...`, type `@sys /config /help` for help");
                                    errs.ListThem();
                                    errs.CacheClean();
                                }
                            }
                            else if (parts[2].ToLower().Equals("/apply"))
                            {
                                if (parts.Length >= 3)
                                {
                                    string _file_path = parts[3];

                                    if (master._Conf_Files_List.FindAll(x => x.File.Path.Equals(_file_path)) != null)
                                    {
                                        string _code = File.ReadAllText(_file_path);
                                        if (lang.xMake.XmInterpreter.Interpret(_code) == true)
                                        {
                                            Console.WriteLine("Configuration Applied!");
                                        }
                                        else
                                        {
                                            errs.CacheClean();
                                            errs.New($"Error: fault while interpreting code.");
                                            errs.ListThem();
                                            errs.CacheClean();
                                        }
                                    }
                                    else
                                    {
                                        errs.CacheClean();
                                        errs.New($"Error: config file with path `{_file_path}` not found in config register, please add it first using `@sres /config /files /+ {_file_path}`.");
                                        errs.ListThem();
                                        errs.CacheClean();
                                    }
                                }
                                else
                                {
                                    errs.CacheClean();
                                    errs.New($"Error: Usage: `@sys /config /argument_name $arg1...`, type `@sys /config /help` for help");
                                    errs.ListThem();
                                    errs.CacheClean();
                                }
                            }
                            else
                            {
                                errs.CacheClean();
                                errs.New($"Error: Usage: `@sys /config /more /arguments /if /needed`, type `@sys /config /help` for help");
                                errs.ListThem();
                                errs.CacheClean();
                            }
                        }
                        else
                        {
                            errs.CacheClean();
                            errs.New($"Error: Usage: `@sys /config /more /arguments /if /needed`, type `@sys /config /help` for help");
                            errs.ListThem();
                            errs.CacheClean();
                        }
                    }
                    else
                    {
                        errs.CacheClean();
                        errs.New($"Error: Usage: `@sys /more /arguments /if /needed`, type `@sys /help` for help");
                        errs.ListThem();
                        errs.CacheClean();
                    }
                }
                else
                {
                    ProcessSystemCommand(["@sys","/help"]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing command: {ex.Message}");
            }
        }

        public static string ReplaceEnvironmentPointers(string input)
        {
            return Regex.Replace(input, @":(\w+)", match =>
            {
                string key = match.Groups[1].Value;

                string _val;

                try
                {
                    _val = CommandEnv.EnvironmentPointers[key];
                }
                catch (Exception)
                {
                    _val = "";
                }

                // Convert the sPointer to a string representation, assuming it has a meaningful ToString() method
                return _val.ToString();
            });
        }
        public static void CustomSaveEnvironmentPointers(string file_path)
        {
            File.WriteAllText(file_path, JsonSerializer.Serialize(CommandEnv.EnvironmentPointers, new JsonSerializerOptions { WriteIndented = true }));
        }
        public static void CustomLoadEnvironmentPointers(string file_path)
        {
            try
            {
                if (File.Exists(file_path))
                {
                    string json = File.ReadAllText(file_path);
                    CommandEnv.EnvironmentPointers = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                }
                else
                {
                    if (!File.Exists(file_path))
                    {
                        errs.New($"Error: `{file_path}` file not found. Creating file.");
                        File.WriteAllText(file_path, CommandEnv.EnvironmentPointers.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                errs.New($"Error loading environment pointers: {ex.Message}");
                errs.ListThem();
                errs.CacheClean();
            }
        }
    }

    public class system_data_types
    {
        public class sPointer
        { 
            public string __name__ { get; set; }
            public string __type__ { get; set; }
            public string __value__ { get; set; }
            public string __address__ { get; set; }
            public string __path__ { get; set; }


            public DateTime __creation_date__ { get; set; }
            public DateTime __last_update_date__ { get; set; }
        }
    }
}
