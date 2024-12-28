using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Diagnostics;
using System.Collections;

using nova_s6.Utils;
using nova_s6.Command;
using System.Runtime.ExceptionServices;
using System.Reflection.Metadata.Ecma335;
using System.Diagnostics.SymbolStore;

using Novaf_Dokr.Command.env.u_f_net;
using System.Xml.Linq;
//using nova_s6.Tests;


namespace novaf
{
    public class Program
    {
        public static string __version__ = "4.3.za.24";
        public static string __shell__ = "holy-c";
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

        public static List<Shell> Shells = new List<Shell>();

        public static void Initnova(bool isctrlc)
        {
            #region UnitTests
            //UnitTests.Test1();
            //UnitTests.Test2();
            //UnitTests.Test3();
            //UnitTests.Test4();
            //UnitTests.Test_RunCmd();

            //nova_s6.Command.env.Vars.main_test();
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

            //nova_vm.Command.env.doker_lang.Interpreter.Interpret.Helper.ThisCode(File.ReadAllText("G:\\s-cat\\fri3nds\\v-category-projects\\Developer-Grade-Virtual-OS\\Nova-S6\\Novaf-Dokr\\Command\\env\\doker_lang\\test.dokr"));
            //nova_vm.Command.env.doker_lang.Interpreter.Interpret.Helper.Interactive();

            #region Actual Init

            CommandEnv.LoadEnvironmentPointers();
            CommandEnv.LoadEnvironmentVariables();

            nova_s6.shells.hqsh.emul.EnsureMacrosExist();

            //foreach (master.conf.ConfigurationData _cd in master._Conf_Files_List)
            //{
            //    Console.WriteLine(_cd.File.Path.ToString());
            //}

            

            Shells.Add(new Shell
            {
                Num = 0,
                Name = "hqsh",
                Desc = "The secondary/customized shell interface, for `nova-s6` systems, not in previous distros.",
                Version = __version__
            });

            Shells.Add(new Shell
            {
                Num = 1,
                Name = "hsh",
                Desc = "The primary/customized shell for `Novaf-Dokr` and other newer distros.",
                Version = "27/11/2024"
            });

            Shells.Add(new Shell
            {
                Num = 2,
                Name = "ush",
                Desc = "The un-fun shell, just made for nova~interpreter, go on with others, dont touch this!",
                Version = nova_s6.shells.ush.emul.UshShellVersion
            });

            Shells.Add(new Shell
            {
                Num = 3,
                Name = "holy-c",
                Desc = "The super-fun, super-awsome and holy shell based on holy-c language. (Terry Davis)",
                Version = nova_s6.shells.holy_c.shell.HolyC_Version
            });

            Console.WriteLine($"\n(*) sh: type: `!help` for `sh shell help`.");

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
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Black;
                            Initnova(true);
                        };

                        if (!isctrlc)
                        {
                            // The rest of your code
                            if (__shell__.ToLower() == "hsh")
                            {
                                DesignFormat.TakeInput([$"\n{CommandEnv.CURRENT_USER_NAME}", "@", $"{Environment.MachineName}", ": ", $"{CommandEnv.CurrentDirDest} ", $"{DesignFormat.get_shell_icon(__shell__)} "]);
                            }
                            else if (__shell__.ToLower() == "hqsh")
                            {
                                DesignFormat.TakeInput([$"\n[{CommandEnv.CurrentDirDest}]", $" [{CommandEnv.CURRENT_USER_NAME}]", $"\n{DesignFormat.get_shell_icon(__shell__)} "]);
                            }
                            else if (__shell__.ToLower() == "ush")
                            {
                                DesignFormat.TakeInput([$"\n({CommandEnv.CURRENT_USER_NAME})", $" "]);
                            }
                            else if (__shell__.ToLower() == "holy-c")
                            {
                                DesignFormat.TakeInput([$"\nT:\\Home\\", $">"]);
                            }
                            else
                            {
                                DesignFormat.TakeInput([$"\n{CommandEnv.CurrentDirDest}", $"\n{CommandEnv.CURRENT_USER_NAME}]", $"\n{DesignFormat.get_shell_icon(__shell__)}\n"]);
                            }

                            List<string> commands = UserInput.Prepare(UserInput.Input());
                            IdentifyCommand.Identify(commands);
                            List<string> parsed_commands = IdentifyCommand.ReturnThemPlease();

                            if (parsed_commands.Count <= 0)
                                continue;



                            if (parsed_commands[0].StartsWith("!"))
                            {
                                bool ExitStat = nova_s6.shells.sh.interpreter.Interpret(parsed_commands);
                                if (!ExitStat)
                                {
                                    novaOutput.starerror($"Process ended with exit status: {ExitStat}"); 
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
                                    nova_s6.shells.hqsh.emul.ate(parsed_commands);
                                }
                                else if (__shell__.ToLower() == "ush")
                                {
                                    nova_s6.shells.ush.emul.ate(parsed_commands);
                                }
                                else if (__shell__.ToLower() == "holy-c")
                                {
                                    nova_s6.shells.holy_c.shell.ate(parsed_commands, "Nova-S->Repl");
                                }
                                else
                                {
                                    errs.CacheClean();
                                    errs.New($"`{__shell__}`: is not a well-known shell, by me. turning to default `hsh` shell. Type: `!csh --list` for the list of builtin shells.");
                                    errs.ListThemAll();
                                    errs.CacheClean();

                                    __shell__ = "hsh";
                                }

                            }

                            errs.ListThem();
                            errs.CacheClean();

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
                        //errs.CacheClean();
                        //errs.New(exp.ToString());
                        //errs.ListThem();
                        novaOutput.starerror("An error occured");
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
                novaOutput.starerror("An error occured");
            }
        }
        #endregion
    }
}