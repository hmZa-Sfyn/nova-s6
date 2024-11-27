using nova.Command;
using nova.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Novaf_Dokr.Customization.configuration.master.conf;

namespace Novaf_Dokr.Customization.configuration
{
    public class master
    {
        public static string _nvf_custom_dir = Path.Combine(CommandEnv.UserHomeDir, "vin_env", "custom");
        public static string novaConfDir = Path.Combine(_nvf_custom_dir, "configurations");
        //public static string CustomPointersFile = Path.Combine(novaConfDir, "conf_pointers.json");
        public static string CustomConfFile = Path.Combine(novaConfDir, "c_files.json");

        public static List<ConfigurationData> _Conf_Files_List = new List<ConfigurationData>();

        public class conf
        {
            public static void EnsureEnvironmentSetup()
            {
                try
                {
                    if (!Directory.Exists(_nvf_custom_dir))
                    {
                        errs.New("Error: `/custom` directory does not exist. Creating directory at " + _nvf_custom_dir);
                        Directory.CreateDirectory(_nvf_custom_dir);
                    }

                    if (!Directory.Exists(novaConfDir))
                    {
                        errs.New("Error: `/custom/configurations` directory does not exist. Creating directory at " + novaConfDir);
                        Directory.CreateDirectory(novaConfDir);
                    }

                    if (!File.Exists(CustomConfFile))
                    {
                        errs.New("Error: `/custom/configurations/c_files.json` file not found. Creating file.");
                        File.WriteAllText(CustomConfFile, "[]");
                    }

                    LoadConfFiles();



                    errs.ListThem();
                    errs.CacheClean();
                }
                catch (Exception ex)
                {
                    errs.New($"Error creating environment setup: {ex.Message}");
                    errs.ListThem();
                    errs.CacheClean();
                }
            }

            public static void LoadConfFiles()
            {
                _Conf_Files_List = [];
                List<ConfigurationData> configList = new List<ConfigurationData>();

                if (File.Exists(CustomConfFile))
                {
                    string existingJson = File.ReadAllText(CustomConfFile);
                    configList = JsonSerializer.Deserialize<List<ConfigurationData>>(existingJson) ?? new List<ConfigurationData>();
                }

                _Conf_Files_List = configList;
            }

            public class ConfigurationData
            {
                public bool Valid { get; set; }
                public string Owner { get; set; }
                public FileData File { get; set; }
                public bool IsEnabled { get; set; }
                public CreationData Creation { get; set; }
                public AllowedListData AllowedList { get; set; }
            }

            public class FileData
            {
                public string Path { get; set; }
                public string FileType { get; set; }
                public string Extension { get; set; }
            }

            public class CreationData
            {
                public string Date { get; set; }
                public string Time { get; set; }
            }

            public class AllowedListData
            {
                public Dictionary<string, string> Users { get; set; }
            }

            public class load
            {
                public static ConfigurationData FromAFile(string filePath)
                {
                    EnsureEnvironmentSetup();

                    try
                    {
                        if (File.Exists(filePath))
                        {
                            string json = File.ReadAllText(filePath);
                            var configData = JsonSerializer.Deserialize<ConfigurationData>(json);
                            return configData;
                        }
                        else
                        {
                            errs.New($"Error: File not found at {filePath}");
                        }
                    }
                    catch (Exception ex)
                    {
                        errs.New($"Error loading JSON data: {ex.Message}");
                    }

                    errs.ListThem();
                    errs.CacheClean();
                    return null;
                }
            }

