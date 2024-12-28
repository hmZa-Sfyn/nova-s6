using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace nova_s6.Utils
{
    using novaf;
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

            if (thing.Contains("root") || thing.Contains("*"))
            {
                return 1;
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
            if (thing.Length < 350) return thing; // Return original if too short

            string shortenText = string.Concat(thing.Substring(0, 3));

            // Add ellipsis based on the length
            shortenText += new string('.', Math.Min(3, Math.Max(0, thing.Length - 7)));

            shortenText += thing.Substring(thing.Length - 4);

            return shortenText;
        }
    }

    internal class DesignFormat
    {
        public static void TakeInput(List<string> things)
        {
            if (things.Count < 2) return;

            Console.ForegroundColor = ConsoleColor.White;

            for (int i = 0; i < things.Count; i++)
            {
                PrintStyledThing(things[i]);
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
            else if (shell.ToLower() == "ush")
            {
                shell_icon = "&";
            }
            else if (shell.ToLower() == "holy-c")
            {
                shell_icon = "hc";
            }
            else
            {
                shell_icon = "??";
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
            Console.ForegroundColor = ConsoleColor.White; // Reset after printing
        }

        public static void Banner()
        {
            Console.WriteLine($@"
888888ba                                      .d88888b  .d8888P 
88    `8b                                     88.    ""' 88'     
88     88 .d8888b. dP   .dP .d8888b.          `Y88888b. 88baaa. 
88     88 88'  `88 88   d8' 88'  `88 88888888       `8b 88` `88 
88     88 88.  .88 88 .88'  88.  .88          d8'   .8P 8b. .d8 
dP     dP `88888P' 8888P'   `88888P8           Y88888P  `Y888P'

╔══════════════════════════════╗ ╔═════════════════════════════════════════════════════════════════════════╗
║ {Program.__shell__,-6} / {Program.__version__,-18}  ║ ║ Website: https://blog.hmza.vercel.app/posts/nova-project/               ║ 
║ Author: hmZa-Sfyn            ║ ║ Github: https://github.com/hmZa-Sfyn                                    ║
╚══════════════════════════════╝ ╚═════════════════════════════════════════════════════════════════════════╝                                                                                                            
╔═══════════════════════════════════════╗
║ Help: type `help` for help message    ║ 
╚═══════════════════════════════════════╝
 
");

        }

    }
}
