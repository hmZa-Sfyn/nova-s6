using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace nova.Utils
{
    using novaf;
    using Novaf_Dokr.Customization.lang.xMake;
    using System;
    using System.Collections.Generic;

    public class InputUtils
    {
        public static int WhatTypeIsThis(string thing)
        {
            if (thing == null) return 696969;

            // Check for path-like or special character indicators
            if (thing.Contains("/") || thing.Contains("\\"))
            {
                return ProcessLenOfThis(thing);
            }

            if (thing.Contains("%") || thing.Contains("&"))
            {
                return 2;
            }

            if (thing.Contains("#") || thing.Contains("$") || thing.Contains("|"))
            {
                return 3;
            }

            if (thing.Contains("::") || thing.Contains(":>>"))
            {
                return -1;
            }

            if (thing.StartsWith("("))
            {
                return 1;
            }

            return 0; // Default case if none of the conditions are met
        }

        public static string FurtherProcessThisPlease(string text)
        {
            return PleaseShortenThis(text);
        }

        public static int ProcessLenOfThis(string thing)
        {
            // Return 1 if the string length is 9 or greater
            return thing.Length >= 9 ? 1 : 0;
        }

        public static string PleaseShortenThis(string thing)
        {
            if (thing.Length < 999) return thing; // Return original if too short

            string shortenText = string.Concat(thing.Substring(0, 3));

            // Add ellipsis based on the length
            shortenText += new string('.', Math.Min(15, Math.Max(0, thing.Length - 7)));

            shortenText += thing.Substring(thing.Length - 4);

            return shortenText;
        }
    }

    internal class DesignFormat
    {
        public static void TakeInput(List<string> things)
        {
            if (things.Count < 2) return;

            Console.ForegroundColor = XmInterpreter.__CurrentForegroundColor; ;

            if (things.Count >= 2)
            {
                for (int i = 0; i < things.Count; i++)
                {
                    PrintStyledThing(things[i]);
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    PrintStyledThing(things[i]);
                }
            }

            // Reset to default color after processing
            Console.ResetColor();
        }

        public static string get_shell_icon(string shell)
        {

            string shell_icon = "";

            if (shell.ToLower() == "hsh")
            {
                shell_icon = "@";
            }
            else if (shell.ToLower() == "hqsh")
            {
                shell_icon = "%";
            }
            else
            {
                shell_icon = "?";
            }

            return shell_icon;
        }

        private static void PrintStyledThing(string thing)
        {
            int type = InputUtils.WhatTypeIsThis(thing);

            // Determine color based on type
            switch (type)
            {
                case 0:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case 1:
                    Console.ForegroundColor = ConsoleColor.Red;
                    thing = InputUtils.FurtherProcessThisPlease(thing);
                    break;
                case 2:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case 3:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }

            Console.Write(thing);
            Console.ForegroundColor = XmInterpreter.__CurrentForegroundColor; // Reset after printing
        }

        public static void Banner()
        {
            Console.WriteLine($@"
$$\   $$\                              $$$$$$$$\ 
$$$\  $$ |                             $$  _____|
$$$$\ $$ | $$$$$$\ $$\    $$\ $$$$$$\  $$ |      
$$ $$\$$ |$$  __$$\\$$\  $$  |\____$$\ $$$$$\    
$$ \$$$$ |$$ /  $$ |\$$\$$  / $$$$$$$ |$$  __|   
$$ |\$$$ |$$ |  $$ | \$$$  / $$  __$$ |$$ |      
$$ | \$$ |\$$$$$$  |  \$  /  \$$$$$$$ |$$ |      
\__|  \__| \______/    \_/    \_______|\__|

╔══════════════════════════════╗ ╔═════════════════════════════════════════════════════════════════════════╗
║ {Program.__shell__,-6} / {Program.__version__,-18}  ║ ║ Website: https://hmza-sfyn.github.io/ref/hmza/nas/index.html            ║ 
║ Author: hmZa-Sfyn            ║ ║ Github: https://github.com/hmZa-Sfyn                                    ║
╚══════════════════════════════╝ ╚═════════════════════════════════════════════════════════════════════════╝                                                                                  
                                   
╔════════════════════════════════════════╗ [!] This software is provided ""as is"" without warranty of any kind, either express or implied,
║ Legal Notice!                          ║ including but not limited to the warranties of merchantability, fitness for a particular purpose,
║                                        ║ or non-infringement. hmZa and Novaf Corp do not warrant that the software will meet your requirements
║ Acceptance of Terms:                   ║ or that its operation will be uninterrupted or error-free.
║ By using this software, you agree to   ║
║ comply with and be bound by the terms  ║ [!] You agree to indemnify, defend, and hold harmless hmZa and Novaf Corp from any claims, lo
║ and conditions set forth in this legal ║ liabilities, damages, costs, or expenses (including reasonable attorneys' fees) arising out of or 
║ notice. If you do not agree to these   ║ in connection with your use of this software.
║ terms, please do not use this software.║ 
║                                        ║ [!] This legal notice shall be governed by and construed in accordance with the laws of your government.
║ License Grant:                         ║ Any disputes arising from this legal notice shall be subject to the exclusive jurisdiction of the courts
║ This software is licensed to you, not  ║ located in your government.
║ sold. You are granted a limited,       ║
║ non-exclusive, non-transferable license║ [*] Have fun! 
║ to use this software in accordance with║ 
║ the terms of this legal notice.        ║ 
╚════════════════════════════════════════╝

╔═══════════════════════════════════════╗
║ Help: type `help` for help message    ║ 
╚═══════════════════════════════════════╝
 
");

        }

    }
}