            public class save
            {
                public static void ToAFile(ConfigurationData configData, string filePath)
                {
                    try
                    {
                        string json = JsonSerializer.Serialize(configData, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(filePath, json);
                    }
                    catch (Exception ex)
                    {
                        errs.New($"Error saving JSON data: {ex.Message}");
                    }
                }
            }
        }
        public class test
        {
            // Method to generate dummy data
            public static ConfigurationData GenerateDummyData(int index)
            {
                return new ConfigurationData
                {
                    Valid = true,
                    Owner = $"user{index:000}",
                    File = new FileData
                    {
                        Path = $"C:/.zlib/programs/file{index}.js",
                        FileType = "javascript",
                        Extension = ".js"
                    },
                    IsEnabled = false,
                    Creation = new CreationData
                    {
                        Date = DateTime.Now.ToString("M/d/yyyy"),
                        Time = DateTime.Now.ToString("HH:mm:ss.ffffff")
                    },
                    AllowedList = new AllowedListData
                    {
                        Users = new Dictionary<string, string>
                    {
                        { "user001", "admin" },
                        { $"user{index + 83:000}", "developer" },
                        { $"user{index + 80:000}", "developer" },
                        { $"user{index + 45:000}", "user" },
                        { $"user{index + 23:000}", "user" }
                    }
                    }
                };
            }

