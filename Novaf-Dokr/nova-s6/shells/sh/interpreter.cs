using nova_s6.Utils;
using novaf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static novaf.Program;

namespace nova_s6.shells.sh
{
    public class interpreter
    {
        public static bool Interpret(List<string> parsed_commands)
        {
            if (parsed_commands[0] == "!csh")
            {
                if (parsed_commands.Count < 1)
                    return false;

                if (parsed_commands[1].ToLower() == "--list")
                {
                    Console.WriteLine(" +-----+---------+-----+--------------------------------------------------------------------------------------------+----------------------+");
                    Console.WriteLine(" | S#  | Name    | Ico | Desc                                                                                       | Version              |");
                    Console.WriteLine(" +-----+---------+-----+--------------------------------------------------------------------------------------------+----------------------+");

                    foreach (var shell in Program.Shells)
                    {
                        Console.WriteLine($" | {shell.Num,-3} | {shell.Name,-7} | {DesignFormat.get_shell_icon(shell.Name),-3} | {shell.Desc,-90} | {shell.Version,-20} |");
                    }
                    Console.WriteLine(" +-----+---------+-----+--------------------------------------------------------------------------------------------+----------------------+");
                }
                else if (parsed_commands[1].ToLower() == "+d")
                {
                    if (parsed_commands.Count < 2)
                        return false;

                    __shell__ = parsed_commands[2];
                }
                else if (parsed_commands[1].ToLower() == "--help")
                {
                    Console.WriteLine(" !csh --list    // To list shells. " +
                                    "\n !csh --help    // To show this help message." +
                                    "\n !csh +d $shell // To deploy `$shell` and logout from the previoes one.");
                }
                else
                {
                    errs.CacheClean();
                    errs.New($"sh: csh: type: `!csh --help` for help.");
                    errs.ListThemAll();
                    errs.CacheClean();
                    return false;
                }
                return true;
            }
            else if (parsed_commands[0] == "!cls")
            {
                Console.Clear();
                return true;
            }
            else if (parsed_commands[0] == "!help")
            {
                List<string> help_lines = [
                    "csh:",
                    " !csh --list    // To list shells. ",
                    " !csh --help    // To show this help message.",
                    " !csh +d $shell // To deploy `$shell` and logout from the previoes one.",
                    "cls:",
                    " !cls // To clear the screen.",
                    "help:",
                    " !help // To view this help message."
                    ];

                foreach (var line in help_lines)
                {
                    Console.WriteLine(line);
                }
                return true;
            }
            else
            {
                return false;
            }
            //return false;
        }
    }
}
