using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nova_s6.Command
{
    internal class TakeInput
    {
    }

    public class UserInput 
    {
        public static string Input()
        { 
            string input = Console.ReadLine();
            return input;
        }

        public static List<string> Prepare(string input)
        {
            try
            {
                List<string> result = [];
                if (input != null && input != string.Empty)
                {
                    result = input.Split(" ").ToList();
                }
                else 
                {
                    result = [];
                }
                
                return result;
            }
            catch (Exception)
            {
                List<string> result = ["",""];
                return result;
            }
        }

    }
}
