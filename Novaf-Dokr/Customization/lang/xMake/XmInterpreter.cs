using nova.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novaf_Dokr.Customization.lang.xMake
{
    public class XmInterpreter
    {
        public static ConsoleColor __CurrentForegroundColor = ConsoleColor.White;
        public static ConsoleColor __CurrentBackgroundColor = ConsoleColor.Black;

        public static List<string> AvalableColors = [
            "red",
            "yellow",
            "blue",
            "green",
            "cyan",
            "black",
            "gray",
            "magenta",
            "white",
            ];
        public static bool Interpret(string _Code)
        {
            List<string> Lines = _Code.Split(';').ToList();
            int x = 0;
            foreach (string Line in Lines)
            {
                List<string> CodeBlocks = Line.Split(" ").ToList();

                if (CodeBlocks[0] == "set")
                {
                    if (CodeBlocks[1] == "background")
                    {
                        if (CodeBlocks[2] == "color")
                        {
                            if (CodeBlocks[3] == "to")
                            {
                                if (AvalableColors.Contains(CodeBlocks[4].ToLower()))
                                {
                                    string ColorName = CodeBlocks[4];
                                    if (ColorName == "red")
                                    {
                                        Console.BackgroundColor = ConsoleColor.Red;
                                        __CurrentBackgroundColor = Console.BackgroundColor;
                                    }
                                    else if (ColorName == "yellow")
                                    {
                                        Console.BackgroundColor = ConsoleColor.Yellow;
                                        __CurrentBackgroundColor = Console.BackgroundColor;
                                    }
                                    else if (ColorName == "blue")
                                    {
                                        Console.BackgroundColor = ConsoleColor.Blue;
                                        __CurrentBackgroundColor = Console.BackgroundColor;
                                    }
                                    else if (ColorName == "green")
                                    {
                                        Console.BackgroundColor = ConsoleColor.Green;
                                        __CurrentBackgroundColor = Console.BackgroundColor;
                                    }
                                    else if (ColorName == "cyan")
                                    {
                                        Console.BackgroundColor = ConsoleColor.Cyan;
                                        __CurrentBackgroundColor = Console.BackgroundColor;
                                    }
                                    else if (ColorName == "black")
                                    {
                                        Console.BackgroundColor = ConsoleColor.Black;
                                        __CurrentBackgroundColor = Console.BackgroundColor;
                                    }
                                    else if (ColorName == "gray")
                                    {
                                        __CurrentBackgroundColor = Console.BackgroundColor;
                                        Console.BackgroundColor = ConsoleColor.Gray;
                                    }
                                    else if (ColorName == "megenta")
                                    {
                                        Console.BackgroundColor = ConsoleColor.Magenta;
                                        __CurrentBackgroundColor = Console.BackgroundColor;
                                    }
                                    else if (ColorName == "white")
                                    {
                                        Console.BackgroundColor = ConsoleColor.White;
                                        __CurrentBackgroundColor = Console.BackgroundColor;
                                    }
                                    else
                                    {
                                        eo.LineWord_CodeBlock_NotFound(x, 5, CodeBlocks[5]);
                                    }
                                }
                                else
                                {
                                    eo.LineWord_CodeBlock_NotFound(x, 4, CodeBlocks[4]);
                                }
                            }
                            else
                            {
                                eo.LineWord_CodeBlock_NotFound(x, 3, CodeBlocks[3]);
                            }
                        }
                        else
                        {
                            eo.LineWord_CodeBlock_NotFound(x, 2, CodeBlocks[2]);
                        }
                    }
                    if (CodeBlocks[1] == "foreground")
                    {
                        if (CodeBlocks[2] == "color")
                        {
                            if (CodeBlocks[3] == "to")
                            {
                                if (AvalableColors.Contains(CodeBlocks[4].ToLower()))
                                {
                                    string ColorName = CodeBlocks[4];
                                    if (ColorName == "red")
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        __CurrentForegroundColor = Console.ForegroundColor;
                                    }
                                    else if (ColorName == "yellow")
                                    {
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                        __CurrentForegroundColor = Console.ForegroundColor;
                                    }
                                    else if (ColorName == "blue")
                                    {
                                        Console.ForegroundColor = ConsoleColor.Blue;
                                        __CurrentForegroundColor = Console.ForegroundColor;
                                    }
                                    else if (ColorName == "green")
                                    {
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        __CurrentForegroundColor = Console.ForegroundColor;
                                    }
                                    else if (ColorName == "cyan")
                                    {
                                        Console.ForegroundColor = ConsoleColor.Cyan;
                                        __CurrentForegroundColor = Console.ForegroundColor;
                                    }
                                    else if (ColorName == "black")
                                    {
                                        Console.ForegroundColor = ConsoleColor.Black;
                                        __CurrentForegroundColor = Console.ForegroundColor;
                                    }
                                    else if (ColorName == "gray")
                                    {
                                        Console.ForegroundColor = ConsoleColor.Gray;
                                        __CurrentForegroundColor = Console.ForegroundColor;
                                    }
                                    else if (ColorName == "megenta")
                                    {
                                        Console.ForegroundColor = ConsoleColor.Magenta;
                                        __CurrentForegroundColor = Console.ForegroundColor;
                                    }
                                    else if (ColorName == "white")
                                    {
                                        Console.ForegroundColor = ConsoleColor.White;
                                        __CurrentForegroundColor = Console.ForegroundColor;
                                    }
                                    else
                                    {
                                        eo.LineWord_CodeBlock_NotFound(x, 5, CodeBlocks[5]);
                                    }
                                }
                                else
                                {
                                    eo.LineWord_CodeBlock_NotFound(x, 4, CodeBlocks[4]);
                                }
                            }
                            else
                            {
                                eo.LineWord_CodeBlock_NotFound(x, 3, CodeBlocks[3]);
                            }
                        }
                        else
                        {
                            eo.LineWord_CodeBlock_NotFound(x, 2, CodeBlocks[2]);
                        }
                    }
                    else
                    {
                        eo.LineWord_CodeBlock_NotFound(x, 1, CodeBlocks[1]);
                    }
                }
                else
                {
                    eo.LineWord_CodeBlock_NotFound(x, 0, CodeBlocks[0]);
                }
                x++;
            }

            return true;
        }
        public class eo
        {
            public static void LineWord_CodeBlock_NotFound(int x, int y, string codeblock)
            {
                errs.New($"xMake: (Line:{x} | Word:{y}): `{codeblock}`: not found");
            }
        }
    }
}