            // Method to append multiple dummy records to JSON file
            public static void AppendDummyData(int times)
            {
                if (!Directory.Exists(_nvf_custom_dir)) Directory.CreateDirectory(_nvf_custom_dir);
                if (!Directory.Exists(novaConfDir)) Directory.CreateDirectory(novaConfDir);
                if (!File.Exists(CustomConfFile)) File.WriteAllText(CustomConfFile, "[]"); // Initialize as an empty array

                List<ConfigurationData> configList = new List<ConfigurationData>();

                if (File.Exists(CustomConfFile))
                {
                    string existingJson = File.ReadAllText(CustomConfFile);
                    configList = JsonSerializer.Deserialize<List<ConfigurationData>>(existingJson) ?? new List<ConfigurationData>();
                }

                for (int i = 0; i < times; i++)
                {
                    ConfigurationData dummyData = GenerateDummyData(i + 1);
                    configList.Add(dummyData);
                }

                string json = JsonSerializer.Serialize(configList, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(CustomConfFile, json);

                LoadConfFiles();
            }
            public static bool Deletefileentry(string _hint)
            {
                string json = File.ReadAllText(CustomConfFile);
                List<ConfigurationData> configList = JsonSerializer.Deserialize<List<ConfigurationData>>(json);

                if (configList.FindAll(x => x.File.Path.Equals(_hint)) != null)
                {
                    foreach (var item in configList.FindAll(x => x.File.Path.Equals(_hint)))
                    {
                        configList.Remove(item);
                    }

                    string jsonx = JsonSerializer.Serialize(configList, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(CustomConfFile, jsonx);

                    LoadConfFiles();
                    return true;
                }
                else
                {
                    errs.CacheClean();
                    errs.New($"Error: config file with path `{_hint}` not found in config register.");
                    errs.ListThem();
                    errs.CacheClean();
                    return false;
                }

            }
            // Method to display JSON data as a table in the console
            public static void DisplayJsonDataAsTable()
            {
                LoadConfFiles();
                if (!File.Exists(CustomConfFile))
                {
                    Console.WriteLine("No data found.");
                    return;
                }

                string json = File.ReadAllText(CustomConfFile);
                List<ConfigurationData> configList = JsonSerializer.Deserialize<List<ConfigurationData>>(json);

                if (configList == null || !configList.Any())
                {
                    Console.WriteLine("No data available in the file.");
                    return;
                }

                Console.WriteLine($"{"Valid",-7} {"Owner",-13} {"Path",-45} {"File Type",-10} {"Enabled",-7} {"Date",-12} {"Time",-15} {"Users",-20}");
                Console.WriteLine(new string('=', 140));
                foreach (var config in configList)
                {
                    string users = string.Join(", ", config.AllowedList.Users.Select(u => $"{u.Key}:{u.Value}"));
                    Console.WriteLine($"{config.Valid,-7} {config.Owner,-13} {config.File.Path,-45} {config.File.FileType,-10} {config.IsEnabled,-7} {config.Creation.Date,-12} {config.Creation.Time,-15} {users,-20}");
                }
            }
            public static void TestAltests()
            {
                ////Generate and append dummy data to the JSON file
                //master.test.AppendDummyData(10); // This will add 10 dummy records to the JSON file.

                ////Display JSON data as a table
                //master.test.DisplayJsonDataAsTable();

                //var configData = master.conf.load.FromAFile(master.CustomConfFile);
                //if (configData != null)
                //{
                //    Console.WriteLine($"Owner: {configData.Owner}");
                //    Console.WriteLine($"File Path: {configData.File.Path}");
                //    // Access other properties as needed
                //}

                ////configData.IsEnabled = true; // Modify data as needed

                //master.conf.save.ToAFile(configData, master.CustomConfFile);

                //master.test.CollectUserInputAndAppend();
            }
            // Method to collect user input and append it to the JSON file
            public static void CollectUserInputAndAppend()
            {
                Console.WriteLine("Please enter the following details:");

                // Collect data from user
                Console.Write("Valid (true/false): ");
                bool valid = bool.Parse(Console.ReadLine());

                Console.Write("Owner (e.g., user001): ");
                string owner = Console.ReadLine();

                Console.Write("File Path (e.g., C:/.zlib/programs/file.js): ");
                string filePath = Console.ReadLine();

                //Console.Write("File Type (e.g., javascript): ");
                string fileType = "json";

                //Console.Write("Extension (e.g., .js): ");
                string extension = ".json";

                Console.Write("Is Enabled (true/false): ");
                bool isEnabled = bool.Parse(Console.ReadLine());

                //Console.Write("Creation Date (M/d/yyyy): ");
                string creationDate = DateTime.Now.ToString("M/d/yyyy");

                //Console.Write("Creation Time (HH:mm:ss): ");
                string creationTime = DateTime.Now.ToString("HH:mm:ss");

                // Collect allowed users
                var allowedUsers = new Dictionary<string, string>();
                string addMoreUsers;
                do
                {
                    Console.WriteLine("Add new user to allowed list:");
                    Console.Write("User ID (e.g., user001): ");
                    string userId = Console.ReadLine();
                    Console.Write("User Role (e.g., admin, developer, user): ");
                    string userRole = Console.ReadLine();
                    allowedUsers[userId] = userRole;

                    Console.Write("Do you want to add another user? (yes/no): ");
                    addMoreUsers = Console.ReadLine();
                } while (addMoreUsers.Equals("yes", StringComparison.OrdinalIgnoreCase));

                // Create ConfigurationData object
                var newConfigData = new ConfigurationData
                {
                    Valid = valid,
                    Owner = owner,
                    File = new FileData
                    {
                        Path = filePath,
                        FileType = fileType,
                        Extension = extension
                    },
                    IsEnabled = isEnabled,
                    Creation = new CreationData
                    {
                        Date = creationDate,
                        Time = creationTime
                    },
                    AllowedList = new AllowedListData
                    {
                        Users = allowedUsers
                    }
                };

                // Append new data to the JSON file
                AppendToJsonFile(newConfigData);
                LoadConfFiles();
            }

            // Method to append a new ConfigurationData object to the JSON file
            public static void AppendToJsonFile(ConfigurationData configData)
            {
                List<ConfigurationData> configList;

                // Ensure the configuration directory and file exist
                EnsureEnvironmentSetup();

                // Read existing configurations from the file
                if (File.Exists(CustomConfFile))
                {
                    string existingJson = File.ReadAllText(CustomConfFile);
                    configList = JsonSerializer.Deserialize<List<ConfigurationData>>(existingJson) ?? new List<ConfigurationData>();
                }
                else
                {
                    configList = new List<ConfigurationData>();
                }

                // Add new configuration data
                configList.Add(configData);

                // Write the updated list back to the file
                string json = JsonSerializer.Serialize(configList, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(CustomConfFile, json);

                Console.WriteLine("Data successfully appended to the JSON file.");
                LoadConfFiles();
            }
        }
    }
}
